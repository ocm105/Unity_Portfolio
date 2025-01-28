using System.Collections;
using UnityEngine;

namespace UISystem
{
    public class Les_UIManager : UIManager
    {
        public static new Les_UIManager Instance
        {
            get
            {
                if (UIManager.Instance == null)
                    UIManager.Instance = FindObjectOfType<UIManager>();

                return UIManager.Instance as Les_UIManager;
            }
        }
        public static object ViewParam { get { return viewParam; } }
    }
}
