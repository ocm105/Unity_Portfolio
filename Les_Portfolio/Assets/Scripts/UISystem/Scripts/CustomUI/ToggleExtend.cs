using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
[RequireComponent(typeof(Toggle))]
public class ToggleExtend : MonoBehaviour
{
    [SerializeField] GameObject off_Obj, on_Obj;
       

    Toggle toggle;
    

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(delegate { OnValueChanged(); });
            SetState_ChildObject();
        }
    }

    public void SetState_ChildObject()
    {
        if (!gameObject.activeSelf)
            return;
        
        if(on_Obj != null)
            on_Obj.SetActive(toggle.isOn);

        if (off_Obj != null)
            off_Obj.SetActive(!toggle.isOn);
    }


    #region Event
    void OnValueChanged()
    {
        SetState_ChildObject();
    }
    #endregion

}
