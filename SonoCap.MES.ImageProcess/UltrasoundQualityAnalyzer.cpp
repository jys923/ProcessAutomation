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

    cv::Scalar meanScalar = cv::mean(grayImage);
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

    // 1. DFT
    cv::Mat planes[] = { cv::Mat_<float>(grayImage), cv::Mat::zeros(grayImage.size(), CV_32F) };
    cv::Mat complexImage;
    cv::merge(planes, 2, complexImage);
    cv::dft(complexImage, complexImage);

    // 2. Magnitude
    cv::split(complexImage, planes);
    cv::magnitude(planes[0], planes[1], planes[0]);
    cv::Mat magnitudeImage = planes[0];

    // 3. Log scale (shift 전)
    magnitudeImage += cv::Scalar::all(1);
    cv::log(magnitudeImage, magnitudeImage);

    // 🔥 저장 (Shift 안 한 FFT 스펙트럼)
    {
        cv::Mat display;
        cv::normalize(magnitudeImage, display, 0, 255, cv::NORM_MINMAX);
        display.convertTo(display, CV_8U);
        cv::cvtColor(display, display, cv::COLOR_GRAY2BGRA);
        showAndSaveImage(".\\DebugOutput\\fftSpectrumBGRA", display);
    }

    // 🧪 Fourier 노이즈 점수 계산
    double  noiseScore = 0.0f;
    {
        int cx = magnitudeImage.cols / 2;
        int cy = magnitudeImage.rows / 2;
        int radius = static_cast<int>(std::min(imageWidth, imageHeight) * 0.05); // 중심 5%

        cv::Mat lowFreqMask = cv::Mat::zeros(magnitudeImage.size(), CV_8U);
        cv::circle(lowFreqMask, cv::Point(cx, cy), radius, 255, -1);

        double totalEnergy = cv::sum(magnitudeImage)[0];
        double lowFreqEnergy = cv::sum(magnitudeImage & lowFreqMask)[0]; // mask가 255일 때만
        double highFreqEnergy = totalEnergy - lowFreqEnergy;

        if (totalEnergy > 0.0)
            noiseScore = highFreqEnergy / totalEnergy * 100.0;
    }

    // 4. Shift
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

    // 5. Normalize + BGRA 변환 (Shift한 FFT 스펙트럼)
    {
        cv::Mat display;
        cv::normalize(magnitudeImage, display, 0, 255, cv::NORM_MINMAX);
        display.convertTo(display, CV_8U);
        cv::cvtColor(display, display, cv::COLOR_GRAY2BGRA);
        showAndSaveImage(".\\DebugOutput\\fftSpectrumShiftedBGRA", display);
    }

    // 6. JSON 형식 결과 저장
    QualityMetrics quality;
    quality.fourierNoise = noiseScore;

    std::string jsonString = objectToJsonString(quality);
    memcpy(textBuffer.ToPointer(), jsonString.c_str(), jsonString.size() + 1);

    std::cout << "[" << __func__ << "] " << jsonString << std::endl;
}
