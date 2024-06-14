using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selected_dictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();

    private bool change_dictionary = false;

    private robot_ID_list robot_ID_List;
    private global_selection global_selection;

    void Start()
    {
        robot_ID_List = GetComponent<robot_ID_list>();
        global_selection = GetComponent<global_selection>();
    }

    void Update()
    {
        if (change_dictionary)
        {
            change_dictionary = false;
            robot_ID_List.robotID.Clear();
            robot_ID_List.UAV_ID.Clear();
            foreach (KeyValuePair<int, GameObject> pair in selectedTable)
            {

                if (pair.Value != null)
                {
                    if (selectedTable[pair.Key].tag == "drone")     //Aion
                    {
                        robot_ID_List.robotID.Add(selectedTable[pair.Key]);
                    }
                    else if (selectedTable[pair.Key].tag == "drone_1")  //UAV
                    {
                        robot_ID_List.UAV_ID.Add(selectedTable[pair.Key]);
                    }
                }
            }
        }
    }

    public void addSelected(GameObject go)
    {
        int id = go.GetInstanceID();

        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, go);
            go.AddComponent<selection_component>();
            Debug.Log("Added " + id + " to selected dict");
        }
        else
        {
            deselect(id);
        }
        change_dictionary = true;
    }

    public void deselect(int id)
    {
        Destroy(selectedTable[id].GetComponent<selection_component>());
        selectedTable.Remove(id);
        change_dictionary = true;
    }

    public void deselectAll()
    {
        foreach(KeyValuePair<int,GameObject> pair in selectedTable)
        {
            if(pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetComponent<selection_component>());
            }
        }
        selectedTable.Clear();
        change_dictionary = true;
    }

    public void addGoalPoint(Vector3 goal_pose)
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable[pair.Key].AddComponent<goal_point_update>();
                selectedTable[pair.Key].transform.parent.Find("Goal Point").transform.position = goal_pose;
            }
        }
    }

    public void stop_robot()
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetComponent<goal_point_update>());
            }
        }
    }

    public void addCalPoint(Vector3 cal_pose)
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable[pair.Key].AddComponent<cal_pose_update>();
                cal_pose.y += 0.5f;
                selectedTable[pair.Key].transform.parent.Find("CalibrationDirection").transform.position = cal_pose;
            }
        }
    }

    public void addCalDirection(float angle)
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable[pair.Key].transform.parent.Find("CalibrationDirection").transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
    }

    public void deselectCalPoint()
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetComponent<cal_pose_update>());
            }
        }
    }
}
