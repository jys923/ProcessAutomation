@echo off
chcp 65001 > nul
setlocal

@REM ### 1. 중단할 프로세스 파일명들을 여기에 추가하세요 (공백으로 구분) ###
set "PROCESS_LIST=sonoCap.exe launcher.exe notepad.exe chrome.exe msedge.exe"

@REM ### 2. 실행할 프로그램의 파일명 또는 경로 ###
set "EXEC_PATH=.\SonoCap.MES.PreviewUI.exe"

echo 다음 프로세스들을 중단합니다: %PROCESS_LIST%

@REM ### 리스트에 있는 각 프로세스에 대해 종료 명령 실행 ###
for %%P in (%PROCESS_LIST%) do (
    echo.
    echo "%%P" 프로세스 중단 시도...
    taskkill /f /im "%%P" >nul 2>&1

    if %errorlevel% equ 0 (
        echo "%%P" 프로세스가 성공적으로 중단되었습니다.
    ) else (
        echo "%%P" 프로세스가 실행 중이지 않거나 중단에 실패했습니다.
    )
)

echo.
echo 모든 프로세스 중단 작업 완료.
@REM pause

@REM ### 3. 새 프로세스 실행 ###
echo "%EXEC_PATH%" 프로그램 실행...
start "" "%EXEC_PATH%"

if %errorlevel% equ 0 (
    echo "%EXEC_PATH%" 프로그램이 성공적으로 실행되었습니다.
) else (
    echo "%EXEC_PATH%" 프로그램 실행에 실패했습니다.
)

echo.
echo 작업 완료.
@REM pause