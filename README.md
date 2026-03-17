# portfolio-mmorpg-unity
Project name : Unity Client project for Testing MMORPG Server 
Overview : 
고성능 액션 MMORPG Server 연결 테스트를 위한 클라이언트 프로젝트입니다.

#Prerequisites
Server 프로젝트를 받지 않으셨다면 먼저 clone해주세요.
git clone https://github.com/milrow/portfolio-mmoserver.git

서버git 경로에서 .\Protocol\GenProto.bat 실행
생성된 Generated_CS를 .\Assets\Scripts에 붙여넣기

## Environment
- OS: Windows 10/11 
- IDE: Visual Studio 2022 권장
- Unity Version: 6000.3.2f1(Unity 6.3 LTS)

#Directory Structure
├───Assets
│   ├───Material
│   ├───Plugins
│   ├───Prefabs
│   ├───Scenes
│   ├───Scripts
│   ├───Settings
│   └───Unity.VisualScripting.Generated
├───Packages       
└───ProjectSetting
    