using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CinemachineControl : MonoBehaviour
{
    [Tooltip("FPSView, QuarterView, ShoulderView")]
    [SerializeField] GameObject[] views;
    [SerializeField] CameraRayAlpha cameraRayAlpha;

    public CameraViewType viewType { get; private set; }

    private void Start()
    {
        SetActiveCinemachine(CameraViewType.ShoulderView);
    }

    private void SetActiveCinemachine(CameraViewType type)
    {
        bool isActive = false;

        for (int i = 0; i < views.Length; i++)
        {
            isActive = (int)type == i ? true : false;
            views[i].SetActive(isActive);
        }

        if (type != CameraViewType.QuarterView) cameraRayAlpha.SetInit();

        viewType = type;
    }

    // Cinemachine 카메라 변경
    public void OnChange_Cinemachine(CameraViewType type)
    {
        LoadingManager.Instance.SetFadeOut(() =>
        {
            SetActiveCinemachine(type);
            LoadingManager.Instance.SetFadeIn();
        });
    }
}
