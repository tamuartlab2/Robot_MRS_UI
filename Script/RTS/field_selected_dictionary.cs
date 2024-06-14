using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class field_selected_dictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();
    public gps_pivot gps_pivot;
    public OperationCMD operationCMD;
    public float dist_min = 100000000000f;
    public int corner_number;
    public Vector3 goal_min;
    public int[] robot_area_id;
    public float[] cost;

    private robot_ID_list robot_ID_List;
    private global_selection global_selection;
    private bool change_dictionary = false;
    private double[,] costMatrix, costMatrix_field_0, costMatrix_field_1;

    void Start()
    {
        robot_ID_List = GetComponent<robot_ID_list>();
        global_selection = GetComponent<global_selection>();

        robot_area_id = new int[256];
        cost = new float[256];

        //testHA();
    }

    void Update()
    {
        if (change_dictionary)
        {
            change_dictionary = false;
            robot_ID_List.blockID.Clear();
            robot_ID_List.blockID_1.Clear();
            foreach (KeyValuePair<int, GameObject> pair in selectedTable)
            {

                if (pair.Value != null)
                {
                    if (selectedTable[pair.Key].tag == "Field_0")
                    {
                        robot_ID_List.blockID.Add(selectedTable[pair.Key]);
                    }
                    else if(selectedTable[pair.Key].tag == "Field_1")
                    {
                        robot_ID_List.blockID_1.Add(selectedTable[pair.Key]);
                    }
                }
            }
        }

        if (global_selection.field_selection)
        {
            if (operationCMD.operationStart)
            {
                int numTask_0 = robot_ID_List.blockID.Count;
                int numTask_1 = robot_ID_List.blockID_1.Count;
                int numDrone_0 = robot_ID_List.robotID.Count;
                int numDrone_1 = robot_ID_List.UAV_ID.Count;

                if (numTask_0 > 0)
                {
                    costMatrix_field_0 = new double[numDrone_0, numTask_0];
                    for (int i = 0; i < numTask_0; i++)
                    {
                        for (int j = 0; j < numDrone_0; j++)
                        {
                            GameObject obj = robot_ID_List.robotID[j];
                            GameObject obj_block = robot_ID_List.blockID[i];
                            double distance = Vector3.Distance(obj.transform.position, obj_block.transform.position);
                            costMatrix_field_0[j, i] = distance;
                        }
                    }

                    //assign tasks with minimal total costs
                    var solver_0 = new HungarianAlgorithm(costMatrix_field_0);
                    int[] assignment_0 = solver_0.Run();

                    for (int j = 0; j < numDrone_0; j++)
                    {
                        if (assignment_0[j] < numTask_0)
                        {
                            GameObject obj = robot_ID_List.robotID[j];
                            if (obj.transform.Find("Goal Point") == null)
                            {
                                obj = obj.transform.parent.gameObject;
                            }
                            GameObject obj_block = robot_ID_List.blockID[assignment_0[j]];
                            double lat = obj_block.GetComponent<blockInfo>().gps_lat[0];
                            double lon = obj_block.GetComponent<blockInfo>().gps_lon[0];
                            Vector2 field_pose = gps_pivot.gpsToPoint(lat, lon);
                            obj.transform.Find("Goal Point").transform.position = new Vector3(field_pose.x, 0, field_pose.y);

                            if (obj.transform.Find("Aion_robot").GetComponent<Sim_Force_on_robot>() != null)
                            {
                                obj.transform.Find("Aion_robot").GetComponent<Sim_Force_on_robot>().enable_goal_point_force = true;

                            }
                            if (obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>() != null)
                            {
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().enable_goal_point_force = true;
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().enable_scout_mode = true;
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().gps_lat[0] = obj_block.GetComponent<blockInfo>().gps_lat[0];
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().gps_lon[0] = obj_block.GetComponent<blockInfo>().gps_lon[0];
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().gps_lat[1] = obj_block.GetComponent<blockInfo>().gps_lat[1];
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().gps_lon[1] = obj_block.GetComponent<blockInfo>().gps_lon[1];
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().gps_lat[2] = obj_block.GetComponent<blockInfo>().gps_lat[2];
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().gps_lon[2] = obj_block.GetComponent<blockInfo>().gps_lon[2];
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().row_number = obj_block.GetComponent<blockInfo>().row_number;
                                obj.transform.Find("Aion_robot").GetComponent<Force_on_robot>().row_width = obj_block.GetComponent<blockInfo>().row_width;

                            }
                        }
                    }
                }

                if (numTask_1 > 0)
                {
                    costMatrix_field_1 = new double[numDrone_1, numTask_1];
                    for (int i = 0; i < numTask_1; i++)
                    {
                        for (int j = 0; j < numDrone_1; j++)
                        {
                            GameObject obj = robot_ID_List.UAV_ID[j];
                            GameObject obj_block = robot_ID_List.blockID_1[i];
                            double distance = Vector3.Distance(obj.transform.position, obj_block.transform.position);
                            costMatrix_field_1[j, i] = distance;
                        }
                    }

                    //assign tasks with minimal total costs
                    var solver_1 = new HungarianAlgorithm(costMatrix_field_1);
                    int[] assignment_1 = solver_1.Run();
                    for (int j = 0; j < numDrone_1; j++)
                    {
                        if (assignment_1[j] < numTask_1)
                        {
                            GameObject obj = robot_ID_List.UAV_ID[j];
                            if (obj.transform.Find("Goal Point") == null)
                            {
                                obj = obj.transform.parent.gameObject;
                            }
                            GameObject obj_block = robot_ID_List.blockID_1[assignment_1[j]];
                            obj.transform.Find("Goal Point").transform.position = obj_block.transform.position;

                            if (obj.transform.Find("Aion_robot").GetComponent<Sim_Force_on_robot>() != null)
                            {
                                obj.transform.Find("Aion_robot").GetComponent<Sim_Force_on_robot>().enable_goal_point_force = true;
                            }
                        }
                    }
                }
            }
        }
    }


    public void testHA()
    {
        costMatrix = new double[,]
        {
            {10, 4, 5},
            {2, 10, 2},
            {9, 3, 17},
            {1, 1, 1}
        };
        var solver = new HungarianAlgorithm(costMatrix);
        int[] assignment = solver.Run();
        //print(assignment.Length);
        print(assignment[0]);
        print(assignment[1]);
        print(assignment[2]);
        print(assignment[3]);
    }

    public void addSelected(GameObject go)
    {
        int id = go.GetInstanceID();

        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, go);
            go.AddComponent<field_selection>();
            Debug.Log("Added " + id + " to selected dict");
        }
        else
        {
            deselect(id);
        }

        change_dictionary = true;
        //updateRobot();
    }

    public void deselect(int id)
    {
        Destroy(selectedTable[id].GetComponent<field_selection>());
        selectedTable.Remove(id);

        change_dictionary = true;
    }

    public void deselectAll()
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetComponent<field_selection>());
            }
        }
        selectedTable.Clear();

        change_dictionary = true;
    }

}
