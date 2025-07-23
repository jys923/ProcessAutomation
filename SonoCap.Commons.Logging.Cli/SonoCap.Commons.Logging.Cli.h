// SonoCap.Commons.Logging.Cli.h
#pragma once
#include <string>

// C#의 Serilog 네임스페이스 임포트 (Serilog이 설치된 C# 프로젝트를 참조했으므로 사용 가능)
using namespace Serilog;
using namespace System; // System::String, System::Object 등 .NET 기본 타입 사용
using namespace Serilog::Events; // LogEventLevel 사용을 위해 추가

namespace SonoCap // 기존 솔루션의 접두사 사용
{
    namespace Commons
    {
        namespace Logging // 로깅 기능을 위한 네임스페이스
        {
            // ref class는 .NET CLR에서 관리되는 클래스임을 의미합니다.
            public ref class Logger
            {
            private:
                // C#의 ILogger 인스턴스를 저장할 정적 멤버
                static ILogger^ _logger;

            public:
                // C# 코드에서 Serilog.Log.Logger 인스턴스를 넘겨받아 초기화하는 메서드
                static void Initialize(ILogger^ serilogLogger)
                {
                    _logger = serilogLogger;
                }

                // --- Serilog 로깅 메서드 래퍼 ---
                // 각 레벨별로 C# Serilog의 해당 메서드를 호출합니다.
                // messageTemplate은 Serilog의 메시지 템플릿 (예: "Value is {Property}")
                // propertyValues는 템플릿의 속성에 매핑될 값들
                // ... array<Object^>^ args는 가변 인수를 받는 .NET 방식입니다.

                static void Verbose(String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Verbose))
                        _logger->Verbose(messageTemplate, propertyValues);
                }

                static void Debug(String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Debug))
                        _logger->Debug(messageTemplate, propertyValues);
                }

                static void Information(String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Information))
                        _logger->Information(messageTemplate, propertyValues);
                }

                static void Warning(String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Warning))
                        _logger->Warning(messageTemplate, propertyValues);
                }

                static void Error(String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Error))
                        _logger->Error(messageTemplate, propertyValues);
                }

                // 예외를 포함하는 Error 로깅 (오버로드)
                static void Error(Exception^ exception, String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Error))
                        _logger->Error(exception, messageTemplate, propertyValues);
                }

                static void Fatal(String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Fatal))
                        _logger->Fatal(messageTemplate, propertyValues);
                }

                // 예외를 포함하는 Fatal 로깅 (오버로드)
                static void Fatal(Exception^ exception, String^ messageTemplate, ... array<Object^>^ propertyValues)
                {
                    if (_logger != nullptr && _logger->IsEnabled(LogEventLevel::Fatal))
                        _logger->Fatal(exception, messageTemplate, propertyValues);
                }
            };
        }
    }
}