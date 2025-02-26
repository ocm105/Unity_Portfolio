using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UISystem
{
    public enum CandyGameType
    {
        Start,
        Game
    }
    public class CandyView : UIView
    {
        [SerializeField] FeroControl feroControl;
        [SerializeField] GameObject[] canvasGroups;

        [Header("Start")]
        [SerializeField] Button startButton;
        [SerializeField] Button rankingButton;
        [SerializeField] TextMeshProUGUI userCoin;

        [Header("Game")]
        [SerializeField] Sprite[] candySprites;
        [SerializeField] Image nextCandyImage;
        [SerializeField] TextMeshProUGUI[] rankingScoreTexts;   // 랭킹 점수들
        [SerializeField] TextMeshProUGUI maxScoreText;      // 최고 점수
        [SerializeField] TextMeshProUGUI nowScoreText;      // 현재 점수
        private int[] rankingScores;
        private int nowScore = 0;
        private int maxScore = 0;

        [Header("Count")]
        [SerializeField] GameObject countObj;
        [SerializeField] Image countImg;
        [SerializeField] Sprite[] countSprites;

        [SerializeField] Button ExitButton;
        private CandyGameType candyGameType = CandyGameType.Start;


        public void Show(object param)
        {
            ShowLayer();
            SetInfo();
        }

        protected override void OnFirstShow()
        {
            startButton.onClick.AddListener(CandyGameStart);
            rankingButton.onClick.AddListener(OpenRankingPopup);   // 랭킹 불러오기
            ExitButton.onClick.AddListener(CandyGameExit);
        }

        private void SetInfo()
        {
            CandyGameInit();
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
            feroControl.CreateCandy();
            // CandyGameStateChange(CandyGameType.Start);
        }

        private void CandyGameInit()
        {
            CandyGameStateChange(CandyGameType.Start);
            GetUserCoin();
            GameDataInit();
            countObj.SetActive(false);
        }

        #region Default
        /// <summary> 게임 상태 변경 </summary>
        private void CandyGameStateChange(CandyGameType state)
        {
            bool isActive = false;
            candyGameType = state;
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
        private void CandyGameExit()
        {
            // PopupState popupState = WV_UIMamager.Instance.Popup<CommonPopup>().Open(CommonPopupType.D, "candy_game_exit", ePopupType.i_quit);
            // popupState.OnYes = p => OkExitCallback();
        }

        private void OkExitCallback()
        {
            if (candyGameType == CandyGameType.Start)
            {
                // WV_UIMamager.Instance.SceneLoad(new LoadingDataParam()
                // {
                //     loadScene = Constants.Scene.Square,
                //     viewName = "SquareView",
                //     param = null
                // });
            }
            else
                CandyGameInit();
        }
        #endregion

        #region Start
        /// <summary> 유저 코인 불러오기 </summary>
        private void GetUserCoin()
        {
            // 유저 코인 가져오기
            // userCoin.text = GameContentManager.Instance.GetUserInfo().wittiPang.ToString("n0");
        }

        /// <summary> 캔디 게임 시작 </summary>
        private void CandyGameStart()
        {
            // UseWittiPangReqParam reqParam = new UseWittiPangReqParam()
            // {
            //     contsId = GameContentManager.Instance.MinigameMainResutRes.gameObj.contsId,
            //     rwrdTp = UseWittiPangType.GAME.ToString(),
            //     rwrdPnt = GameContentManager.Instance.MinigameMainResutRes.gameObj.rwrdPnt
            // };

            // NetworkClient.Instance.PostUseWittiPang(reqParam, okcall: () =>
            // {
            //     GetUserCoin();
            //     CandyGameStateChange(CandyGameType.Game);
            //     CountStart();
            //     feroControl.FeroControlStart();
            // },
            // nocall: () =>
            // {
            //     WV_UIMamager.Instance.Popup<CommonPopup>().Open(CommonPopupType.C, "playzone_wittipang", ePopupType.i_warning);
            // });
        }

        /// <summary> 캔디게임 랭킹 팝업 오픈 </summary>
        private void OpenRankingPopup()
        {
            // WV_UIMamager.Instance.Popup<RankPopup>().Open(PlayZoneGameType.CandyCandy);
        }
        #endregion

        #region Game
        /// <summary> 게임 데이터 초기화 </summary>
        public void GameDataInit()
        {
            NowScoreInit();
            GetMaxScore();
            GetRankingDate();
            feroControl.Init();
        }

        /// <summary> 랭킹 데이터 불러오기 </summary>
        private void GetRankingDate()
        {
            // MinigameReqParam req = new MinigameReqParam()
            // {
            //     contsId = GameContentManager.Instance.MinigameMainResutRes.gameObj.contsId,
            // };
            // NetworkClient.Instance.GetMiniGame_Ranking(req, (resData) =>
            // {
            //     rankingScores = new int[resData.Count];
            //     for (int i = 0; i < resData.Count; i++)
            //     {
            //         rankingScores[i] = resData[i].rankedScore;

            //         if (i < rankingScoreTexts.Length)
            //         {
            //             rankingScoreTexts[i].text = resData[i].rankedScore.ToString("n0");
            //         }
            //     }
            // });
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
        public void ShowNextCandy(int idx)
        {
            nextCandyImage.sprite = candySprites[idx];
        }
        #endregion

        #region End
        public void GameEnd()
        {
            feroControl.isEnd = true;
            int rank = GetMyRanking();

            // PopupState popupState = WV_UIMamager.Instance.Popup<GameResultPopup>().Open(nowScore);
            // popupState.OnClose = p => EndPopupCloseAction();
        }

        private int GetMyRanking()
        {
            int temp = 11;
            for (int i = rankingScores.Length; i > 0; i--)
            {
                if (rankingScores[i - 1] < nowScore)
                    temp--;
                else
                    break;
            }

            return temp;
        }

        private void EndPopupCloseAction()
        {
            CandyGameInit();
        }
        #endregion
    }
}
