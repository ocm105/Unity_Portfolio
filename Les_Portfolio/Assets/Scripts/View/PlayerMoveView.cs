using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveView : UIView
{
    [SerializeField] Joystick joystick;
    [SerializeField] Button jumpBtn;
    [SerializeField] Button[] moveBtns;
    [SerializeField] Button homeBtn;

    private PlayerInfo playerInfo;
    private PlayerMoveType playerMoveType;
    private CinemachineControl cinemachineControl;

    public void Show()
    {
        ShowLayer();
    }
    protected override void OnFirstShow()
    {
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        cinemachineControl = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<CinemachineControl>();

        jumpBtn.onClick.AddListener(() =>
        {
            playerInfo._playerMoveControl.Jump();
            SoundManager.Instance.PlaySFXSound("Button");
        });
        homeBtn.onClick.AddListener(OnClick_HomeBtn);

        for (int i = 0; i < moveBtns.Length; i++)
        {
            PlayerMoveType type = (PlayerMoveType)i;
            moveBtns[i].onClick.AddListener(() => ChangeMove(type));
        }
    }
    protected override void OnShow()
    {
        playerInfo._playerMoveControl.SetJoystick(joystick);
        Init();
    }

    private void Init()
    {
        playerMoveType = PlayerMoveType.Joystick;
        SetActiveButton(playerMoveType);
    }

    #region Function
    // 조작 버튼 활성화 / 비활성화
    private void SetActiveButton(PlayerMoveType type)
    {
        bool isOn = false;
        for (int i = 0; i < moveBtns.Length; i++)
        {
            isOn = (int)type == i ? false : true;
            moveBtns[i].interactable = isOn;
        }
        switch (type)
        {
            case PlayerMoveType.Joystick:
                joystick.gameObject.SetActive(true);
                jumpBtn.gameObject.SetActive(true);
                playerInfo._playerMoveControl.enabled = true;
                playerInfo._playerTouchMoveControl.enabled = false;
                break;
            case PlayerMoveType.Touch:
                joystick.gameObject.SetActive(false);
                jumpBtn.gameObject.SetActive(false);
                playerInfo._playerMoveControl.enabled = false;
                playerInfo._playerTouchMoveControl.enabled = true;
                break;
        }
        playerMoveType = type;
    }
    // 조작 변경
    private void ChangeMove(PlayerMoveType type)
    {
        SoundManager.Instance.PlaySFXSound("Button");
        SetActiveButton(type);

        // 변경 함수
        switch (type)
        {
            case PlayerMoveType.Joystick:
                cinemachineControl.OnChange_Cinemachine(CameraViewType.ShoulderView);
                break;
            case PlayerMoveType.Touch:
                cinemachineControl.OnChange_Cinemachine(CameraViewType.QuarterView);
                break;
        }
    }
    #endregion

    #region Event
    private void OnClick_HomeBtn()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.Title);
    }
    #endregion
}
