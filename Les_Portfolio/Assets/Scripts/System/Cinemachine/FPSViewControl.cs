using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class FPSViewControl : MonoBehaviour
{
    [SerializeField] float moveSpeedX = 0.1f;
    [SerializeField] float moveSpeedY = 0.1f;

    [SerializeField] GameObject fpsViewTarget;
    [SerializeField] GameObject player;


    float x, y;
    private Vector2 moveValue;
    private Vector2 prePos;
    private Vector2 nowPos;

    private PointerEventData pointerEventData;
    private List<RaycastResult> pointerResults = new List<RaycastResult>();

    private bool[] isMove = new bool[2];

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < isMove.Length; i++)
        {
            isMove[i] = false;
        }
    }

    private void SetMove()
    {
        x += moveValue.y * moveSpeedY;
        y -= moveValue.x * moveSpeedX;

        fpsViewTarget.transform.localRotation = Quaternion.Euler(Mathf.Clamp(x, -10f, 20f), 0, 0);
        player.transform.rotation = Quaternion.Euler(0, y, 0);
    }

    private void LateUpdate()
    {
        // 화면을 눌렀을 때 움직임 O (UI 검사)
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonUp(0))
        {
            Init();
        }
        else
        {
            EditorInputCheckMove();
        }
#elif UNITY_ANDROID
        if (Input.touchCount <= 0 || Input.touchCount > 2)
        {
            Init();
        }
        else
        {
            MobileTouchCheckMove();
        }
#endif
    }


    #region Check
    // Editor 용 Input 검사
    private void EditorInputCheckMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                isMove[0] = true;
                prePos = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButton(0) && isMove[0])
        {
            nowPos = Input.mousePosition;
            moveValue = prePos - nowPos;
            prePos = Input.mousePosition;
            SetMove();
        }
    }
    // Mobile 용 Input 검사
    private void MobileTouchCheckMove()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:      // 손가락이 화면을 터치 시작.
                    if (!IsPointerOverUIObject(touch.position))
                    {
                        isMove[i] = true;
                        prePos = touch.position - touch.deltaPosition;
                    }
                    break;
                case TouchPhase.Moved:      // 화면에서 손가락이 움직임.
                case TouchPhase.Stationary: // 손가락이 화면을 터치하고 있지만 움직이지 않음.
                    if (isMove[i])
                    {
                        nowPos = touch.position - touch.deltaPosition;
                        moveValue = prePos - nowPos;
                        prePos = touch.position - touch.deltaPosition;
                        SetMove();
                    }
                    break;
                case TouchPhase.Ended:      // 화면에서 손가락이 들어 올려짐 터치 끝.
                case TouchPhase.Canceled:   // 시스템이 터치 추적을 취소함.
                default:
                    if (isMove[i]) Init();
                    break;

            }
        }
    }

    /// <summary> UI Event가 들어왔는지 보는 함수 </summary>
    private bool IsPointerOverUIObject(Vector2 touchPos)
    {
        pointerResults.Clear();

        pointerEventData = new PointerEventData(EventSystem.current);

        pointerEventData.position = touchPos;

        EventSystem.current.RaycastAll(pointerEventData, pointerResults);

        return pointerResults.Count > 0;
    }
    #endregion
}

