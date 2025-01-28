# Les_Portfolio

# Local Save 
- PlayerPrefs로 저장하는 Local 데이터를 Base64로 변환하여 저장

![LocalBase64(3)](https://github.com/user-attachments/assets/817b5fe6-762b-495f-a1e7-8ccb238ab5dd)
![LocalBase64(2)](https://github.com/user-attachments/assets/d463babc-b31d-4872-9574-0354068958ae)
![LocalBase64](https://github.com/user-attachments/assets/d89bbe66-8741-42aa-8077-2aeebd43df49)
![LocalBase64(4)](https://github.com/user-attachments/assets/5f0bcc04-ef06-4c0e-a40c-7aaf99ba142a)

# Scene Loading
- Fade Out => 비동기 씬로드 => Fade In
  
![Fade](https://github.com/user-attachments/assets/3f151b13-028b-4506-a446-594227b08c3f)
![화면 캡처 2024-11-22 081333](https://github.com/user-attachments/assets/53b839f7-c4c1-4b21-8359-eaa9b65c7e56)

# Occlusion

https://github.com/user-attachments/assets/59c8b487-5f37-473a-ac0f-c1bcee46d6c7

# Cinemachine 카메라 제어

https://github.com/user-attachments/assets/0edbb98e-ed2e-463c-aae4-3eebff2a37e7

# Network
- Script의 가독성과 팀 작업의 효율을 위해 partial Class를 사용

![Network1](https://github.com/user-attachments/assets/a6e67da9-63e5-49b7-ac75-2c624298c127)
![Network2](https://github.com/user-attachments/assets/e2dbc1a6-a945-44ea-8a72-9c17229cb809)
![Network3](https://github.com/user-attachments/assets/d7bdf95c-eaef-4e4c-bb0f-7655d2d89315)

# Localization
- Google SpreadSheet와 연동
- Google SpreadSheet URL
- (https://docs.google.com/spreadsheets/d/1XCdzsYQIEroE5NoNgPrv2Ek071ot36CVL6TUbKG_BNI/edit?usp=sharing)

![화면 캡처 2024-11-22 083013](https://github.com/user-attachments/assets/9e202f8a-4af6-4f29-9c2b-0a60f31b90be)

# Addressable
- AWS3 bucket에 Addressable data를 올리고 Title 화면에서 다운로드 후 사용

![add3](https://github.com/user-attachments/assets/b6fe0447-06c3-48ce-bd92-556ab3a28b6a)
![add2](https://github.com/user-attachments/assets/2b3810ad-e72e-42ee-af2c-e9205c1c873c)
![add1](https://github.com/user-attachments/assets/b196d3d1-5f6a-4e40-b02a-c4ea0e256ef7)

# GameDataManager
- 전체적으로 가져온 게임 데이터를 저장하여 사용하는 메니저를 만들어 사용

![화면 캡처 2024-11-22 092128](https://github.com/user-attachments/assets/a6a99a15-2918-49d6-9525-00608de3ec44)

# Dungeon Script
- 기본적으로 Enum으로 State를 만든 후 제어

![화면 캡처 2024-11-22 085958](https://github.com/user-attachments/assets/5a90d610-603e-44e5-9c03-466fef017ede)

- 재활용 되는 Moster는 Queue로 Pooling하여 죽으면 다시 Queue로 들어오고 Queqe에 남아있는 Moster가 없으면 추가하는 식으로 짰습니다.
  
![화면 캡처 2024-11-22 090153](https://github.com/user-attachments/assets/a7e1e150-903e-46cd-b6a2-96bfe48fb8bf)

- 데미지 주는 방식은 Interface를 사용하였습니다.

![화면 캡처 2024-11-22 090658](https://github.com/user-attachments/assets/e48752b5-3418-4f71-8733-fe07f389ea7e)
![11](https://github.com/user-attachments/assets/be2f030b-2bfd-4582-a2df-2eb21b1822ae)
![22](https://github.com/user-attachments/assets/b91507c7-7544-412c-9ce8-7957dd9f0be0)

# Monster
- Monster는 NavmeshAgent로 이동 및 제어하며, MonsterBase Script를 상속하여 사용합니다.

![화면 캡처 2024-11-22 093403](https://github.com/user-attachments/assets/9ccfa055-0efa-4417-8864-67c32b7bd08c)
![base](https://github.com/user-attachments/assets/a50ebb53-4280-40e8-9403-839541623d1a)


# 기타 Data
- 기획자분들과 협업을 위해 Google SpreadSheet를 사용하였습니다.

- Dungoen (https://docs.google.com/spreadsheets/d/1f3pklhv_iEpukXS6HCqoPPomcdSBPq9hVEsxbhUijU4/edit?usp=sharing)
![data1](https://github.com/user-attachments/assets/b309c87c-9a00-4d32-bf26-c9423a694309)

- Descript (https://docs.google.com/spreadsheets/d/13vYZKF0P7r1ipcfow_eBzGAeFe0IoGscadxUe-_L8oU/edit?usp=sharing)
![data2](https://github.com/user-attachments/assets/4235af72-b5ac-4d07-bd4b-99d8fcd21918)

- Monster (https://docs.google.com/spreadsheets/d/1QaGvz4XmbMwFnUNE6BBvY7X6pPfzUdiaPcD45Em6UlU/edit?usp=sharing)
![data3](https://github.com/user-attachments/assets/6667a2f3-f8a0-4aab-80b6-675f6da5e504)

- Player (https://docs.google.com/spreadsheets/d/117WcavqmLLFPs3JXc53kY7xaCW4Z8mbMcyv3iAhh3bw/edit?usp=sharing)
![data4](https://github.com/user-attachments/assets/0b2b9053-d054-4366-accc-4744c5c7a434)
