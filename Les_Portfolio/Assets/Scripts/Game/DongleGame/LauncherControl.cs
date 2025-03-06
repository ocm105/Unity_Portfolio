using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class LauncherControl : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private const float MAX_LEFT = -315f;
    private const float MAX_RIGHT = 315f;
    private const float FIX_VALUE = 19f;
    [SerializeField] DongleView dongleView;
    [SerializeField] RectTransform launcherRect;

    [SerializeField] Transform dongleCreatePos;          // 동글 생성 위치
    [SerializeField] Transform donglePushPos;            // 동글 넣는 위치
    [SerializeField] GameObject donglePrefab;
    [SerializeField] int[] donglePercents;
    [SerializeField] GameObject lineObject;

    [SerializeField] int poolCount = 50;
    [SerializeField] Vector2 originPos;

    private Queue<DongleControl> dongleQueue = new Queue<DongleControl>();
    private DongleControl currentDongle;                    // 현재 들고 있는 동글
    private int nextDongleIndex = -1;

    private float fixWidth = 0f;
    private float fixLeft = 0;
    private float fixRight = 0;


    private Vector2 clampPos;
    private bool isPointDown = false;
    private bool isPush = false;

    [Header("Effect")]
    [SerializeField] int sfxPoolCount = 50;
    [SerializeField] GameObject mergeEffect;
    [SerializeField] Transform effectParent;
    private Queue<ParticleSystem> effectQueue = new Queue<ParticleSystem>();

    [SerializeField] GameObject endTrigger;
    public bool isEnd { get; set; }

    public void Init()
    {
        fixWidth = Screen.width / 2;
        launcherRect.anchoredPosition = originPos;
        isPointDown = false;
        isPush = false;
        nextDongleIndex = -1;
        lineObject.SetActive(false);
        isEnd = false;
        InitDonglePool();
    }
    public void LauncherControlStart()
    {
        dongleCreatePos.gameObject.SetActive(true);
        donglePushPos.gameObject.SetActive(true);
        OutDonglePool();
    }
    public void CreateDongle()
    {
        CreateDonglePool();
        CreateEffectPool();
    }

    #region Handler
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isEnd == true) return;
        if (isPush == true) return;             // 캔디 푸쉬중이면 리턴

        isPointDown = true;

        Move(eventData.position);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isEnd == true) return;

        isPointDown = false;
        if (isPush == false)
            StartCoroutine(PushDongle());
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isEnd == true) return;
        if (isPointDown == false) return;       // 클릭이 안됬으면 리턴
        if (isPush == true) return;             // 캔디 푸쉬중이면 리턴

        Move(eventData.position);
    }
    private void Move(Vector2 pos)
    {
        launcherRect.anchoredPosition = new Vector2(pos.x - fixWidth, launcherRect.anchoredPosition.y);

        clampPos.x = Mathf.Clamp(launcherRect.anchoredPosition.x, fixLeft, fixRight);
        clampPos.y = launcherRect.anchoredPosition.y;
        launcherRect.anchoredPosition = clampPos;
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            // if (isEnd == true) return;
            if (isPush == false)
                StartCoroutine(PushDongle());
        }
    }
#else
#endif
    #endregion

    #region Function
    private int GetRandomDongle()
    {
        int add = 0, index = 0;
        int ran = Random.Range(1, 101);         // 100% 
        for (int i = 0; i < donglePercents.Length; i++)
        {
            add += donglePercents[i];
            if (add >= ran)
            {
                index = i;
                break;
            }
        }

        return index;
    }
    #endregion

    #region Effect
    private void CreateEffectPool()
    {
        for (int i = 0; i < sfxPoolCount; i++)
        {
            GameObject go = Instantiate(mergeEffect, effectParent);
            effectQueue.Enqueue(go.GetComponent<ParticleSystem>());
            go.SetActive(false);
        }
    }

    public void ShowEffect(Vector2 pos)
    {
        StartCoroutine(ShowEffectCoroutine(pos));
    }
    private IEnumerator ShowEffectCoroutine(Vector2 pos)
    {
        ParticleSystem particle = effectQueue.Dequeue();
        // particle.transform.position = pos;
        particle.GetComponent<RectTransform>().anchoredPosition = pos;
        particle.gameObject.SetActive(true);

        yield return new WaitForSeconds(particle.main.duration);

        particle.gameObject.SetActive(false);
        effectQueue.Enqueue(particle);
    }
    #endregion

    private void CreateDonglePool()
    {
        for (int i = 0; i < poolCount; i++)
        {
            GameObject go = Instantiate(donglePrefab, dongleCreatePos);
            go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            go.GetComponent<CircleCollider2D>().enabled = false;
            go.SetActive(false);
            dongleQueue.Enqueue(go.GetComponent<DongleControl>());
        }
    }
    public void InitDonglePool()
    {
        GameObject[] objs = new GameObject[donglePushPos.childCount];

        for (int i = 0; i < donglePushPos.childCount; i++)
        {
            objs[i] = donglePushPos.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < objs.Length; i++)
        {
            DongleControl cc = objs[i].GetComponent<DongleControl>();
            InDonglePool(cc);
        }
        if (currentDongle != null)
        {
            InDonglePool(currentDongle);
        }
    }
    public void InDonglePool(DongleControl target)
    {
        target.transform.SetParent(dongleCreatePos);
        target.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        target.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        target.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
        target.GetComponent<CircleCollider2D>().enabled = false;
        dongleQueue.Enqueue(target);
        target.gameObject.SetActive(false);
    }
    private void OutDonglePool()
    {
        currentDongle = dongleQueue.Dequeue();
        currentDongle.gameObject.SetActive(true);

        if (nextDongleIndex == -1)
            currentDongle.SetDongle((DongleType)GetRandomDongle());
        else
            currentDongle.SetDongle((DongleType)nextDongleIndex);

        lineObject.SetActive(true);


        fixLeft = MAX_LEFT + (FIX_VALUE * (int)currentDongle._dongleType);
        fixRight = MAX_RIGHT - (FIX_VALUE * (int)currentDongle._dongleType);

        // 여기 다음 캔디 생성 -> cmd 에 함수 호출하여 넘기기
        nextDongleIndex = GetRandomDongle();
        dongleView.ShowNextDongle(nextDongleIndex);
    }

    private IEnumerator PushDongle()
    {
        // SoundManager.Instance.PlaySFXSound("drop");
        isPush = true;
        endTrigger.SetActive(false);
        lineObject.SetActive(false);

        currentDongle.transform.SetParent(donglePushPos);
        currentDongle.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        currentDongle.GetComponent<CircleCollider2D>().enabled = true;

        yield return new WaitForSeconds(1f);

        endTrigger.SetActive(true);

        OutDonglePool();
        isPush = false;

        yield break;
    }


}
