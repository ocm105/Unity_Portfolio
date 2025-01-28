using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class CameraView : UIView
{
    [SerializeField] Joystick joystick;
    [SerializeField] Button[] viewBtns;
    [SerializeField] Button homeBtn;

    private PlayerInfo playerInfo;
    private CinemachineControl cinemachineControl;
    private CameraViewType cameraViewType;

    public void Show()
    {
        ShowLayer();
    }
    protected override void OnFirstShow()
    {
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        cinemachineControl = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<CinemachineControl>();

        for (int i = 0; i < viewBtns.Length; i++)
        {
            CameraViewType type = (CameraViewType)i;
            viewBtns[i].onClick.AddListener(() => ChangeCinemachine(type));
        }
        homeBtn.onClick.AddListener(OnClick_HomeBtn);
    }
    protected override void OnShow()
    {
        playerInfo._playerMoveControl.SetJoystick(joystick);
        Init();
    }

    private void Init()
    {
        cameraViewType = CameraViewType.ShoulderView;
        SetActiveButton(cameraViewType);
    }

    #region Function
    // 카메라 버튼 활성화 / 비활성화
    private void SetActiveButton(CameraViewType type)
    {
        bool isOn = false;
        for (int i = 0; i < viewBtns.Length; i++)
        {
            isOn = (int)type == i ? false : true;
            viewBtns[i].interactable = isOn;
        }
        cameraViewType = type;
    }
    // 카메라 Cinemachine View 변경
    private void ChangeCinemachine(CameraViewType type)
    {
        SetActiveButton(type);
        cinemachineControl.OnChange_Cinemachine(type);
    }
    #endregion

    private void OnClick_HomeBtn()
    {
        LoadingManager.Instance.SceneLoad(Constants.Scene.Title);
    }
}
