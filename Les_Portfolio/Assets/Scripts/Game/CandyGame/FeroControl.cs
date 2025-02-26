using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class FeroControl : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private const float MAX_LEFT = -295f;
    private const float MAX_RIGHT = 295f;
    private const float FIX_VALUE = 7.5f;
    [SerializeField] CandyView candyView;
    [SerializeField] RectTransform feroRect;

    [SerializeField] Transform candyCreatePos;          // 캔디 생성 위치
    [SerializeField] Transform candyPushPos;            // 캔디 넣는 위치
    [SerializeField] GameObject candyPrefab;
    [SerializeField] int[] candyPercents;
    [SerializeField] GameObject lineObject;

    [SerializeField] int poolCount = 50;
    [SerializeField] Vector2 originPos;

    private Queue<CandyControl> candyQueue = new Queue<CandyControl>();
    private CandyControl currentCandy;                    // 현재 들고 있는 캔디
    private int nextCandyIndex = -1;

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
        feroRect.anchoredPosition = originPos;
        isPointDown = false;
        isPush = false;
        nextCandyIndex = -1;
        lineObject.SetActive(false);
        isEnd = false;
        InitCandyPool();
    }
    public void FeroControlStart()
    {
        candyCreatePos.gameObject.SetActive(true);
        candyPushPos.gameObject.SetActive(true);
        OutCandyPool();
    }
    public void CreateCandy()
    {
        CreateCandyPool();
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
            StartCoroutine(PushCandy());
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
        feroRect.anchoredPosition = new Vector2(pos.x - fixWidth, feroRect.anchoredPosition.y);

        clampPos.x = Mathf.Clamp(feroRect.anchoredPosition.x, fixLeft, fixRight);
        clampPos.y = feroRect.anchoredPosition.y;
        feroRect.anchoredPosition = clampPos;
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            if (isEnd == true) return;
            if (isPush == false)
                StartCoroutine(PushCandy());
        }
    }
#else
#endif
    #endregion

    #region Function
    private int GetRandomCandy()
    {
        int add = 0, index = 0;
        int ran = Random.Range(1, 101);         // 100% 
        for (int i = 0; i < candyPercents.Length; i++)
        {
            add += candyPercents[i];
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

    private void CreateCandyPool()
    {
        for (int i = 0; i < poolCount; i++)
        {
            GameObject go = Instantiate(candyPrefab, candyCreatePos);
            go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            go.GetComponent<CircleCollider2D>().enabled = false;
            go.SetActive(false);
            candyQueue.Enqueue(go.GetComponent<CandyControl>());
        }
    }
    public void InitCandyPool()
    {
        GameObject[] objs = new GameObject[candyPushPos.childCount];

        for (int i = 0; i < candyPushPos.childCount; i++)
        {
            objs[i] = candyPushPos.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < objs.Length; i++)
        {
            CandyControl cc = objs[i].GetComponent<CandyControl>();
            InCandyPool(cc);
        }
        if (currentCandy != null)
        {
            InCandyPool(currentCandy);
        }
    }
    public void InCandyPool(CandyControl target)
    {
        target.transform.SetParent(candyCreatePos);
        target.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        target.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        target.GetComponent<CircleCollider2D>().enabled = false;
        candyQueue.Enqueue(target);
        target.gameObject.SetActive(false);
    }
    private void OutCandyPool()
    {
        currentCandy = candyQueue.Dequeue();
        currentCandy.gameObject.SetActive(true);

        if (nextCandyIndex == -1)
            currentCandy.SetCandy((BallType)GetRandomCandy());
        else
            currentCandy.SetCandy((BallType)nextCandyIndex);

        lineObject.SetActive(true);


        fixLeft = MAX_LEFT + (FIX_VALUE * (int)currentCandy._ballType);
        fixRight = MAX_RIGHT - (FIX_VALUE * (int)currentCandy._ballType);

        // 여기 다음 캔디 생성 -> cmd 에 함수 호출하여 넘기기
        nextCandyIndex = GetRandomCandy();
        candyView.ShowNextCandy(nextCandyIndex);
    }

    private IEnumerator PushCandy()
    {
        // SoundManager.Instance.PlaySFXSound("drop");
        isPush = true;
        endTrigger.SetActive(false);
        lineObject.SetActive(false);

        currentCandy.transform.SetParent(candyPushPos);
        currentCandy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        currentCandy.GetComponent<CircleCollider2D>().enabled = true;

        yield return new WaitForSeconds(1f);

        endTrigger.SetActive(true);

        OutCandyPool();
        isPush = false;

        yield break;
    }


}
