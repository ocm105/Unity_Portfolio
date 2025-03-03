using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UISystem
{
    public enum DongleGameType
    {
        Start,
        Game
    }
    public class DongleView : UIView
    {
        [SerializeField] LauncherControl launcherControl;
        [SerializeField] GameObject[] canvasGroups;

        [Header("Start")]
        [SerializeField] Button startButton;

        [Header("Game")]
        [SerializeField] Sprite[] dongleSprites;
        [SerializeField] Image nextDongleImage;
        [SerializeField] TextMeshProUGUI maxScoreText;      // 최고 점수
        [SerializeField] TextMeshProUGUI nowScoreText;      // 현재 점수
        private int nowScore = 0;
        private int maxScore = 0;

        [Header("Count")]
        [SerializeField] GameObject countObj;
        [SerializeField] Image countImg;
        [SerializeField] Sprite[] countSprites;

        [SerializeField] Button ExitButton;
        private DongleGameType dongleGameType = DongleGameType.Start;


        public void Show()
        {
            ShowLayer();
            SetInfo();
        }

        protected override void OnFirstShow()
        {
            startButton.onClick.AddListener(GameStart);
            ExitButton.onClick.AddListener(GameExit);
        }

        private void SetInfo()
        {
            GameInit();
            // try
            // {
            //     SoundManager.Instance.PlayBGMSound(BgmKey);
            // }
            // catch (System.Exception)
            // {
            //     SoundManager.Instance.SoundLoad(() =>
            //     {
            //         SoundManager.Instance.PlayBGMSound(BgmKey);
            //     });
            // }
            launcherControl.CreateDongle();
            // DongleGameStateChange(DongleGameType.Start);
        }

        private void GameInit()
        {
            DongleGameStateChange(DongleGameType.Start);
            GameDataInit();
            countObj.SetActive(false);
        }

        #region Default
        /// <summary> 게임 상태 변경 </summary>
        private void DongleGameStateChange(DongleGameType state)
        {
            bool isActive = false;
            dongleGameType = state;
            for (int i = 0; i < canvasGroups.Length; i++)
            {
                isActive = (int)state == i ? true : false;
                canvasGroups[i].SetActive(isActive);
            }
        }

        private void CountStart()
        {
            StartCoroutine(CountCoroutine());
        }
        private IEnumerator CountCoroutine()
        {
            countObj.SetActive(true);
            // SoundManager.Instance.PlaySFXSound("countDown");
            for (int i = countSprites.Length; i > 0; i--)
            {
                countImg.sprite = countSprites[i - 1];
                countImg.SetNativeSize();
                yield return new WaitForSeconds(1.15f);
            }
            countObj.SetActive(false);
        }

        /// <summary> 게임나가기 </summary>
        private void GameExit()
        {
            PopupState popupState = Les_UIManager.Instance.Popup<BasePopup_TwoBtn>().Open("게임을 나가시겠습니까?");
            popupState.OnYes = p => OkExitCallback();
        }

        private void OkExitCallback()
        {
            if (dongleGameType == DongleGameType.Start)
            {
                LoadingManager.Instance.SceneLoad(Constants.Scene.Title);
            }
            else
                GameInit();
        }
        #endregion

        #region Start
        /// <summary> 게임 시작 </summary>
        private void GameStart()
        {
            DongleGameStateChange(DongleGameType.Game);
            CountStart();
            launcherControl.LauncherControlStart();
        }
        #endregion

        #region Game
        /// <summary> 게임 데이터 초기화 </summary>
        public void GameDataInit()
        {
            NowScoreInit();
            GetMaxScore();
            launcherControl.Init();
        }

        /// <summary> 현재 점수 초기화 </summary>
        private void NowScoreInit()
        {
            nowScore = 0;
            nowScoreText.text = nowScore.ToString();
        }

        /// <summary> 현재 점수 갱신 </summary>
        public void NowScoreUpdate(int add)
        {
            nowScore += add;
            nowScoreText.text = nowScore.ToString();
        }

        /// <summary> 최고점수 불러오기 </summary>
        private void GetMaxScore()
        {
            // MinigameReqParam reqParam = new MinigameReqParam() { contsId = GameContentManager.Instance.MinigameMainResutRes.gameObj.contsId };
            // NetworkClient.Instance.GetMiniGame_Score(reqParam, (resData) =>
            //        {
            //            int temp = resData.topScore;
            //            if (temp == 0)
            //                maxScoreText.text = "-";
            //            else
            //            {
            //                maxScore = temp;   // 저장된 어딘가로 부터 받기
            //                maxScoreText.text = maxScore.ToString();
            //            }
            //        });
        }

        /// <summary> 다음 캔디 표시 </summary>
        public void ShowNextDongle(int idx)
        {
            nextDongleImage.sprite = dongleSprites[idx];
        }
        #endregion

        #region End
        public void GameEnd()
        {
            launcherControl.isEnd = true;

            // PopupState popupState = WV_UIMamager.Instance.Popup<GameResultPopup>().Open(nowScore);
            // popupState.OnClose = p => EndPopupCloseAction();
        }

        private void EndPopupCloseAction()
        {
            GameInit();
        }
        #endregion
    }
}
