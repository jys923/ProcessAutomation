## OpenCV `cv::quality` 네임스페이스

OpenCV의 `cv::quality` 네임스페이스는 이미지 품질 평가와 관련된 알고리즘을 제공하며, **`opencv_contrib`** 저장소에 포함되어 있습니다. 따라서 이 기능을 사용하려면 OpenCV를 빌드할 때 `opencv_contrib` 모듈을 함께 빌드해야 합니다.

### 주요 클래스 및 함수

`cv::quality` 네임스페이스의 클래스들은 대부분 `QualityBase`를 상속받으며, `compute` 메서드를 통해 품질 평가를 수행합니다.

* **`cv::quality::QualityBase`**
    * 모든 품질 평가 알고리즘의 **기본(Base) 클래스**입니다.

* **`cv::quality::QualityPSNR`**
    * **PSNR (Peak Signal-to-Noise Ratio)**을 계산합니다.
    * 두 이미지 간의 **평균 제곱 오차(MSE)**를 기반으로 화질 손실을 측정하며, 값이 높을수록 원본에 가깝습니다.
    * **사용 예시:** `cv::quality::QualityPSNR::compute(img1, img2);`

* **`cv::quality::QualitySSIM`**
    * **SSIM (Structural Similarity Index Measure)**을 계산합니다.
    * 인간 시각 시스템의 특성을 반영하여 이미지의 **구조적 유사성**을 측정하며, 값이 1에 가까울수록 매우 유사합니다.
    * **사용 예시:** `cv::quality::QualitySSIM::compute(img1, img2);`

* **`cv::quality::QualityGMSD`**
    * **GMSD (Gradient Magnitude Similarity Deviation)**를 계산합니다.
    * **전체 참조 이미지 품질 평가(Full-Reference Image Quality Assessment)** 알고리즘 중 하나로, 이미지의 **경사도(Gradient) 유사도**를 기반으로 합니다.

* **`cv::quality::QualityMSE`**
    * **MSE (Mean Squared Error)**를 계산합니다.
    * 두 이미지의 픽셀 값 차이의 제곱 평균을 나타내며, PSNR 계산의 기반이 됩니다. 값이 낮을수록 두 이미지가 유사합니다.
    * **사용 예시:** `cv::quality::QualityMSE::compute(img1, img2);`

* **`cv::quality::QualityBRISQUE`**
    * **BRISQUE (Blind/Referenceless Image Spatial Quality Evaluator)**를 계산합니다.
    * **참조 이미지 없이(No-Reference)** 이미지 품질을 평가하는 알고리즘으로, 사람의 인지 품질과 상관관계가 높습니다. 모델 파일(`brisque_model_live.yml`)과 범위 파일(`brisque_range_live.yml`)이 필요합니다.

### 주의사항

* `cv::quality`의 기능들을 사용하려면 OpenCV를 빌드할 때 **`opencv_contrib` 모듈을 반드시 포함**시켜야 합니다.
* 이 기능들은 주로 이미지 압축, 복원, 처리 알고리즘의 성능을 평가하는 데 활용됩니다.