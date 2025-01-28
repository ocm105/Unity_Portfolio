using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyObjects : MonoBehaviour
{
    private void Start()
    {

        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
