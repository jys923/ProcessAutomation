#include "ConfigManager.h"
#include <iostream>
#include <fstream>
#include <chrono>
#include <cstdio>
#include <iomanip> // for std::setw

namespace MyOpenCVWrapper {

    ConfigManager& ConfigManager::getInstance() {
        static ConfigManager instance;
        return instance;
    }

    ConfigManager::ConfigManager()
        : m_isConfigLoaded(false) {
        resetToDefaults();
        std::cout << "ConfigManager: 기본 생성자가 호출되었습니다. 모든 파라미터가 기본값으로 초기화됩니다." << std::endl;
    }

    void ConfigManager::resetToDefaults() {
        m_generalSettings = GeneralSettingsParams();
        m_inspectionParams = InspectionParams();
        // AnalyzerParams는 제공된 JSON 예시에 없으므로 ConfigManager에서 제외
        m_isConfigLoaded = false;
    }

    void ConfigManager::loadConfigFromJsonString(const std::string& jsonString) {
        try {
            nlohmann::json j = nlohmann::json::parse(jsonString);
            j.value("GeneralSettings", nlohmann::json::object()).get_to(m_generalSettings);
            j.value("Inspection", nlohmann::json::object()).get_to(m_inspectionParams);
            // Analyzer 섹션은 제공된 JSON 예시에 없으므로 로드 로직에서 제외

            m_isConfigLoaded = true;
            std::cout << "ConfigManager: JSON 문자열로부터 설정이 성공적으로 로드되었습니다." << std::endl;
        }
        catch (const nlohmann::json::exception& e) {
            std::cerr << "오류: JSON 문자열 파싱 중 예외 발생: " << e.what() << std::endl;
            resetToDefaults();
            m_isConfigLoaded = false;
        }
        catch (const std::exception& e) {
            std::cerr << "오류: 알 수 없는 예외 발생: " << e.what() << std::endl;
            resetToDefaults();
            m_isConfigLoaded = false;
        }
    }

    void ConfigManager::loadConfigFromFile(const std::string& filePath) {
        std::cout << "ConfigManager: 파일로부터 설정 로드를 시도합니다: " << filePath << std::endl;
        std::ifstream file(filePath);

        if (!file.is_open()) {
            std::cerr << "경고: JSON 설정 파일을 열 수 없습니다: " << filePath << ". 기본 설정으로 파일을 생성합니다." << std::endl;
            resetToDefaults();
            saveConfigToFile(filePath);
            m_isConfigLoaded = false;
            return;
        }

        try {
            nlohmann::json j;
            file >> j;
            file.close();

            j.value("GeneralSettings", nlohmann::json::object()).get_to(m_generalSettings);
            j.value("Inspection", nlohmann::json::object()).get_to(m_inspectionParams);
            // Analyzer 섹션은 제공된 JSON 예시에 없으므로 로드 로직에서 제외

            m_isConfigLoaded = true;
            std::cout << "ConfigManager: JSON 파일로부터 설정이 성공적으로 로드되었습니다." << std::endl;

        }
        catch (const nlohmann::json::exception& e) {
            std::cerr << "오류: JSON 파일 파싱 중 예외 발생 (" << filePath << "): " << e.what() << std::endl;
            resetToDefaults();
            m_isConfigLoaded = false;
        }
        catch (const std::exception& e) {
            std::cerr << "오류: 알 수 없는 예외 발생 (" << filePath << "): " << e.what() << std::endl;
            resetToDefaults();
            m_isConfigLoaded = false;
        }
    }

    void ConfigManager::saveConfigToFile(const std::string& filePath) const {
        std::cout << "ConfigManager: 현재 설정을 파일로 저장합니다: " << filePath << std::endl;
        std::ofstream file(filePath);

        if (!file.is_open()) {
            std::cerr << "오류: 설정 파일을 저장할 수 없습니다: " << filePath << std::endl;
            return;
        }

        nlohmann::json j;
        j["GeneralSettings"] = m_generalSettings;
        j["Inspection"] = m_inspectionParams;
        // Analyzer 섹션은 제공된 JSON 예시에 없으므로 저장 로직에서 제외

        file << std::setw(4) << j << std::endl;
        file.close();
        std::cout << "ConfigManager: 설정이 성공적으로 파일에 저장되었습니다." << std::endl;
    }

    void SetConfigJson(const char* jsonString) {
        ConfigManager::getInstance().loadConfigFromJsonString(jsonString);
    }

} // namespace MyOpenCVWrapper