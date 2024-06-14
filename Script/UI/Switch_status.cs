//using System.Collections;
//using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch_status : MonoBehaviour
{
    
    public robot_ID_list robot_ID_List;
    public global_selection global_selection;
    public Text global_selection_button_text, global_text_button_text, global_enable_gps_button_text;
    public Text sd_enable_gps_button_text;



    void Start()
    {
        
    }

    public void OnSwitchModelButtonClick()
    {
        global_selection.field_selection = !global_selection.field_selection;
        global_selection.RTS_selection = !global_selection.field_selection;
        if (global_selection.field_selection) 
        {
            global_selection_button_text.text = "Swtich to Robot";
        }
        else
        {
            global_selection_button_text.text = "Swtich to Field";
        }
    }

    public void OnDisplayTextButtonClick()
    {
        //print("Clicked");
        bool current_display = true;
        foreach (GameObject robotObject in robot_ID_List.robotID)
        {
            GameObject obj;
            if (robotObject.transform.Find("Goal Point") == null)
            {
                obj = robotObject.transform.parent.gameObject;
            }
            else
            {
                obj = robotObject;
            }
            current_display = !obj.transform.Find("Aion_robot").transform.Find("Bot Text").GetComponent<MeshRenderer>().enabled;
            obj.transform.Find("Aion_robot").transform.Find("Bot Text").GetComponent<MeshRenderer>().enabled = current_display;
        }

        foreach (GameObject UAVObject in robot_ID_List.UAV_ID)
        {
            GameObject obj_uav;
            if (UAVObject.transform.Find("Goal Point") == null)
            {
                obj_uav = UAVObject.transform.parent.gameObject;
            }
            else
            {
                obj_uav = UAVObject;
            }
            obj_uav.transform.Find("Aion_robot").transform.Find("Bot Text").GetComponent<MeshRenderer>().enabled = current_display;
        }

        GameObject.Find("Instruction Text").GetComponent<MeshRenderer>().enabled = current_display;

        if (current_display)
        {
            global_text_button_text.text = "Hide Text";
        }
        else
        {
            global_text_button_text.text = "Show Text";
        }
    }


    public void OnGPSEnabledButtonClick()
    {
        bool current_display = true;

        foreach (GameObject robotObject in robot_ID_List.robotID)
        {
            if (robotObject.GetComponent<Force_on_robot>() != null)
            {
                current_display = !robotObject.GetComponent<Force_on_robot>().gps_enabled;
                robotObject.GetComponent<Force_on_robot>().gps_enabled = current_display;
            }
        }

        if (current_display)
        {
            global_enable_gps_button_text.text = "GPS Enabled";
        }
        else
        {
            global_enable_gps_button_text.text = "GPS Disabled";
        }
    }

    public void OnSpringDamperButtonClick()
    {
        bool current_display = true;
        foreach (GameObject robotObject in robot_ID_List.robotID)
        {
            if (robotObject.GetComponent<Force_on_robot>() != null)
            {
                current_display = !robotObject.GetComponent<Force_on_robot>().sd_enabled;
                robotObject.GetComponent<Force_on_robot>().sd_enabled = current_display;
            }
        }
        if (current_display)
        {
            sd_enable_gps_button_text.text = "SD On";
        }
        else
        {
            sd_enable_gps_button_text.text = "SD Off";
        }
    }

    public void OnStopClick()
    {
        foreach (GameObject robotObject in robot_ID_List.robotID)
        {
            if (robotObject.GetComponent<Force_on_robot>() != null)
            {
                robotObject.GetComponent<Force_on_robot>().enable_goal_point_force = false;
                robotObject.GetComponent<Force_on_robot>().publish_stop();
            }
        }
    }
}
