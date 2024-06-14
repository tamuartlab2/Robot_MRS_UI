using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selection_component : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //GetComponentInChildren<Renderer>().material.color = Color.green;
        //GetComponentInChildren<Renderer>().enabled = true;
        //the object is the Aion_robot
        transform.Find("Selection_Indicator").GetComponent<Renderer>().enabled = true;
        //if (GetComponent<Force_on_robot>() == null)
        //{
        //    if (GetComponent<Sim_Force_on_robot>().enable_goal_point_force)
        //    {
        //        transform.parent.Find("Goal Point").GetComponent<Renderer>().enabled = true;
        //        transform.parent.Find("LineRenderer").GetComponent<targetLine>().enable_line = true;
        //    }
        //}

        //else if (GetComponent<Force_on_robot>().enable_goal_point_force)
        //{
        //    transform.parent.Find("Goal Point").GetComponent<Renderer>().enabled = true;
        //    transform.parent.Find("LineRenderer").GetComponent<targetLine>().enable_line = true;
        //}
        transform.parent.Find("Goal Point").GetComponent<Renderer>().enabled = true;
        transform.parent.Find("LineRenderer").GetComponent<targetLine>().enable_line = true;
    }


    private void OnDestroy()
    {
        //GetComponentInChildren<Renderer>().material.color = Color.white;
        //GetComponentInChildren<Renderer>().enabled = false;
        transform.Find("Selection_Indicator").GetComponent<Renderer>().enabled = false;
        transform.parent.Find("Goal Point").GetComponent<Renderer>().enabled = false;
        transform.parent.Find("LineRenderer").GetComponent<targetLine>().enable_line = false;
    }
}
