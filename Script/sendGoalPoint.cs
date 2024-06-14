using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using UnityEngine.WSA;

public class sendGoalPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Aion_bot;
    public Force_on_robot FR;
    public float goal_tolerance = 0.5f;      //m
    public bool publish_enable = false;
    public float period_time = 0.5f;
    public bool end_flag = false;
    public int i = 0;
    public List<Vector3> Tasks = new List<Vector3>();

    Vector3 pos_temp;
    private int num_count = 0;
    private int goal_point_num = 5;
    private bool next_flag = false;
    private float t_now = 0.0f, t_last = 0.0f;
    

    void Start()
    {

        Tasks.Add(new Vector3 { x = 0.0f, z = 12.5f });
        Tasks.Add(new Vector3 { x = 2, z = 12.5f });
        Tasks.Add(new Vector3 { x = 2, z = -1.5f });
        Tasks.Add(new Vector3 { x = 4, z = -1.5f });
        Tasks.Add(new Vector3 { x = 4, z = 12.5f });
    }

    // Update is called once per frame
    void Update()
    {

        if (publish_enable)
        {
            FR.enable_goal_point_force = true;
            //FR.enable_obstacle_force = true;
        }
        else
        {
            FR.enable_goal_point_force = false;
            //FR.enable_obstacle_force = false;
        }

        t_now = Time.time;
        float dt = t_now - t_last;
        if (dt > period_time)
        {
            t_last = t_now;
            Vector3 current_goal_postion = transform.position;
            current_goal_postion.x = Tasks[i].x;
            current_goal_postion.z = Tasks[i].z;
            transform.position = current_goal_postion;
            if (i + 1 < goal_point_num)
            {
                if (next_flag)
                {
                    i++;
                    current_goal_postion = transform.position;
                    current_goal_postion.x = Tasks[i].x;
                    current_goal_postion.z = Tasks[i].z;
                    transform.position = current_goal_postion;
                    publish_enable = true;
                    next_flag = false;
                }
            }
            else
            {
                if (next_flag)
                {
                    end_flag = true;
                }
            }
            //print(i);

            double robot_lat = 0.0;
            double robot_long = 0.0;
            pos_temp = Aion_bot.transform.position;
            Vector3 position = transform.position;
            if (Mathf.Abs(pos_temp.x - position.x) <= goal_tolerance && Mathf.Abs(pos_temp.z - position.z) <= goal_tolerance)
            {
                //publish_enable = false;     //reach goal point
                robot_lat = 0;
                robot_long = 0;
            }
            else
            {
                robot_lat = 10.0;
                robot_long = 10.0;
            }

            if (robot_lat == 0 && robot_long == 0)
            {
                num_count++;
                if (num_count == 3)
                {
                    num_count = 0;
                    publish_enable = false;
                    next_flag = true;
                }
            }
        }
    }


    void init_script()
    {
        publish_enable = false;
        end_flag = false;
        i = 0;
        next_flag = false;
    }
}
