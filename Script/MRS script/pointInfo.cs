using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Color c = GetComponent<MeshRenderer>().material.color;
        c.a = 0.3f;
        GetComponent<Renderer>().material.color = c;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
