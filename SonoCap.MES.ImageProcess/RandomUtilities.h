#pragma once
#include <random>

class RandomUtilities {
public:
    // 생성자
    RandomUtilities() {
        gen.seed(rd());
    }

    // 정수 난수 생성 함수
    int getRandomInt(int min = 0, int max = 255) {
        std::uniform_int_distribution<> dis(min, max);
        return dis(gen);
    }

    // 실수 난수 생성 함수
    double getRandomDouble(double min = 0.0, double max = 1.0) {
        std::uniform_real_distribution<> dis(min, max);
        return dis(gen);
    }

private:
    std::random_device rd;
    std::mt19937 gen;
};

//// RandomUtilities 인스턴스 생성
//RandomUtilities randomUtil;
//
//// 난수 사용 예시
//int randomValue = randomUtil.getRandomInt();
//double randomDoubleValue = randomUtil.getRandomDouble