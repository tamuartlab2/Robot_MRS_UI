using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goal_point_update : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //GetComponentInChildren<Renderer>().material.color = Color.green;
        //GetComponentInChildren<Renderer>().enabled = true;
        //the object is the Aion_robot
        transform.Find("Selection_Indicator").GetComponent<Renderer>().material.color = Color.green;
        //GetComponent<Force_on_robot>().enable_obstacle_force = true;
        if (!(GetComponent<Sim_Force_on_robot>() == null))
        {
            GetComponent<Sim_Force_on_robot>().enable_goal_point_force = true;
        }
        else if (!(GetComponent<Force_on_robot>() == null))
        {
            GetComponent<Force_on_robot>().send_stop = false;
            GetComponent<Force_on_robot>().enable_goal_point_force = true;
        }
        transform.parent.Find("Goal Point").GetComponent<Renderer>().enabled = true;
        transform.parent.Find("LineRenderer").GetComponent<targetLine>().enable_line = true;    //draw the line between the robot and the goal point
    }


    private void OnDestroy()
    {
        //GetComponentInChildren<Renderer>().material.color = Color.white;
        //GetComponentInChildren<Renderer>().enabled = false;
        //GetComponent<Force_on_robot>().enable_obstacle_force = false;
        if (!(GetComponent<Sim_Force_on_robot>() == null))
        {
            GetComponent<Sim_Force_on_robot>().enable_goal_point_force = false;
        }
        else if (!(GetComponent<Force_on_robot>() == null))
        {
            GetComponent<Force_on_robot>().enable_goal_point_force = false;
            GetComponent<Force_on_robot>().send_stop = true;
        }
        transform.Find("Selection_Indicator").GetComponent<Renderer>().material.color = Color.red;
        transform.parent.Find("Goal Point").GetComponent<Renderer>().enabled = false;
        transform.parent.Find("LineRenderer").GetComponent<targetLine>().enable_line = false;
    }
}
