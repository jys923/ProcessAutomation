# ORB 기반 회전 추정 노트

## ✅ 목적
두 이미지(기준 이미지, 회전된 이미지) 간의 회전 각도를 추정한다.

---

## 📌 주요 함수 구조

```cpp
float estimateRotationByORB(const cv::Mat& reference, const cv::Mat& rotated) {
    cv::Ptr<cv::ORB> orb = cv::ORB::create(500);
    std::vector<cv::KeyPoint> kp1, kp2;
    cv::Mat des1, des2;
    orb->detectAndCompute(reference, cv::noArray(), kp1, des1);
    orb->detectAndCompute(rotated,  cv::noArray(), kp2, des2);

    if (des1.empty() || des2.empty()) return 0.0f;

    std::vector<cv::DMatch> matches;
    cv::BFMatcher matcher(cv::NORM_HAMMING);
    matcher.match(des1, des2, matches);

    std::sort(matches.begin(), matches.end(), [](const auto& a, const auto& b) {
        return a.distance < b.distance;
    });

    if (matches.size() < 10) return 0.0f;

    std::vector<cv::Point2f> pts1, pts2;
    for (int i = 0; i < std::min(50, (int)matches.size()); ++i) {
        pts1.push_back(kp1[matches[i].queryIdx].pt);
        pts2.push_back(kp2[matches[i].trainIdx].pt);
    }

    cv::Mat affine = cv::estimateAffinePartial2D(pts2, pts1);
    if (affine.empty()) return 0.0f;

    float angleRad = std::atan2(affine.at<double>(0, 1), affine.at<double>(0, 0));
    return angleRad * 180.0f / CV_PI;
}
```

---

## 🧠 핵심 개념 요약

### ORB 처리 흐름
- `detectAndCompute` : ORB 특징점 추출 + 디스크립터 생성
- `BFMatcher::match` : Hamming 거리 기반 디스크립터 매칭
- `std::sort` : 매칭 점 중 상위 N개 추출 (노이즈 제거 목적)
- `estimateAffinePartial2D(pts2, pts1)` :
    - `pts2`를 `pts1`로 정합되도록 회전/이동 변환 행렬 추정

---

## 🧮 어파인 변환 행렬 상세 설명

### 일반 Affine 변환 행렬 (2D):
```
[ a  b  tx ]
[ c  d  ty ]
```

- `a, b, c, d`: 회전 + 스케일 + 비대칭 스케일(Shear)
- `tx, ty`: 이동 (translation)

---

### Partial Affine (`estimateAffinePartial2D`)에서는:
- **스케일(S)** + **회전(θ)** + **이동(T)** 만 포함
- 행렬은 다음과 같은 형태:

```
[ cosθ  -sinθ  tx ]
[ sinθ   cosθ  ty ]
```

→ 즉, **회전만 포함된 rigid-like 변환**

---

### 회전각 계산 공식:

```cpp
θ = atan2(affine[0,1], affine[0,0])
```

- `affine[0,0] = cosθ`
- `affine[0,1] = -sinθ`
- `atan2(-sinθ, cosθ)` → **시계 방향이 음수, 반시계 방향이 양수**

---

## ✅ 사용 예시

```cpp
float angle = estimateRotationByORB(referenceImage, rotatedImage);
rotateBack(rotatedImage, restoredImage, -angle);
```

- 회전 방향이 시계 방향이면 음수
- 원래 방향으로 되돌릴 때는 `-angle`로 보정

---

## 💡 팁

- 입력 이미지는 Grayscale로 변환해서 사용하는 것이 일반적
- 특징점 매칭 수가 너무 적으면 정확도 낮음 (`if (matches.size() < 10)` 조건으로 필터링)
- 이미지 품질이 서로 너무 다르면 실패 가능성 있음
- 스케일까지 추정하려면 `estimateAffine2D()` 사용 가능
