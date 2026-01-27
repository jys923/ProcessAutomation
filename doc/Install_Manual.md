## 💡 설치 매뉴얼: 시스템 구성 요소 및 드라이버 설치 가이드

본 매뉴얼은 시스템 운영에 필요한 필수 구성 요소와 드라이버 설치 과정을 안내합니다. 원활한 프로그램 작동을 위해 아래 단계를 순서대로 진행해 주십시오.

---

### 1. 필수 구성 요소 및 드라이버 설치

제공된 설치 파일 중 다음과 같은 필수 구성 요소와 드라이버를 설치해야 합니다.

* **닷넷 런타임 (DotNet Runtime):** 응용 프로그램 실행에 필요한 Microsoft .NET 환경입니다.
* **Windows 데스크톱 런타임 (Windows Desktop Runtime):** 데스크톱 응용 프로그램 실행에 필요한 .NET 환경입니다.
* **CH34x 드라이버:** 특정 USB-직렬 변환 칩셋(CH340/CH341 등)을 사용하는 장치용 드라이버입니다.
* **CP210x 드라이버:** Silicon Labs의 CP210x USB-직렬 변환 칩셋을 사용하는 장치용 드라이버입니다.

#### 설치 단계:

1.  **`dotnet-runtime-8.0.15-win-x64.exe` 실행:**
    * `dotnet-runtime-8.0.15-win-x64.exe` 파일을 찾아 실행합니다.
    * 설치 마법사의 지시에 따라 설치를 완료합니다. 특별한 설정을 변경할 필요 없이 **"설치(Install)"** 버튼을 클릭하여 진행합니다.
2.  **`windowsdesktop-runtime-8.0.15-win-x64.exe` 실행:**
    * `windowsdesktop-runtime-8.0.15-win-x64.exe` 파일을 찾아 실행합니다.
    * 설치 마법사의 지시에 따라 설치를 완료합니다.
3.  **`CH34x_Install_Windows_v3_4.zip` 압축 해제 및 설치:**
    * `CH34x_Install_Windows_v3_4.zip` 파일을 마우스 오른쪽 버튼으로 클릭한 후 **"모두 압축 풀기(Extract All)"**를 선택하여 압축을 해제합니다.
    * 압축 해제된 폴더로 이동하여, 내부의 **설치 실행 파일(`.exe` 또는 `.msi`)을 찾아 실행**합니다. (일반적으로 `SETUP.exe` 또는 `INSTALL.exe`와 같은 이름입니다.)
    * 설치 마법사의 지시에 따라 드라이버 설치를 완료합니다.
4.  **`CP210x_Universal_Windows_Driver.zip` 압축 해제 및 드라이버 설치:**
    * `CP210x_Universal_Windows_Driver.zip` 파일을 마우스 오른쪽 버튼으로 클릭한 후 **"모두 압축 풀기(Extract All)"**를 선택하여 압축을 해제합니다.
    * 압축 해제된 폴더로 이동합니다. 이 드라이버는 별도의 설치 파일이 없으므로, **`silabser.inf` 파일을 마우스 오른쪽 버튼으로 클릭한 후 "설치(Install)"**를 선택하여 드라이버를 수동으로 설치합니다.
    ![inf_install][inf_install]

---

### 2. MariaDB 데이터베이스 설치 및 설정

데이터 저장을 위한 관계형 데이터베이스 MariaDB를 설치하고 초기 설정을 진행합니다.

#### 설치 단계:

1.  **`mariadb-11.7.2-winx64.msi` 실행:**
    * `mariadb-11.7.2-winx64.msi` 파일을 찾아 실행합니다.
    * 설치 마법사의 지시에 따라 진행합니다.
    ![db_install][db_install]
    * **"Set root password"** 단계에서 `root` 계정의 비밀번호를 **`Endolfin12!@`** 로 설정합니다.
    ![set_db_id_pw][set_db_id_pw]
    * **"Database features"** 또는 유사한 설정 단계에서 **"Use UTF8 as default server's character set"** 옵션이 있다면 **체크**하여 기본 문자셋을 UTF8로 설정합니다.
    * **"Remote access for 'root' user"** 또는 유사한 옵션이 있다면 **체크**하여 원격에서 root 계정으로 접속할 수 있도록 허용합니다.
    * 설치가 완료되면, 데이터베이스 `sonocap_mes`를 생성해야 합니다. 설치 마법사 내에 데이터베이스 생성 옵션이 있다면 해당 옵션을 활용하고, 없다면 설치 완료 후 별도의 SQL 클라이언트(예: HeidiSQL, DBeaver)를 사용하여 `CREATE DATABASE sonocap_mes;` 명령어를 실행하여 생성합니다.
    ![connect_db][connect_db]
    ![create_db_1][create_db_1]
    ![create_db_2][create_db_2]

---

### 3. SonoCap.MES.UI 프로그램 설치 및 초기 설정

메인 프로그램을 설치하고 데이터베이스 테이블 자동 생성 설정을 진행합니다.

#### 설치 단계:

1.  **`SonoCap.MES.UI_v1.*****.zip` 압축 해제:**
    * `SonoCap.MES.UI_v1.*****.zip` 파일을 마우스 오른쪽 버튼으로 클릭한 후 **"모두 압축 풀기(Extract All)"**를 선택하여 원하는 설치 경로에 압축을 해제합니다.
2.  **`appsettings.json` 파일 수정:**
    * 압축 해제된 폴더로 이동하여 `appsettings.json` 파일을 **텍스트 편집기(예: 메모장)**로 엽니다.
    * 파일 내에서 `"DbSettings"` 섹션을 찾아 다음과 같이 **`"AutoMigrate": true`** 로 값을 변경합니다.
        ```json
        "DbSettings": {
          "AutoMigrate": true
        },
        ```
    * 파일을 저장하고 닫습니다.
3.  **`SonoCap.MES.UI.exe` 실행 (테이블 자동 생성):**
    * `SonoCap.MES.UI.exe` 실행 파일을 찾아 실행합니다.
    * 프로그램이 시작되면, `AutoMigrate: true` 설정에 따라 MariaDB에 필요한 테이블이 **자동으로 생성**됩니다.
4.  **`appsettings.json` 파일 재수정 (설정 변경):**
    * 데이터베이스 테이블 생성이 완료된 후, 다시 `appsettings.json` 파일을 열어 `"DbSettings"` 섹션의 `"AutoMigrate"` 값을 **`false`** 로 변경합니다.
        ```json
        "DbSettings": {
          "AutoMigrate": false
        },
        ```
    * 파일을 저장하고 닫습니다.
5.  **`SonoCap.MES.UI.exe` 재시작:**
    * 변경된 설정을 적용하기 위해 `SonoCap.MES.UI.exe`를 **종료한 후 다시 실행**합니다. 이제 프로그램이 정상적으로 작동할 준비가 되었습니다.

---

**참고:** 모든 설치 과정에서 **관리자 권한**이 필요할 수 있습니다. 설치 파일 실행 시 "사용자 계정 컨트롤" 메시지가 나타나면 **"예(Yes)"**를 클릭하여 진행하십시오.

위 단계를 모두 완료하면 시스템이 프로그램 실행에 필요한 환경을 갖추게 되며, `SonoCap.MES.UI` 프로그램도 올바르게 설치 및 설정됩니다.


[set_db_id_pw]: ./images/set_db_id_pw.png "set_db_id_pw"
[db_install]: ./images/db_install.png "db_install"
[connect_db]: ./images/connect_db.png "connect_db"
[create_db_1]: ./images/create_db_1.png "create_db_1"
[create_db_2]: ./images/create_db_2.png "create_db_2"
[inf_install]: ./images/inf_install.png "inf_install"