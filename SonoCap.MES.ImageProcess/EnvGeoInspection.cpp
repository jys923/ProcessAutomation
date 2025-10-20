#include "EnvGeoInspection.h"

// =========================================================
// 데이터 구조
// =========================================================
struct TrialOutcome {
    double threshold;
    std::vector<EnvGeoData> finalResultPoints;
    std::vector<EnvGeoData> filteredOutXPoints;
    std::vector<EnvGeoData> filteredOutYPoints;
    std::vector<EnvGeoData> detectedPoints;
    EnvGeoData bestRefPoint;
    EnvGeoResult result;
};

// =========================================================
// Trial 단위 처리 함수 (원본 로직 완전 복원)
// =========================================================
bool RunEnvGeoTrial(TrialOutcome& out, const cv::Mat& roiGray, const cv::Rect& roi, double perThreshold, const MyOpenCVWrapper::InspectionParams::EnvGeoParams& cfg)
{
    out.threshold = perThreshold;

    const double minContourArea = cfg.minContourArea;
    const float yTolerance = cfg.yTolerance;

    // 1. Binary + Morphology
    cv::Mat binary;
    cv::threshold(roiGray, binary, perThreshold, 255, cv::THRESH_BINARY);

    cv::Mat eroded, restored;
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    cv::erode(binary, eroded, kernel);
    cv::dilate(eroded, restored, kernel);
	//showAndSaveImage("EnvGeo_Restored", restored);

    // 2. Contour 추출
    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(restored, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);
    if (contours.empty()) return false;

    // 3. Contour → EnvGeoData 변환
    out.detectedPoints.clear();
    for (int i = 0; i < contours.size(); ++i) {
        const auto& contour = contours[i];
        double area = cv::contourArea(contour);
        if (area < minContourArea)
            continue;

        float min_x = std::numeric_limits<float>::max();
        float min_y = std::numeric_limits<float>::max();
        float max_y = std::numeric_limits<float>::lowest();

        for (auto& p : contour) {
            if (p.x < min_x) min_x = p.x;
            if (p.y < min_y) min_y = p.y;
            if (p.y > max_y) max_y = p.y;
        }

        float vertical_mid_y = (min_y + max_y) / 2.0f;
        cv::Point2f extracted(min_x, vertical_mid_y);
        double dist = cv::pointPolygonTest(contour, extracted, false);
        if (dist < 0) continue;

        out.detectedPoints.push_back({ min_x, vertical_mid_y, area, contour, i });
    }

    if (out.detectedPoints.empty()) return false;

    // 4. Y 그룹 필터
    std::sort(out.detectedPoints.begin(), out.detectedPoints.end(),
        [](const EnvGeoData& a, const EnvGeoData& b) {
            return a.vertical_mid_y < b.vertical_mid_y;
        });

    std::vector<EnvGeoData> tempFiltered;
    std::vector<EnvGeoData> current = { out.detectedPoints[0] };

    for (size_t i = 1; i < out.detectedPoints.size(); ++i) {
        const auto& cur = out.detectedPoints[i];
        const auto& base = current.front();

        if (std::abs(cur.vertical_mid_y - base.vertical_mid_y) <= yTolerance)
            current.push_back(cur);
        else {
            auto minX = std::min_element(current.begin(), current.end(),
                [](auto& a, auto& b) { return a.leftmost_x < b.leftmost_x; });
            tempFiltered.push_back(*minX);
            current = { cur };
        }
    }
    if (!current.empty()) {
        auto minX = std::min_element(current.begin(), current.end(),
            [](auto& a, auto& b) { return a.leftmost_x < b.leftmost_x; });
        tempFiltered.push_back(*minX);
    }

    // 5. Contour 외부 점 제거
    std::vector<EnvGeoData> finalFiltered;
    for (auto& d : tempFiltered) {
        cv::Point2f p(d.leftmost_x, d.vertical_mid_y);
        double dist = cv::pointPolygonTest(d.originalContour, p, false);
        if (dist >= 0)
            finalFiltered.push_back(d);
    }

    if (finalFiltered.empty()) return false;

    std::vector<double> xs;
    xs.reserve(finalFiltered.size()); // 성능 최적화 (선택)

    // --- MAD 필터링 안정화 버전 ---
    for (auto& d : finalFiltered) {
        d.leftmost_x = std::round(d.leftmost_x);  // float 오차 제거
        xs.push_back(d.leftmost_x);                // MAD 계산용 수집
    }

    // median, MAD 계산
    std::sort(xs.begin(), xs.end());
    double median = xs[xs.size() / 2];
    std::vector<double> dev;
    for (auto x : xs) dev.push_back(std::abs(x - median));
    std::sort(dev.begin(), dev.end());
    double mad = dev[dev.size() / 2];
    mad = std::max(mad, 1.0); // 최소 보정
    double xTol = 1.4826 * mad * cfg.xMadConstant;

    std::vector<EnvGeoData> filteredByX, filteredOutX;
    for (auto& d : finalFiltered) {
        double x_rel = d.leftmost_x; // ROI 상대좌표 그대로
        if (std::abs(x_rel - median) > xTol)
            filteredOutX.push_back(d);
        else
            filteredByX.push_back(d);
    }

    out.result.madMetrics.median_x = median;
    out.result.madMetrics.mad_x = mad;
    out.result.madMetrics.x_tolerance = xTol;
    out.result.madMetrics.filtered_out_by_x.clear();
    for (const auto& d : filteredOutX) {
        out.result.madMetrics.filtered_out_by_x.push_back(
            cv::Point(static_cast<int>(d.leftmost_x + roi.x),
                static_cast<int>(d.vertical_mid_y + roi.y)));
    }

    // 7. Y 간격 필터링
    std::vector<EnvGeoData> filteredByY, filteredOutY;
    const double targetY = cfg.targetYInterval;
    const double yTol2 = cfg.yIntervalTolerance;

    std::map<int, int> matchCounts;
    for (auto& ref : finalFiltered) {
        int count = 1;
        for (auto& other : finalFiltered) {
            if (&ref == &other) continue;
            double diff = std::abs(other.vertical_mid_y - ref.vertical_mid_y);
            double rem = fmod(diff, targetY);
            if (std::abs(rem) < yTol2 || std::abs(rem - targetY) < yTol2)
                count++;
        }
        matchCounts[ref.originalContourIndex] = count;
    }

    auto best = std::max_element(matchCounts.begin(), matchCounts.end(),
        [](auto& a, auto& b) { return a.second < b.second; });

    if (best != matchCounts.end()) {
        auto it = std::find_if(finalFiltered.begin(), finalFiltered.end(),
            [&](auto& d) { return d.originalContourIndex == best->first; });
        if (it != finalFiltered.end())
            out.bestRefPoint = *it;
    }

    if (out.bestRefPoint.area > 0) {
        for (auto& d : finalFiltered) {
            double diff = std::abs(d.vertical_mid_y - out.bestRefPoint.vertical_mid_y);
            double rem = fmod(diff, targetY);
            if (std::abs(rem) < yTol2 || std::abs(rem - targetY) < yTol2)
                filteredByY.push_back(d);
            else
                filteredOutY.push_back(d);
        }
    }
    else {
        filteredByY = finalFiltered;
    }

    // 8. X/Y 필터 조합
    std::vector<EnvGeoData> finalResult;
    bool useX = cfg.enableXFilter;
    bool useY = cfg.enableYFilter;

    if (useX && useY) {
        std::set<int> yIndices;
        for (auto& d : filteredByY) yIndices.insert(d.originalContourIndex);
        for (auto& d : filteredByX)
            if (yIndices.count(d.originalContourIndex))
                finalResult.push_back(d);
    }
    else if (useX) {
        finalResult = filteredByX;
    }
    else if (useY) {
        finalResult = filteredByY;
    }
    else {
        finalResult = finalFiltered;
    }

    out.result.yIntervalMetrics.target_y_interval = targetY;
    out.result.yIntervalMetrics.y_tolerance = yTol2;

    // Y 필터 관련 결과 저장 추가 (여기 삽입)
    out.result.yIntervalMetrics.filtered_out_by_y.clear();
    for (const auto& d : filteredOutY) {
        out.result.yIntervalMetrics.filtered_out_by_y.push_back(
            cv::Point(static_cast<int>(d.leftmost_x + roi.x),
                static_cast<int>(d.vertical_mid_y + roi.y)));
    }

    // 9. 결과 구성
    if (finalResult.empty()) return false;

    out.finalResultPoints = finalResult;
    out.filteredOutXPoints = filteredOutX;
    out.filteredOutYPoints = filteredOutY;

    for (auto& d : finalResult)
        out.result.finalPoints.push_back(cv::Point(d.leftmost_x + roi.x, d.vertical_mid_y + roi.y));
    for (auto& d : out.detectedPoints)
        out.result.findPoints.push_back(cv::Point(d.leftmost_x + roi.x, d.vertical_mid_y + roi.y));

    if (out.bestRefPoint.area > 0)
        out.result.yIntervalMetrics.best_ref_point =
        cv::Point(out.bestRefPoint.leftmost_x + roi.x, out.bestRefPoint.vertical_mid_y + roi.y);

    return true;
}

// =========================================================
// 드로잉 함수 (최종 1회만)
// =========================================================
static void DrawEnvGeoTrial(cv::Mat& srcImg, const cv::Rect& roi, const TrialOutcome& t)
{
    for (auto& d : t.detectedPoints) {
        std::vector<std::vector<cv::Point>> c = { d.originalContour };
        for (auto& p : c[0]) p += cv::Point(roi.x, roi.y);
        cv::drawContours(srcImg, c, -1, GreenA, 1);
    }

    for (auto& d : t.filteredOutXPoints)
        cv::circle(srcImg, { int(d.leftmost_x + roi.x), int(d.vertical_mid_y + roi.y) }, 2, OrangeA, -1);

    for (auto& d : t.filteredOutYPoints)
        cv::circle(srcImg, { int(d.leftmost_x + roi.x), int(d.vertical_mid_y + roi.y) }, 2, BlueA, -1);

    for (auto& d : t.finalResultPoints)
        cv::circle(srcImg, { int(d.leftmost_x + roi.x), int(d.vertical_mid_y + roi.y) }, 2, RedA, -1);

    if (t.bestRefPoint.area > 0)
        cv::circle(srcImg, { int(t.bestRefPoint.leftmost_x + roi.x), int(t.bestRefPoint.vertical_mid_y + roi.y) }, 3, CyanA, -1);

    cv::rectangle(srcImg, roi, YellowA, 1);
    showAndSaveImage("EnvGeo_End", srcImg);
}

void LogEnvGeoHeader()
{
    Logger::Debug("---- EnvGeoResult Summary ----");
    Logger::Debug("Idx | findPts | finalPts | median_x | mad_x | target_y_interval | y_tolerance");
    Logger::Debug("--------------------------------------------------------------------------");
}

// 2) 인덱스 포함 단일 행 출력
void LogEnvGeoRow(const TrialOutcome& t, size_t idx)
{
    const auto& e = t.result;

    Logger::Debug(
        "%3zu | %7zu | %9zu | %9.3f | %6.3f | %17.2f | %11.2f",
        idx,
        e.findPoints.size(),
        e.finalPoints.size(),
        e.madMetrics.median_x,
        e.madMetrics.mad_x,
        e.yIntervalMetrics.target_y_interval,
        e.yIntervalMetrics.y_tolerance
    );
}

double ComputePhaseError(const TrialOutcome& t)
{
    if (t.finalResultPoints.empty())
        return std::numeric_limits<double>::infinity();

    const double refY = t.bestRefPoint.vertical_mid_y;
    const double interval = t.result.yIntervalMetrics.target_y_interval;

    double sumErr = 0.0;
    for (auto& p : t.finalResultPoints)
    {
        double diff = std::fabs(p.vertical_mid_y - refY);
        double r = std::fmod(diff, interval);
        double err = std::min(r, interval - r); // 1과 29를 동일하게 취급
        sumErr += err;
    }
    return sumErr / t.finalResultPoints.size(); // 평균 오차
}


TrialOutcome SelectBestTrial(const std::vector<TrialOutcome>& trials)
{
    TrialOutcome best = trials[0];

    for (size_t i = 1; i < trials.size(); ++i)
    {
        const auto& t = trials[i];

        int currCount = static_cast<int>(t.finalResultPoints.size());
        int bestCount = static_cast<int>(best.finalResultPoints.size());

        if (currCount > bestCount) {
            best = t;
            continue;
        }

        if (currCount == bestCount)
        {
            double bestPhaseErr = ComputePhaseError(best);
            double currPhaseErr = ComputePhaseError(t);

            if (currPhaseErr < bestPhaseErr) {
                best = t;
                continue;
            }

            if (std::fabs(currPhaseErr - bestPhaseErr) < 1e-6 &&
                t.result.madMetrics.mad_x < best.result.madMetrics.mad_x)
            {
                best = t;
                continue;
            }
        }
    }

    return best;
}


void MyOpenCVWrapper::EnvGeoInspection(cv::Mat& srcImg, EnvGeoResult& result)
{
    const auto& cfg = ConfigManager::getInstance().getInspectionParams().envGeo;

    auto prep = CalcHistBasedThreshold(srcImg, cfg.roi);
    auto thresholds = GenerateThresholds(prep.baseThreshold, 20.0, 40);

    std::vector<TrialOutcome> trials;

    for (double t : thresholds) {
    TrialOutcome temp;
        if (RunEnvGeoTrial(temp, prep.roiGray, prep.roi, t, cfg))
            trials.push_back(temp);
    }

    if (trials.empty()) {
        Logger::Warning("No valid EnvGeo trials found.");
        return;
    }
    else
    {
        LogEnvGeoHeader();
        for (size_t i = 0; i < trials.size(); ++i) {
            LogEnvGeoRow(trials[i], i);
        }
    }

    const TrialOutcome& chosen = SelectBestTrial(trials);//trials.at(5);// trials.front();
    
    Logger::Information("Final result threshold = {0}, Points = {1}",
        chosen.threshold, chosen.result.finalPoints.size());

    // --- Detected Points 전체 출력 (y 오름차순 정렬) ---
    {
        std::vector<EnvGeoData> allPoints;

        allPoints.insert(allPoints.end(), chosen.finalResultPoints.begin(), chosen.finalResultPoints.end());
        allPoints.insert(allPoints.end(), chosen.filteredOutXPoints.begin(), chosen.filteredOutXPoints.end());
        allPoints.insert(allPoints.end(), chosen.filteredOutYPoints.begin(), chosen.filteredOutYPoints.end());

        std::sort(allPoints.begin(), allPoints.end(),
            [](const EnvGeoData& a, const EnvGeoData& b) {
                if (a.vertical_mid_y == b.vertical_mid_y)
                    return a.leftmost_x < b.leftmost_x;
                return a.vertical_mid_y < b.vertical_mid_y;
            });

        allPoints.erase(
            std::unique(allPoints.begin(), allPoints.end(),
                [](const EnvGeoData& a, const EnvGeoData& b) {
                    return std::abs(a.vertical_mid_y - b.vertical_mid_y) < 0.1 &&
                        std::abs(a.leftmost_x - b.leftmost_x) < 0.1;
                }),
            allPoints.end());

        const double targetY = chosen.result.yIntervalMetrics.target_y_interval;
        const double tol = chosen.result.yIntervalMetrics.y_tolerance;
        const double refY = chosen.result.yIntervalMetrics.best_ref_point.y;

        std::ostringstream ossY;
        ossY << "\nY-Interval Verification (refY=" << refY
            << ", target=" << targetY
            << ", tol=" << tol << ")\n";
        ossY << "-------------------------------------------------------------\n";
        ossY << " No |    X   |    Y   | ΔY(refY) | Remainder |  Status\n";
        ossY << "-------------------------------------------------------------\n";

        int idx = 1;
        for (const auto& d : allPoints)
        {
            double dY = d.vertical_mid_y - refY;
            double rem = std::fmod(std::abs(dY), targetY);
            if (rem < 0) rem += targetY;

            bool ok = (std::abs(rem) < tol) || (std::abs(rem - targetY) < tol);

            ossY << std::setw(3) << idx++ << " | "
                << std::setw(6) << std::fixed << std::setprecision(1) << d.leftmost_x << " | "
                << std::setw(6) << d.vertical_mid_y << " | "
                << std::setw(8) << std::fixed << std::setprecision(1) << dY << " | "
                << std::setw(10) << std::fixed << std::setprecision(2) << rem << " | "
                << (ok ? "OK" : "OUT") << "\n";
        }

        ossY << "-------------------------------------------------------------\n";
        Logger::Information("{0}", gcnew System::String(ossY.str().c_str()));
    }


    DrawEnvGeoTrial(srcImg, prep.roi, chosen);
    result = chosen.result;
}

void MyOpenCVWrapper::EnvGeoInspection(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr resultBuffer, System::IntPtr textBuffer)
{
    // 초기화
    EnvGeoResult geoResult;
    memset(resultBuffer.ToPointer(), 0, imageWidth * imageHeight * 4);

    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) {
        Logger::Error("Image loading failed: Input image is empty. Image Size: {0}x{1}", imageHeight, imageWidth);
        return;
    }

    Logger::Information(
        "Image size: {0}x{1}, Channels: {2}",
        imageWidth, imageHeight, inputImage.channels()
    );

    cv::Mat resultImage = inputImage.clone();

    cv::Mat gray;
    cv::cvtColor(resultImage, gray, cv::COLOR_BGRA2GRAY);

    // 검사 실행 (여기서 resultImage가 업데이트 됨)
    EnvGeoInspection(resultImage, geoResult);

    // JSON 결과 직렬화
    nlohmann::json j = geoResult;
    std::string finalText = j.dump(2); // 가독성을 위해 2칸 들여쓰기 추가
    Logger::Information("\n{0}",gcnew System::String(finalText.c_str()));
    // 복사
    memcpy(textBuffer.ToPointer(), finalText.c_str(), finalText.size() + 1);
    memcpy(resultBuffer.ToPointer(), resultImage.data, resultImage.total() * resultImage.elemSize());
}