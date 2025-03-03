using System.Collections;
using UISystem;
using UnityEngine;

public class DongleControl : MonoBehaviour
{
    private DongleView dongleView;
    private LauncherControl launcherControl;

    [SerializeField] DongleType dongleType;
    public DongleType _dongleType { get { return dongleType; } }
    private Animator animator;
    [SerializeField] int[] candyScores;

    [SerializeField] bool isMerge = false;
    public bool IsMerge { get { return isMerge; } }

    private float levelUpSpeed = 0f;
    private float maxFrame = 10;

    private void Awake()
    {
        dongleView = FindObjectOfType<DongleView>();
        launcherControl = FindObjectOfType<LauncherControl>();
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

    public void SetDongle(DongleType type)
    {
        Init();
        dongleType = type;
        animator.SetFloat("Level", (int)type);
    }
    public void DongleLevelUp()
    {
        isMerge = true;
        StartCoroutine(DongleLevelUpCoroutine());
    }
    private IEnumerator DongleLevelUpCoroutine()
    {
        int frame = 0;
        int level = (int)dongleType;
        dongleView.NowScoreUpdate(candyScores[level]);

        if (dongleType == DongleType.Eight)
        {
            dongleView.GameEnd();
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
            dongleType++;
            level += 1;

            animator.SetFloat("Level", level);
            Init();
        }
    }
    public void DongleDelete()
    {
        launcherControl.InDonglePool(this);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // 공이 부딧혔는지
        if (other.collider.CompareTag("Candy"))
        {
            DongleControl ccOther = other.collider.GetComponent<DongleControl>();
            // 같은 타입의 공이면서 둘다 합체를 안하고 있을 때
            if (ccOther._dongleType == dongleType && ccOther.IsMerge == false && isMerge == false)
            {
                if (ccOther.transform.GetSiblingIndex() < this.transform.GetSiblingIndex())
                {
                    isMerge = true;
                    // SoundManager.Instance.PlaySFXSound("merge2");

                    Vector2 one = ccOther.GetComponent<RectTransform>().anchoredPosition;
                    Vector2 two = this.GetComponent<RectTransform>().anchoredPosition;
                    launcherControl.ShowEffect(Vector2.Lerp(one, two, 0.5f));

                    ccOther.DongleLevelUp();
                    DongleDelete();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            dongleView.GameEnd();
        }
    }


}
