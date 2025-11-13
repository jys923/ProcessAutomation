#pragma once

#include <string>
#include <nlohmann/json.hpp>
#include <opencv2/opencv.hpp> // cv::Rect 등을 위해 유지

namespace MyOpenCVWrapper {

    //-------------------------------------------------------------------------
    // ROI 파라미터 구조체 (설정)
    // 모든 값은 그대로 출력됩니다. (0이어도 생략하지 않음)
    //-------------------------------------------------------------------------
    struct RoiParams {
        int x = 0;
        int y = 0;
        int width = 0;
        int height = 0;
    };

    inline void to_json(nlohmann::json& j, const RoiParams& r) {
        j = nlohmann::json{ {"X", r.x}, {"Y", r.y}, {"Width", r.width}, {"Height", r.height} };
    }
    inline void from_json(const nlohmann::json& j, RoiParams& r) {
        // 필드가 없으면 기본값 유지 (안정성)
        r.x = j.value("X", r.x);
        r.y = j.value("Y", r.y);
        r.width = j.value("Width", r.width);
        r.height = j.value("Height", r.height);
    }


    //-------------------------------------------------------------------------
    // GeneralSettings 섹션 파라미터 구조체 (설정)
    //-------------------------------------------------------------------------
    struct GeneralSettingsParams {
        bool debugImg = false;
        bool enableRotation = false;
        std::string logLevel = "Info";

        struct ReferenceImageParams {
            std::string path = "./reference_image.png";
            bool loadOnStartup = false;
        } referenceImage; // <-- 여기에 인스턴스 선언

        GeneralSettingsParams() = default;
    };

    inline void to_json(nlohmann::json& j, const GeneralSettingsParams& g) {
        j = nlohmann::json{
			{"DebugImg", g.debugImg},
			{"EnableRotation", g.enableRotation},
            {"LogLevel", g.logLevel},
            {"ReferenceImage", {
                {"Path", g.referenceImage.path},
                {"LoadOnStartup", g.referenceImage.loadOnStartup}
            }}
        };
    }
    
    inline void from_json(const nlohmann::json& j, GeneralSettingsParams& g) {
        g.debugImg = j.value("DebugImg", g.debugImg);
		g.enableRotation = j.value("EnableRotation", g.enableRotation);
        g.logLevel = j.value("LogLevel", g.logLevel);

        if (j.contains("ReferenceImage") && j.at("ReferenceImage").is_object()) {
            const auto& refImgJson = j.at("ReferenceImage");
            g.referenceImage.path = refImgJson.value("Path", g.referenceImage.path);
            g.referenceImage.loadOnStartup = refImgJson.value("LoadOnStartup", g.referenceImage.loadOnStartup);
        }
    }

    //-------------------------------------------------------------------------
    // Inspection 섹션 파라미터 구조체 (설정)
    // 모든 필드와 서브 섹션을 JSON에 명시된 그대로 출력합니다.
    //-------------------------------------------------------------------------
    struct InspectionParams {
        // 중첩된 구조체 정의 (멤버 변수로 인스턴스를 선언하지 않음)
        struct CommonParams {};
        struct GrayParams {
            RoiParams roi = { 0, 0, 14, 14 };
            double radius = 75.0;
            //double angleCenter = 0.39269908169872414; // CV_PI / 8.0
            //double angleOffset = 0.2617993877991494; // CV_PI / 12.0
            double angleCenter = 22.5; // CV_PI / 8.0
            double angleOffset = 15.0; // CV_PI / 12.0
        };
        struct ResParams {
			RoiParams roi = { 165, 175, 60, 70 };
            int drMin = 65;
            int drMax = 70;
            double minContourArea = 4.0;
            double targetDistance = 75.0;
            double distanceTolerance = 5.0;
            double angleToleranceDeg = 5.0;
        };
        struct GeoParams {
            RoiParams roi = { 60, 74, 400, 50 };
            int numSlices = 5;
            int maskOffsetY = 15;
            int maskHeight = 40;
            int drMin = 55;
            int drMax = 60;
            double minContourArea = 1000.0;
            double aspectRatio = 5.0;
        };

        struct EnvGeoParams {
            RoiParams roi = { 110, 0, 40, 50};
            int drMin = 55;
            int drMax = 70;
            double minContourArea = 10.0;
            double yTolerance = 5.0;
            double xMadConstant = 2.5;
            double targetYInterval = 30.0;
            double yIntervalTolerance = 5.0;
            bool enableXFilter = true;
            bool enableYFilter = true;
            double clusterDistTol = 5.0;
            double clusterXTol = 3.0;
            //double clusterYTol;
			double xTol = 2.0;
            double yInterval = 32.0;
            double yTol = 3.0;
        };
        
        struct AlignParams {
            RoiParams roi = { 0, 0, 14, 14 };
            double radius = 75.0;
            double angleCenter = 22.5; //0.39269908169872414; // CV_PI / 8.0
            double angleOffset = 15.0; //0.2617993877991494; // CV_PI / 12.0
        };

        // InspectionParams의 실제 멤버 변수들 (위에서 정의된 타입의 인스턴스)
        CommonParams common;
        GrayParams gray;
        ResParams res;
        GeoParams geo;
        EnvGeoParams envGeo;
        AlignParams align;
    };

    // InspectionParams 내부 구조체에 대한 to_json/from_json 함수들을 외부로 이동
    inline void to_json(nlohmann::json& j, const InspectionParams::CommonParams& c) { j = nlohmann::json::object(); }
    inline void from_json(const nlohmann::json& j, InspectionParams::CommonParams& c) { /* do nothing for empty struct */ }

    inline void to_json(nlohmann::json& j, const InspectionParams::GrayParams& p) {
        j = nlohmann::json{
            {"ROI", p.roi},
            {"radius", p.radius},
            {"angleCenter", p.angleCenter},
            {"angleOffset", p.angleOffset}
        };
    }

    inline void from_json(const nlohmann::json& j, InspectionParams::GrayParams& p) {
        if (j.contains("ROI") && j.at("ROI").is_object()) j.at("ROI").get_to(p.roi);
        p.radius = j.value("radius", p.radius);
        p.angleCenter = j.value("angleCenter", p.angleCenter);
        p.angleOffset = j.value("angleOffset", p.angleOffset);
    }

    inline void to_json(nlohmann::json& j, const InspectionParams::ResParams& p) {
        j = nlohmann::json{
            {"ROI", p.roi},
            {"DRMin", p.drMin},
            {"DRMax", p.drMax},
            {"MinContourArea", p.minContourArea},
            {"TargetDistance", p.targetDistance},
            {"DistanceTolerance", p.distanceTolerance},
            {"AngleToleranceDeg", p.angleToleranceDeg}
        };
    }

    inline void from_json(const nlohmann::json& j, InspectionParams::ResParams& p) {
        if (j.contains("ROI") && j.at("ROI").is_object()) j.at("ROI").get_to(p.roi);
        p.drMin = j.value("DRMin", p.drMin);
        p.drMax = j.value("DRMax", p.drMax);
        p.minContourArea = j.value("MinContourArea", p.minContourArea);
        p.targetDistance = j.value("TargetDistance", p.targetDistance);
        p.distanceTolerance = j.value("DistanceTolerance", p.distanceTolerance);
        p.angleToleranceDeg = j.value("AngleToleranceDeg", p.angleToleranceDeg);
    }

    inline void to_json(nlohmann::json& j, const InspectionParams::GeoParams& p) {
        j = nlohmann::json{
            {"ROI", p.roi},
            {"numSlices", p.numSlices},
            {"maskOffsetY", p.maskOffsetY},
            {"maskHeight", p.maskHeight},
            {"DRMin", p.drMin},
            {"DRMax", p.drMax},
            {"MinContourArea", p.minContourArea},
            {"aspectRatio", p.aspectRatio}
        };
    }

    inline void from_json(const nlohmann::json& j, InspectionParams::GeoParams& p) {
        if (j.contains("ROI") && j.at("ROI").is_object()) j.at("ROI").get_to(p.roi);
        p.numSlices = j.value("numSlices", p.numSlices);
        p.maskOffsetY = j.value("maskOffsetY", p.maskOffsetY);
        p.maskHeight = j.value("maskHeight", p.maskHeight);
        p.drMin = j.value("DRMin", p.drMin);
        p.drMax = j.value("DRMax", p.drMax);
        p.minContourArea = j.value("MinContourArea", p.minContourArea);
        p.aspectRatio = j.value("aspectRatio", p.aspectRatio);
    }

    // GeoParams 구조체에 대한 to_json 함수
    inline void to_json(nlohmann::json& j, const InspectionParams::EnvGeoParams& p) {
        j = nlohmann::json{
            {"ROI", p.roi},
            {"DRMin", p.drMin},
            {"DRMax", p.drMax},
            {"MinContourArea", p.minContourArea},
            {"Y_Tolerance", p.yTolerance},
            {"TargetYInterval", p.targetYInterval},
            {"YIntervalTolerance", p.yIntervalTolerance},
            {"X_MADConstant", p.xMadConstant},
            {"EnableXFilter", p.enableXFilter},
            {"clusterDistTol", p.clusterDistTol},
            {"clusterXTol", p.clusterXTol},
            {"xTol", p.xTol},
            {"yInterval", p.yInterval},
            {"yTol", p.yTol}
        };
    }

    // GeoParams 구조체에 대한 from_json 함수
    inline void from_json(const nlohmann::json& j, InspectionParams::EnvGeoParams& p) {
        if (j.contains("ROI") && j.at("ROI").is_object()) j.at("ROI").get_to(p.roi);
        p.drMin = j.value("DRMin", p.drMin);
        p.drMax = j.value("DRMax", p.drMax);
        p.minContourArea = j.value("MinContourArea", p.minContourArea);
        p.yTolerance = j.value("Y_Tolerance", p.yTolerance);
        p.targetYInterval = j.value("TargetYInterval", p.targetYInterval);
        p.yIntervalTolerance = j.value("YIntervalTolerance", p.yIntervalTolerance);
        p.xMadConstant = j.value("X_MADConstant", p.xMadConstant);
        p.enableXFilter = j.value("EnableXFilter", p.enableXFilter);
        p.enableYFilter = j.value("EnableYFilter", p.enableYFilter);
        p.clusterDistTol = j.value("clusterDistTol", p.clusterDistTol);
        p.clusterXTol = j.value("clusterXTol", p.clusterXTol);
        p.xTol = j.value("xTol", p.xTol);
        p.yInterval = j.value("yInterval", p.yInterval);
        p.yTol = j.value("yTol", p.yTol);
    }

    inline void to_json(nlohmann::json& j, const InspectionParams::AlignParams& p) {
        j = nlohmann::json{
            {"ROI", p.roi},
            {"radius", p.radius},
            {"angleCenter", p.angleCenter},
            {"angleOffset", p.angleOffset}
        };
    }
    inline void from_json(const nlohmann::json& j, InspectionParams::AlignParams& p) {
        if (j.contains("ROI") && j.at("ROI").is_object()) j.at("ROI").get_to(p.roi);
        p.radius = j.value("radius", p.radius);
        p.angleCenter = j.value("angleCenter", p.angleCenter);
        p.angleOffset = j.value("angleOffset", p.angleOffset);
    }


    inline void to_json(nlohmann::json& j, const InspectionParams& i) {
        j = nlohmann::json::object();
        j["Common"] = i.common;
        j["Gray"] = i.gray;
        j["Res"] = i.res;
        j["Geo"] = i.geo;
        j["EnvGeo"] = i.envGeo;
        j["Align"] = i.align;
    }
    inline void from_json(const nlohmann::json& j, InspectionParams& i) {
        // 각 멤버에 대해 contains와 is_object 체크 후 get_to 호출
        if (j.contains("Common") && j.at("Common").is_object()) j.at("Common").get_to(i.common);
        if (j.contains("Gray") && j.at("Gray").is_object()) j.at("Gray").get_to(i.gray);
        if (j.contains("Res") && j.at("Res").is_object()) j.at("Res").get_to(i.res);
        if (j.contains("Geo") && j.at("Geo").is_object()) j.at("Geo").get_to(i.geo);
        if (j.contains("EnvGeo") && j.at("EnvGeo").is_object()) j.at("EnvGeo").get_to(i.envGeo);
        if (j.contains("Align") && j.at("Align").is_object()) j.at("Align").get_to(i.align);
    }


    //-------------------------------------------------------------------------
    // ConfigManager 싱글톤 클래스
    // JSON 예시에 있는 GeneralSettings와 Inspection만 다룹니다.
    //-------------------------------------------------------------------------
    class ConfigManager {
    public:
        static ConfigManager& getInstance();

        void loadConfigFromJsonString(const std::string& jsonString);
        void loadConfigFromFile(const std::string& filePath);
        void saveConfigToFile(const std::string& filePath) const;

        bool isConfigLoaded() const { return m_isConfigLoaded; }

        const GeneralSettingsParams& getGeneralSettings() const { return m_generalSettings; }
        const InspectionParams& getInspectionParams() const { return m_inspectionParams; }

        // 설정 값을 변경할 수 있는 setter 함수 추가
        void setGeneralSettings(const GeneralSettingsParams& settings) {
            m_generalSettings = settings;
        }
        void setInspectionParams(const InspectionParams& params) {
            m_inspectionParams = params;
        }

    private:
        ConfigManager();
        ConfigManager(const ConfigManager&) = delete;
        ConfigManager& operator=(const ConfigManager&) = delete;

        void resetToDefaults();

        GeneralSettingsParams m_generalSettings;
        InspectionParams m_inspectionParams;

        bool m_isConfigLoaded;
    };

    void SetConfigJson(const char* jsonString); // 외부에서 JSON 문자열로 설정을 업데이트할 수 있는 편의 함수

} // namespace MyOpenCVWrapper