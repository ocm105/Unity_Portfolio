using System.Collections;
using UISystem;
using UnityEngine;

public class CandyControl : MonoBehaviour
{
    private FeroControl feroControl;
    private CandyView candyView;

    [SerializeField] BallType ballType;
    public BallType _ballType { get { return ballType; } }
    private Animator animator;
    [SerializeField] int[] candyScores;

    [SerializeField] bool isMerge = false;
    public bool IsMerge { get { return isMerge; } }

    private float levelUpSpeed = 0f;
    private float maxFrame = 10;

    private void Awake()
    {
        candyView = FindObjectOfType<CandyView>();
        feroControl = FindObjectOfType<FeroControl>();
        animator = this.GetComponent<Animator>();
    }
    private void Start()
    {
        levelUpSpeed = 1 / maxFrame;
        Init();
    }
    private void Init()
    {
        isMerge = false;
    }

    public void SetCandy(BallType type)
    {
        Init();
        ballType = type;
        animator.SetFloat("Level", (int)type);
    }
    public void CandyLevelUp()
    {
        isMerge = true;
        StartCoroutine(CandyLevelUpCoroutine());
    }
    private IEnumerator CandyLevelUpCoroutine()
    {
        int frame = 0;
        int level = (int)ballType;
        candyView.NowScoreUpdate(candyScores[level]);

        if (ballType == BallType.Ten)
        {
            candyView.GameEnd();
            yield break;
        }
        else
        {
            while (frame < maxFrame)
            {
                animator.SetFloat("Level", Mathf.Lerp(level, level + 1, levelUpSpeed * frame));
                yield return null;
                frame++;
            }
            ballType++;
            level += 1;

            animator.SetFloat("Level", level);
            Init();
        }
    }
    public void CandyDelete()
    {
        feroControl.InCandyPool(this);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // 공이 부딧혔는지
        if (other.collider.CompareTag("Candy"))
        {
            CandyControl ccOther = other.collider.GetComponent<CandyControl>();
            // 같은 타입의 공이면서 둘다 합체를 안하고 있을 때
            if (ccOther._ballType == ballType && ccOther.IsMerge == false && isMerge == false)
            {
                if (ccOther.transform.GetSiblingIndex() < this.transform.GetSiblingIndex())
                {
                    isMerge = true;
                    // SoundManager.Instance.PlaySFXSound("merge2");

                    Vector2 one = ccOther.GetComponent<RectTransform>().anchoredPosition;
                    Vector2 two = this.GetComponent<RectTransform>().anchoredPosition;
                    feroControl.ShowEffect(Vector2.Lerp(one, two, 0.5f));

                    ccOther.CandyLevelUp();
                    CandyDelete();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            candyView.GameEnd();
        }
    }


}
