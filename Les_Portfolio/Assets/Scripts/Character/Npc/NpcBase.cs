using UnityEngine;

public class NpcBase : MonoBehaviour
{
    public int npcDialogIndex;
    public GameObject chatIcon;

    protected virtual void Start()
    {
        chatIcon.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chatIcon.SetActive(true);
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chatIcon.SetActive(true);
        }
    }
}
