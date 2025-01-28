using UnityEngine;

[ExecuteInEditMode]
public class FullScreenWidget : MonoBehaviour
{

    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 pos = rt.anchoredPosition3D;
        rt.StrechRectTransformToFullScreen();
        rt.anchoredPosition3D = new Vector3(0, 0, pos.z);
    }
}
