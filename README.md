# Unity Portfolio
- Unity 버전 6000.0.33f1

# 보유 기술
- Google Play Games Login
- Addressable
- Google Admob
- Unity Ads
- Localization
- Camera Control
- Player Control
- Webview
- LoadingManager
- Network System
- Occlusion
  
# Google PlayGames Login
- 구글 플레이 게임즈 연동

![image](https://github.com/user-attachments/assets/7b8d2579-a3a9-487e-8d1a-0f00944d1a09)
![image](https://github.com/user-attachments/assets/6c37cb93-40be-4281-ba58-3284c56896d0)
![image](https://github.com/user-attachments/assets/07a57465-b53d-4105-82ca-b3be73777c4d)

# Addressable
- 사용되는 리소스를 Addressable에서 목적에 따라 Group화를 하여 빌드 후 
AWS3에 Bundle을 업로드하여 사용했습니다.
- 실행 시 AWS3 경로에서 다운로드 할 크기를 받아와 다운로드 할 것이 있으면 다운로드를 진행 합니다.
- 그 후 목적에 따라 Handle와 List를 만들어 저장하여 사용합니다.

![image](https://github.com/user-attachments/assets/d2a441fa-c365-4f63-9681-e1ab1a8ba6fe)
![image](https://github.com/user-attachments/assets/baf2d4f8-0aeb-424a-8efa-09dbdb8f3d8c)
![image](https://github.com/user-attachments/assets/6bb11b57-537a-4caf-bb31-5c9d5c0f44a9)
![image](https://github.com/user-attachments/assets/43328884-85d6-4198-b48d-c15201e7722f)
![image](https://github.com/user-attachments/assets/a2729f09-c8b4-46d6-8643-2fbe2ec41340)

# Google Admob
- Google Admob의 구성은 배너, 전면, 보상형, 리워드, 앱 열기로 5개로 구성하였습니다.

- 배너

![image](https://github.com/user-attachments/assets/ee28c9ff-2919-4c49-a1e3-58dd5d4daf1e)
![image](https://github.com/user-attachments/assets/9d0a6553-30b2-4cf0-a4aa-7b2c8ce3751e)

- 전면

![image](https://github.com/user-attachments/assets/f1cd733f-395f-4cce-9cc0-d431db438525)
![image](https://github.com/user-attachments/assets/7205de1a-31a8-44f1-892f-8be75d877059)

- 보상형

![image](https://github.com/user-attachments/assets/4c05a802-a80b-4872-bbcf-b40a778aa369)
![image](https://github.com/user-attachments/assets/f69bb36e-0d3f-4039-86e0-c416097c097c)

- 리워드

![image](https://github.com/user-attachments/assets/1f2dd208-7e64-4af9-9292-9c24ab223227)
![image](https://github.com/user-attachments/assets/f26d91e8-18c2-448d-a077-496a471f6984)

- 앱 열기

![image](https://github.com/user-attachments/assets/3fa6eebf-9efe-4ef5-87d2-c0fc0f093e4a)
![image](https://github.com/user-attachments/assets/5f5b1822-7c34-46cc-a266-3512e72505fe)

# Unity Ads
- Unity Ads의 구성은 배너, 전면, 보상형으로 3개로 구성하였습니다.

- 배너

![image](https://github.com/user-attachments/assets/fc2d79ed-6699-48c8-92ae-4d1d8235614f)
![image](https://github.com/user-attachments/assets/6f4b7141-5e57-44e6-9e3e-7c35875051b7)

- 전면

![image](https://github.com/user-attachments/assets/e8d787e2-9ea5-4ae5-9fd2-2504d502bd8f)
![image](https://github.com/user-attachments/assets/91b31810-6099-441b-9727-02904e250b5c)

- 보상형

![image](https://github.com/user-attachments/assets/3776cf5e-9d10-476e-93db-2bd1fbcfc6fb)
![image](https://github.com/user-attachments/assets/93f74708-3446-45d5-b77e-b2b2fa5285b9)

# Localization
- 로컬라이징은 기획자 및 여러사람과 협력을 위해 Google SpreadSheet로 작업하였습니다.
- 실행 시 Network Get를 Google SpreadSheet에서 CSVReader로 다운받아 GameDataManger스크립트에 저장하여 필요시 해당 Key로 호출하여 사용합니다.
- Google SpreadSheet URL : https://docs.google.com/spreadsheets/d/1XCdzsYQIEroE5NoNgPrv2Ek071ot36CVL6TUbKG_BNI/edit?usp=sharing

![image](https://github.com/user-attachments/assets/ba861948-b1ae-4290-8b79-15dd4053263c)
![image](https://github.com/user-attachments/assets/bc78f9bd-67fb-4569-b41e-b1bb4aba1fc7)
![image](https://github.com/user-attachments/assets/b3519175-de05-4785-b718-1c89a989e42e)
![image](https://github.com/user-attachments/assets/6c6fbdd3-7df6-4e47-a8f3-28918218ff05)

# Camera Control
- Camera의 타입은 총 3가지의 타입(FPS, Quarter, Shoulder)을 주었습니다.
- 기본적으로 Unity Cinemachine을 사용하여 구현하였습니다.

- Quarter에 사용된 Shader (Opacity)

![image](https://github.com/user-attachments/assets/8b1e3a55-1a05-49fa-afbb-b08e8c0a6382)

- FPS와 Shoulder에 사용된 Shader (Dither)

![image](https://github.com/user-attachments/assets/254fa2ca-4b15-46d2-b13d-ac5212199825)

- FPS와 Shoulder 스크롤 회전을 하다보니 UI 클릭 체크를 했습니다.

![image](https://github.com/user-attachments/assets/10b124ef-fae1-4ad0-8222-eb7c57094272)

- FPS 타입은 Player가 바라보는 View Target을 만들어 구현하였습니다.
- 영상URL : https://drive.google.com/file/d/1YSw87zzuWIRfqzYaPbK3uKkx6Vr8vjM_/view?usp=drive_link

![image](https://github.com/user-attachments/assets/1d9e47e2-b8a6-4a1b-8495-5267af073138)

- Quarter 타입은 Cinemachine의 Aim을 Player에 두고 Quarter View에서 바라보게 구현하고 Camera 부터 Player까지 RayCast를 쏴 해당된 Layer의 물체를 투명하게 하여 구현했습니다.
- 영상 URL : https://drive.google.com/file/d/1N7lmXPc71qX96cshd5QT8tfeogAzKNa_/view?usp=drive_link

![image](https://github.com/user-attachments/assets/92220632-cbfa-47b3-b772-3d84a5dd6ded)
![image](https://github.com/user-attachments/assets/d40a2359-95fd-4e06-8b75-d73a576e5b61)

- Shoulder 타입은 Cinemachine의 FreeLook을 사용하여 구현하였으며, Player가 겹칠 때 DitherShader를 사용하여 구현하였습니다.
- 영상 URL : https://drive.google.com/file/d/1Rb8_6ee7732insndRHAnjLXKxj8yjLNr/view?usp=drive_link

![image](https://github.com/user-attachments/assets/e7913a78-71aa-4157-ac2b-ef26b6eff5f3)

# Player Control
- Player Control은 Joystick 이동과 Touch 이동으로 2가지로 구성 했습니다.
- Joystick이동은 Joystick의 Horizontal과 Vertical 값으로 움직이며, 점프 체크로는 CharacterController의 CollisionFlags로 체크하고 있습니다.

![image](https://github.com/user-attachments/assets/098714d4-d60c-4d18-aaa3-eb7c8114af63)
![image](https://github.com/user-attachments/assets/1ba21f97-2928-46cb-aa5c-92abe4d822c9)

- Touch이동은 NavMeshAgent를 사용하여 구현했습니다.
- Touch 한곳에서 Raycast를 쏴 해당 Layer에 맞으면 그 위치를 NavMeshAgent에 도착지점으로 할당하여 이동하였으며, 이동 경로 표시는 LineRenderer를 사용하여 구현했습니다.

![image](https://github.com/user-attachments/assets/2618df3b-0209-4d64-ba62-929129cce0e3)
![image](https://github.com/user-attachments/assets/4dd8e0e7-5b3b-4523-bda2-d6cd1e39d2eb)

- 화면 터치 이동이다보니 UI 클릭 체크를 하였습니다.

![image](https://github.com/user-attachments/assets/10b124ef-fae1-4ad0-8222-eb7c57094272)

# Webview
- Webview는 gree/unity-webview를 사용하여 구현하였습니다.
- https://github.com/gree/unity-webview.git

![image](https://github.com/user-attachments/assets/b029cbb9-48ec-4a7a-a1d8-2bccf5994e90)
![image](https://github.com/user-attachments/assets/47fbea66-f9fc-4496-99f1-e74b28d3cd42)

# Scene Loading
- Fade Out => 비동기 씬로드 => Fade In
  
![Fade](https://github.com/user-attachments/assets/3f151b13-028b-4506-a446-594227b08c3f)
![화면 캡처 2024-11-22 081333](https://github.com/user-attachments/assets/53b839f7-c4c1-4b21-8359-eaa9b65c7e56)

# Occlusion

https://github.com/user-attachments/assets/59c8b487-5f37-473a-ac0f-c1bcee46d6c7

# Network
- Script의 가독성과 팀 작업의 효율을 위해 partial Class를 사용

![Network1](https://github.com/user-attachments/assets/a6e67da9-63e5-49b7-ac75-2c624298c127)
![Network2](https://github.com/user-attachments/assets/e2dbc1a6-a945-44ea-8a72-9c17229cb809)
![Network3](https://github.com/user-attachments/assets/d7bdf95c-eaef-4e4c-bb0f-7655d2d89315)

# GameDataManager
- 전체적으로 가져온 게임 데이터를 저장하여 사용하는 메니저를 만들어 사용

![화면 캡처 2024-11-22 092128](https://github.com/user-attachments/assets/a6a99a15-2918-49d6-9525-00608de3ec44)

# 기타 Data
- 기획자분들과 협업을 위해 Google SpreadSheet를 사용하였습니다.

- Descript (https://docs.google.com/spreadsheets/d/13vYZKF0P7r1ipcfow_eBzGAeFe0IoGscadxUe-_L8oU/edit?usp=sharing)
![data2](https://github.com/user-attachments/assets/4235af72-b5ac-4d07-bd4b-99d8fcd21918)

- Player (https://docs.google.com/spreadsheets/d/117WcavqmLLFPs3JXc53kY7xaCW4Z8mbMcyv3iAhh3bw/edit?usp=sharing)
![data4](https://github.com/user-attachments/assets/0b2b9053-d054-4366-accc-4744c5c7a434)
