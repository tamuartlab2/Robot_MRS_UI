using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class field_selection : MonoBehaviour
{
    void Start()
    {
        Color c = GetComponent<MeshRenderer>().material.color;
        c.a = 1.0f;
        GetComponent<Renderer>().material.color = c;
    }

    private void OnDestroy()
    {
        //GetComponent<Renderer>().enabled = false;
        Color c = GetComponent<MeshRenderer>().material.color;
        c.a = 0.3f;
        GetComponent<Renderer>().material.color = c;
    }
}
