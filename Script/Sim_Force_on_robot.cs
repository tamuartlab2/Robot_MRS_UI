using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sim_Force_on_robot : MonoBehaviour
{
    public bool enable_goal_point_force = false;
    public bool scout_mode = false;
    public bool enable_calibrate = false;
    public float goal_tolerance = 0.5f;      //m
    public bool publish_enable = false;
    public OperationCMD operationCMD;

    private float K_p = 2.0f, K_c = 1.0f;
    private float F_goal_max = 2.0f;
    //private float K_con = 1.5f;
    private float linear_speed = 0.0f, angular_speed = 0.0f;
    //private float maximum_linear_speed = 1.2f;    // m/s
    //private float maximum_angular_speed = 0.8f;    // rad/s

    private float period_time = 0.02f;
    private float t_now = 0.0f, t_last = 0.0f;
    private float distance_goal_now = 0.0f, distance_goal_last = 0.0f;
    private float goal_point_threshold = 0.2f;
    //private float distance2_now = 0.0f, distance2_last = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //operationCMD = GetComponent<OperationCMD>();
    }

    // Update is called once per frame
    void Update()
    {
        t_now = Time.time;
        float dt = t_now - t_last;
        if (dt > period_time && publish_enable)
        {
            t_last = t_now;

            if (enable_goal_point_force && operationCMD.operationStart)
            {
                
                Vector3 goal_position = transform.parent.Find("Goal Point").transform.position;
                distance_goal_now = Vector3.Distance(gameObject.transform.position, goal_position);

                if (distance_goal_now < goal_point_threshold)
                    enable_goal_point_force = false;

                float d_distance_goal = distance_goal_now - distance_goal_last;
                distance_goal_last = distance_goal_now;
                Vector2 F_goal = generateGoalPointForce(goal_position, d_distance_goal, distance_goal_now, dt);
                float F_x = F_goal.x;
                float F_y = F_goal.y;

                //linear_speed = linear_speed + F_x * dt;
                //if (linear_speed < 0.0f)
                //{
                //    linear_speed = 0.0f;
                //}
                //if (linear_speed == 0.0f)
                //{
                //    angular_speed = F_y * K_con;
                //}
                //else
                //{
                //    angular_speed = F_y / linear_speed;
                //}

                //if (linear_speed > maximum_linear_speed)
                //{
                //    linear_speed = maximum_linear_speed;
                //}


                //if (angular_speed > maximum_angular_speed)
                //{
                //    angular_speed = maximum_angular_speed;
                //}
                //else if (angular_speed < -maximum_angular_speed)
                //{
                //    angular_speed = -maximum_angular_speed;
                //}

                //GetComponent<Rigidbody>().angularVelocity = new Vector3(0, -angular_speed, 0);
                //transform.Translate(Vector3.forward * dt * linear_speed);

                float gamma = Mathf.Atan2(goal_position.z - gameObject.transform.position.z, goal_position.x - gameObject.transform.position.x);
                gamma = gamma * 180.0f / Mathf.PI;

                Quaternion rot_s = Quaternion.Euler(0, -gamma + 90, 0);
                linear_speed = F_x;
                transform.rotation = rot_s;
                transform.Translate(Vector3.forward * dt * linear_speed);
            }

            else
            {
                GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
                transform.Translate(Vector3.forward * 0);
            }

            if (enable_calibrate)
            {
                Quaternion rot = transform.parent.Find("CalibrationDirection").transform.rotation;
                Vector3 Cal_pose = transform.parent.Find("CalibrationDirection").transform.position;
                transform.position = new Vector3 (Cal_pose.x, transform.position.y, Cal_pose.z);
                transform.rotation = rot;
                enable_calibrate = false;
            }
        }
    }

    float wrapToPi(float angle)
    {
        float mappedAngle = Mathf.Repeat(angle + 180.0f, 360.0f) - 180.0f;
        if (mappedAngle > 180.0f)
        {
            mappedAngle -= 360.0f;
        }
        return mappedAngle;
    }

    Vector2 generateGoalPointForce(Vector3 G_position, float d_distance, float distance_now, float dt)
    {
        Vector2 Force;
        float F_k = K_p * distance_now;
        float F_c = K_c * d_distance / dt;
        float F_goal = F_k + F_c;

        if (F_goal > F_goal_max)
        {
            F_goal = F_goal_max;
        }
        else if (F_goal < -F_goal_max)
        {
            F_goal = -F_goal_max;
        }

        float alpha = -gameObject.transform.eulerAngles.y + 90.0f;
        alpha = wrapToPi(alpha);
        float gamma = Mathf.Atan2(G_position.z - gameObject.transform.position.z, G_position.x - gameObject.transform.position.x);
        gamma = gamma * 180.0f / Mathf.PI;
        float angle_of_Force = wrapToPi(alpha - gamma);

        Force.x = F_goal * Mathf.Cos(angle_of_Force * Mathf.PI / 180.0f);
        Force.y = -F_goal * Mathf.Sin(angle_of_Force * Mathf.PI / 180.0f);
        return Force;
    }
}
