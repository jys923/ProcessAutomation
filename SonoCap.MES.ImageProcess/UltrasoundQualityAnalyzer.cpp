#include "UltrasoundQualityAnalyzer.h"
#include "Util.h"

void MyOpenCVWrapper::AnalyzeSharpness(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    // 입력 변환
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    showAndSaveImage(".\\DebugOutput\\inputImage", inputImage);

    // Grayscale 변환
    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    // Tenengrad 계산
    cv::Mat grad_x, grad_y;
    cv::Sobel(grayImage, grad_x, CV_64F, 1, 0);
    cv::Sobel(grayImage, grad_y, CV_64F, 0, 1);
    cv::Mat grad;
    cv::magnitude(grad_x, grad_y, grad);

    cv::Mat gradDisplay;
    cv::normalize(grad, gradDisplay, 0, 255, cv::NORM_MINMAX);
    gradDisplay.convertTo(gradDisplay, CV_8U);
    showAndSaveImage(".\\DebugOutput\\sobel_magnitude", gradDisplay);

    double tenengradValue = cv::sum(grad.mul(grad))[0];

    // Laplacian Variance 계산
    cv::Mat lap;
    cv::Laplacian(grayImage, lap, CV_64F);

    cv::Mat lapDisplay;
    cv::normalize(lap, lapDisplay, 0, 255, cv::NORM_MINMAX);
    lapDisplay.convertTo(lapDisplay, CV_8U);
    showAndSaveImage(".\\DebugOutput\\laplacian_map", lapDisplay);

    cv::Scalar mean, stddev;
    cv::meanStdDev(lap, mean, stddev);

    double laplacianVar = stddev[0] * stddev[0];

    QualityMetrics quality;
    quality.sharpness.tenengrad = tenengradValue;
    quality.sharpness.laplacian = laplacianVar;

	std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeContrast(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    showAndSaveImage(".\\Gray\\inputImage_contrast", inputImage);

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    cv::Scalar mean, stddev;
    cv::meanStdDev(grayImage, mean, stddev);

    double contrastValue = stddev[0];

    QualityMetrics quality;
    quality.contrast = contrastValue;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeBrightness(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    showAndSaveImage(".\\Gray\\inputImage_brightness", inputImage);

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    int radius = static_cast<int>(std::min(imageWidth, imageHeight) * 0.5 * 0.);
    int cx = imageWidth / 2;
    int cy = imageHeight / 2;

    // 중심 원 영역 제외한 마스크 만들기
    cv::Mat mask = cv::Mat::ones(grayImage.size(), CV_8U) * 255;  // 전체 1
    cv::circle(mask, cv::Point(cx, cy), radius, 0, -1); // 중심 원 제거

    // 마스크 시각화용 이미지 저장
    {
        cv::Mat maskedGray;
        grayImage.copyTo(maskedGray, mask);
        /*cv::Mat display;
        cv::normalize(maskedGray, display, 0, 255, cv::NORM_MINMAX);
        display.convertTo(display, CV_8U);
        cv::cvtColor(display, display, cv::COLOR_GRAY2BGRA);
        showAndSaveImage(".\\Gray\\brightness_exclude_center", display);*/
        cv::cvtColor(maskedGray, maskedGray, cv::COLOR_GRAY2BGRA);
        showAndSaveImage(".\\Gray\\brightness_exclude_center", maskedGray);
    }

    // 마스크 영역 제외하고 평균 계산
    cv::Scalar meanScalar = cv::mean(grayImage, mask);
    double brightnessValue = meanScalar[0];

    QualityMetrics quality;
    quality.brightness = brightnessValue;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}


void MyOpenCVWrapper::AnalyzeSNR(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    cv::Scalar meanScalar, stddevScalar;
    cv::meanStdDev(grayImage, meanScalar, stddevScalar);

    double snr = 20.0 * std::log10(meanScalar[0] / (stddevScalar[0] + 1e-6));

    QualityMetrics quality;
    quality.snr = snr;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeSpeckleIndex(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    cv::Scalar meanScalar, stddevScalar;
    cv::meanStdDev(grayImage, meanScalar, stddevScalar);

    double speckleIndex = stddevScalar[0] / (meanScalar[0] + 1e-6);

    QualityMetrics quality;
    quality.speckleIndex = speckleIndex;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeEntropy(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    int histSize = 256;
    float range[] = { 0, 256 };
    const float* histRange = { range };
    cv::Mat hist;
    cv::calcHist(&grayImage, 1, 0, cv::Mat(), hist, 1, &histSize, &histRange);

    hist /= grayImage.total();

    double entropy = 0.0;
    for (int i = 0; i < histSize; ++i)
    {
        float h = hist.at<float>(i);
        if (h > 0)
            entropy -= h * std::log2(h);
    }

    QualityMetrics quality;
    quality.entropy = entropy;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeEdgeDensity(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    cv::Mat edges;
    cv::Canny(grayImage, edges, 50, 150);
    showAndSaveImage(".\\DebugOutput\\canny_edges", edges);

    double edgeDensity = (double)cv::countNonZero(edges) / grayImage.total() * 100.0;

    QualityMetrics quality;
    quality.edgeDensity = edgeDensity;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeLocalVariance(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    const int blockSize = 8;
    double totalVariance = 0.0;
    int count = 0;

    for (int y = 0; y < grayImage.rows; y += blockSize)
    {
        for (int x = 0; x < grayImage.cols; x += blockSize)
        {
            int w = std::min(blockSize, grayImage.cols - x);
            int h = std::min(blockSize, grayImage.rows - y);
            cv::Rect roi(x, y, w, h);
            cv::Mat block = grayImage(roi);

            if (block.total() > 0)
            {
                cv::Scalar mean, stddev;
                cv::meanStdDev(block, mean, stddev);
                totalVariance += stddev[0] * stddev[0];
                count++;
            }
        }
    }

    double avgLocalVariance = (count > 0) ? (totalVariance / count) : 0.0;

    QualityMetrics quality;
    quality.localVariance = avgLocalVariance;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeCNR(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    int w = imageWidth, h = imageHeight;
    cv::Rect roi_signal(w / 4, h / 4, w / 2, h / 2);
    cv::Rect roi_noise(0, 0, w / 8, h / 8);

    cv::Mat cnrDisplay;
    cv::cvtColor(grayImage, cnrDisplay, cv::COLOR_GRAY2BGR);
    cv::rectangle(cnrDisplay, roi_signal, cv::Scalar(0, 255, 0), 2);
    cv::rectangle(cnrDisplay, roi_noise, cv::Scalar(0, 0, 255), 2);
    showAndSaveImage(".\\DebugOutput\\cnr_rois", cnrDisplay);

    cv::Mat signalRegion = grayImage(roi_signal);
    cv::Mat noiseRegion = grayImage(roi_noise);

    cv::Scalar meanSignal, stdSignal;
    cv::meanStdDev(signalRegion, meanSignal, stdSignal);
    cv::Scalar meanNoise, stdNoise;
    cv::meanStdDev(noiseRegion, meanNoise, stdNoise);

    double numerator = std::abs(meanSignal[0] - meanNoise[0]);
    double denominator = std::sqrt(stdSignal[0] * stdSignal[0] + stdNoise[0] * stdNoise[0]);

    double cnr = (denominator > 1e-6) ? (numerator / denominator) : 0.0;

    QualityMetrics quality;
    quality.cnr = cnr;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}

void MyOpenCVWrapper::AnalyzeFFT(System::IntPtr inputBuffer, int imageWidth, int imageHeight, System::IntPtr textBuffer)
{
    uchar* imageData = static_cast<uchar*>(inputBuffer.ToPointer());
    cv::Mat inputImage(imageHeight, imageWidth, CV_8UC4, imageData);
    if (inputImage.empty()) return;

    cv::Mat grayImage;
    cv::cvtColor(inputImage, grayImage, cv::COLOR_BGRA2GRAY);

    cv::Mat grayFloat;
    grayImage.convertTo(grayFloat, CV_32F, 1.0 / 255.0);

    // 1. DFT (shift 포함)
    cv::Mat planes[] = {
        grayFloat,
        cv::Mat::zeros(grayImage.size(), CV_32F)
    };

    cv::Mat complexImage;
    cv::merge(planes, 2, complexImage);
    cv::dft(complexImage, complexImage);
    cv::split(complexImage, planes);

    // 2. Magnitude and Log1p
    cv::magnitude(planes[0], planes[1], planes[0]);  // planes[0] = magnitude
    cv::Mat magnitudeImage = planes[0];
    magnitudeImage += 1.0f;
    cv::log(magnitudeImage, magnitudeImage); // log1p(x)

    // 3. Shift
    magnitudeImage = magnitudeImage(cv::Rect(0, 0, magnitudeImage.cols & -2, magnitudeImage.rows & -2));
    int cx = magnitudeImage.cols / 2;
    int cy = magnitudeImage.rows / 2;

    cv::Mat q0(magnitudeImage, cv::Rect(0, 0, cx, cy));
    cv::Mat q1(magnitudeImage, cv::Rect(cx, 0, cx, cy));
    cv::Mat q2(magnitudeImage, cv::Rect(0, cy, cx, cy));
    cv::Mat q3(magnitudeImage, cv::Rect(cx, cy, cx, cy));
    cv::Mat tmp;
    q0.copyTo(tmp); q3.copyTo(q0); tmp.copyTo(q3);
    q1.copyTo(tmp); q2.copyTo(q1); tmp.copyTo(q2);

    // 4. Save shifted spectrum
    {
        cv::Mat display;
        cv::normalize(magnitudeImage, display, 0, 255, cv::NORM_MINMAX);
        display.convertTo(display, CV_8U);
        cv::cvtColor(display, display, cv::COLOR_GRAY2BGRA);
        showAndSaveImage(".\\DebugOutput\\fftSpectrumShiftedBGRA", display);
    }

    // 5. Compute noise score (based on shifted spectrum)
    double noiseScore = 0.0;
    {
        int w = magnitudeImage.cols;
        int h = magnitudeImage.rows;
        int radius = static_cast<int>(std::min(w, h) * 0.25 * 0.5);

        cv::Mat lowFreqMask = cv::Mat::zeros(h, w, CV_8U);
        cv::circle(lowFreqMask, cv::Point(w / 2, h / 2), radius, 255, -1);

        cv::Mat masked;
        magnitudeImage.copyTo(masked, lowFreqMask); // 마스크를 이용한 복사

        cv::Mat maskDisplay;
        // 마스크 영역만 남기고 나머지를 0으로 (시각화용)
        cv::normalize(masked, maskDisplay, 0, 255, cv::NORM_MINMAX);
        maskDisplay.convertTo(maskDisplay, CV_8U);
        cv::cvtColor(maskDisplay, maskDisplay, cv::COLOR_GRAY2BGRA);
        showAndSaveImage(".\\DebugOutput\\lowFreqOnly", maskDisplay); // 중심 주파수 영역

        cv::Mat highFreqMask;
        cv::bitwise_not(lowFreqMask, highFreqMask);

        cv::Mat highMasked;
        magnitudeImage.copyTo(highMasked, highFreqMask);

        cv::Mat highDisplay;
        cv::normalize(highMasked, highDisplay, 0, 255, cv::NORM_MINMAX);
        highDisplay.convertTo(highDisplay, CV_8U);
        cv::cvtColor(highDisplay, highDisplay, cv::COLOR_GRAY2BGRA);
        showAndSaveImage(".\\DebugOutput\\highFreqOnly", highDisplay); // 노이즈 영역만 시각화

        double totalEnergy = cv::sum(magnitudeImage)[0];
        //double lowFreqEnergy = cv::sum(magnitudeImage, lowFreqMask)[0];
        double lowFreqEnergy = cv::sum(masked)[0];  // 이제 안전하게 합산 가능
        double highFreqEnergy = totalEnergy - lowFreqEnergy;

        if (totalEnergy > 0.0)
            noiseScore = highFreqEnergy / totalEnergy * 100.0;
    }


    // 6. JSON 출력
    QualityMetrics quality;
    quality.fourierNoise = noiseScore;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}