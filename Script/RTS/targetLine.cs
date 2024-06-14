using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetLine : MonoBehaviour
{
    public bool enable_line = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        if (enable_line)
        {
            lineRenderer.SetPosition(0, transform.parent.Find("Aion_robot").transform.Find("Selection_Indicator").transform.position);
            lineRenderer.SetPosition(1, transform.parent.Find("Goal Point").transform.position);
        }
        else
        {
            lineRenderer.SetPosition(0, transform.parent.Find("Aion_robot").transform.Find("Selection_Indicator").transform.position);
            lineRenderer.SetPosition(1, transform.parent.Find("Aion_robot").transform.Find("Selection_Indicator").transform.position);
        }
    }
}
