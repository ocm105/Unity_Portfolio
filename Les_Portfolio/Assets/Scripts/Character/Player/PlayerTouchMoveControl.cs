using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerTouchMoveControl : MonoBehaviour
{
    [SerializeField] LayerMask layer;
    [SerializeField] float rayDistance = 50f;
    [SerializeField] LineRenderer linePath;
    [SerializeField] GameObject lastPath;
    [SerializeField] GameObject pathParent;
    private NavMeshAgent agent;

    private Touch touch;
    private Vector3 touchPos;

    private PointerEventData pointerEventData;
    private List<RaycastResult> pointerResults = new List<RaycastResult>();

    private PlayerInfo playerInfo;
    private bool isMove = false;

    private void Awake()
    {
        playerInfo = this.GetComponent<PlayerInfo>();
        agent = this.GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        agent.enabled = true;
        agent.ResetPath();
    }
    private void OnDisable()
    {
        agent.enabled = false;
        pathParent?.SetActive(false);
    }
    private void Init()
    {
        isMove = false;
    }

    private void SetMove()
    {
        Ray ray = playerInfo._mainCamera.ScreenPointToRay(touchPos);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, layer))
        {
            agent.SetDestination(new Vector3(hit.point.x, this.transform.position.y, hit.point.z));
        }
    }

    private void DrawPath()
    {
        // 경로 할당
        linePath.positionCount = agent.path.corners.Length;
        linePath.SetPositions(agent.path.corners);
        // 도착 지점 할당
        lastPath.transform.position = agent.path.corners.Last();
        pathParent.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
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
        if (Input.touchCount <= 0 || Input.touchCount > 1)
        {
            Init();
        }
        else
        {
            MobileTouchCheckMove();
        }
#endif
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            DrawPath();
            playerInfo._playerAniControl.SetMoveValue(1f);
        }
        else
        {
            playerInfo._playerAniControl.SetMoveValue(0f);
            pathParent.gameObject.SetActive(false);
        }
    }

    #region Check
    // Editor 용 Input 검사
    private void EditorInputCheckMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                isMove = true;
                touchPos = Input.mousePosition;
                SetMove();
            }
        }
        else if (Input.GetMouseButton(0) && isMove)
        {
            touchPos = Input.mousePosition;
            SetMove();
        }
    }
    // Mobile 용 Input 검사
    private void MobileTouchCheckMove()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:      // 손가락이 화면을 터치 시작.
                if (!IsPointerOverUIObject(Input.mousePosition))
                {
                    isMove = true;
                    touchPos = touch.position;
                    SetMove();
                }
                break;
            case TouchPhase.Moved:      // 화면에서 손가락이 움직임.
            case TouchPhase.Stationary: // 손가락이 화면을 터치하고 있지만 움직이지 않음.
                if (isMove)
                {
                    touchPos = touch.position;
                    SetMove();
                }
                break;
            case TouchPhase.Ended:      // 화면에서 손가락이 들어 올려짐 터치 끝.
            case TouchPhase.Canceled:   // 시스템이 터치 추적을 취소함.
            default:
                Init();
                break;
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
