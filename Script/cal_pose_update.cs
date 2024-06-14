using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cal_pose_update : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.parent.Find("CalibrationDirection").GetComponent<Renderer>().enabled = true;
    }


    private void OnDestroy()
    {
        transform.parent.Find("CalibrationDirection").transform.Rotate(0, -90f, 0);
        if (GetComponent<Force_on_robot>() == null)
        {
            GetComponent<Sim_Force_on_robot>().enable_calibrate = true;
        }
        else
        {
            GetComponent<Force_on_robot>().enable_calibrate = true;
        }
        transform.parent.Find("CalibrationDirection").GetComponent<Renderer>().enabled = false;
    }
}
