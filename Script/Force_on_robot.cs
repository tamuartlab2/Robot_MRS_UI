using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RosMessageTypes.Sensor;
//using RosMessageTypes.Nav;
//using RosMessageTypes.Geometry;
//using RosMessageTypes.Std;
//using RosMessageTypes.BuiltinInterfaces;
////using Unity.Robotics.Core;
//using Unity.Robotics.ROSTCPConnector;
//using static UnityEditor.PlayerSettings;
//using static UnityEditor.PlayerSettings;
//using UnityEngine.UIElements;
//using UnityEditor.Localization.Editor;
//using UnityEditor;
using RosSharp.RosBridgeClient;
//using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
//using RosSharp.RosBridgeClient.MessageTypes.Std;

public class Force_on_robot : MonoBehaviour
{
    //public GameObject ROS_connect_obj;
    //public ROSConnection m_Ros;
    public RosConnector m_Ros;
    //public string robot_ID_test = "";
    //public string robot_ID = "/Robot0/";
    public string est_gps_topic = "Localization/GPS";
    public string goal_point_topic = "goal_point";
    public string calibration_topic = "Localization/Calibrate/Odom";
    public string calibration_received_topic = "Localization/Calibrate/received";
    public string reach_goal_topic = "loca_and_nav/reach_goal";
    public string mode_pub_topic = "auto_mode";
    public string gps_enabled_topic = "Localization/enable_gps";
    public string sd_enabled_topic = "force/spring_damper_enabled";
    public float goal_tolerance = 0.5f;      //m
    public bool publish_enable = false;
    //public bool end_flag = false;
    //public bool enable_R_force = false;
    //public bool enable_obstacle_force = false;
    public bool enable_goal_point_force = false;
    public bool enable_calibrate = false;
    public bool enable_scout_mode = false;
    public bool send_stop = false;
    public bool gps_enabled = true, sd_enabled = false;
    public float period_time = 1.0f;
    public OperationCMD operationCMD;
    public double[] gps_lat, gps_lon;
    public double row_number, row_width;

    Vector3 pos_temp;
    Vector3 GoalPoint_pose;
    Vector3 Cal_pose;
    Vector3 vehicle_pose;
    //bool cal_delay = false;
    private bool change_pose = false;
    private bool reach_goal_enter = false;
    private float heading_angle = 0f;
    private double local_gps_lu_lat;
    private double local_gps_lb_lat;
    private double local_gps_lb_long;
    private double local_gps_rb_long;
    private Vector3 pivot_position;
    private Vector3 rb_position;
    private Vector3 lu_position;
    private double x_denom;
    private double z_denom;
    private float t_now = 0.0f, t_last = 0.0f;
    private float time_1, time_2;
    private double robot_lon_last = 0, robot_lat_last = 0;
    private bool gps_enabled_last, sd_enabled_last, stop_sent = false;

    private string GoalPointPubId;
    private string CalibrationPubId;
    private string ModePubID, GPS_EnabledID, SD_EnabledID;

    void Start()
    {
        //GObj = GetComponent<GameObject>();
        //Aion = GetComponent<Aion_Motion>();
        //m_Ros = ROSConnection.GetOrCreateInstance();

        //est_gps_topic = robot_ID + est_gps_topic;
        //goal_point_topic = robot_ID + goal_point_topic;
        //calibration_topic= robot_ID + calibration_topic;
        //calibration_received_topic = robot_ID + calibration_received_topic;
        //reach_goal_topic = robot_ID + reach_goal_topic;
        ////m_Ros = ROS_connect_obj.GetComponent<ROSConnection>();
        //m_Ros.Subscribe<NavSatFixMsg>(est_gps_topic, loca_update);
        //m_Ros.Subscribe<BoolMsg>(calibration_received_topic, calibration_received_update);
        //m_Ros.Subscribe<BoolMsg>(reach_goal_topic, reach_goal_update);
        //m_Ros.RegisterPublisher<NavSatFixMsg>(goal_point_topic);
        //m_Ros.RegisterPublisher<OdometryMsg>(calibration_topic);

        GameObject lb_pivot = GameObject.Find("bottom_left_pivot");
        pivot_position = lb_pivot.transform.position;
        GameObject rb_pivot = GameObject.Find("bottom_right_pivot");
        rb_position = rb_pivot.transform.position;
        GameObject lu_pivot = GameObject.Find("up_left_pivot");
        lu_position = lu_pivot.transform.position;

        x_denom = pivot_position.x - lu_position.x;
        z_denom = rb_position.z - pivot_position.z;

        if (!m_Ros.IsConnected.WaitOne(1 * 1000))
            Debug.LogWarning("Failed to subscribe: RosConnector not connected");

        m_Ros.RosSocket.Subscribe<NavSatFix_ArtLab>(est_gps_topic, loca_update, (int)(0.1 * 1000.0)); // the rate(in ms in between messages) at which to throttle the topics
        m_Ros.RosSocket.Subscribe<RosSharp.RosBridgeClient.MessageTypes.Std.Bool>(calibration_received_topic, calibration_received_update, (int)(0.1 * 1000.0));
        m_Ros.RosSocket.Subscribe<RosSharp.RosBridgeClient.MessageTypes.Std.Bool>(reach_goal_topic, reach_goal_update, (int)(0.1 * 1000.0));

        ModePubID = m_Ros.RosSocket.Advertise<NavSatFix_ArtLab>(mode_pub_topic);
        GPS_EnabledID = m_Ros.RosSocket.Advertise<RosSharp.RosBridgeClient.MessageTypes.Std.Bool>(gps_enabled_topic);
        SD_EnabledID = m_Ros.RosSocket.Advertise<RosSharp.RosBridgeClient.MessageTypes.Std.Bool>(sd_enabled_topic);
        GoalPointPubId = m_Ros.RosSocket.Advertise<NavSatFix_ArtLab>(goal_point_topic);
        CalibrationPubId = m_Ros.RosSocket.Advertise<Odometry_ArtLab>(calibration_topic);
        //operationCMD = GetComponent<OperationCMD>();

        gps_enabled_last = gps_enabled;
        gps_lat = new double[4];
        gps_lon = new double[4];
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(30f, 0, 50f);

        //if (!m_Ros.IsConnected.WaitOne(1 * 1000))
        //{
        //    Debug.LogWarning("Failed to subscribe: RosConnector not connected");
        //    return;
        //}

        //print(Lidar.F_obstacle_x);
        //loca_update_test();
  
        //if (cal_delay)
        //{
        //    if (!enable_calibrate)
        //    {
        //        time_2 = Time.time;
        //        print(time_2 - time_1);
        //        cal_delay = false;
        //    }
        //}

        if (change_pose)
        {
            change_pose = false;
            transform.position = vehicle_pose;
            transform.rotation = Quaternion.Euler(0f, -heading_angle, 0f);
            //transform.Find("Bot Text").transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }

        t_now = Time.time;
        float dt = t_now - t_last;
        if (dt > period_time && publish_enable)
        {
            t_last = t_now;
            double robot_lat = 0.0;
            double robot_long = 0.0;

            if (operationCMD.operationStart)
            {
                stop_sent = false;
                if (enable_scout_mode)
                {
                    if (reach_goal_enter)
                    {
                        enable_goal_point_force = false;

                        //send scouting cmd here
                        double[] cmd_list = new double[9];
                        cmd_list[0] = 1;
                        cmd_list[1] = gps_lat[1];
                        cmd_list[2] = gps_lon[1];
                        cmd_list[3] = gps_lat[2];
                        cmd_list[4] = gps_lon[2];
                        cmd_list[5] = row_width;
                        cmd_list[6] = row_number;
                        var mode_msg = new NavSatFix_ArtLab
                        {
                            latitude = gps_lat[0],
                            longitude = gps_lon[0],
                            position_covariance = cmd_list,
                        };
                        m_Ros.RosSocket.Publish(ModePubID, mode_msg);
                        reach_goal_enter = false;
                    }
                }

                if (enable_goal_point_force)
                {
                    send_stop = false;
                    GoalPoint_pose = transform.parent.Find("Goal Point").transform.position;
                    Vector3 position = GoalPoint_pose;

                    local_gps_lu_lat = gps_pivot.gps_lu_latitude_static;
                    local_gps_lb_lat = gps_pivot.gps_lb_latitude_static;
                    local_gps_lb_long = gps_pivot.gps_lb_longitude_static;
                    local_gps_rb_long = gps_pivot.gps_rb_longitude_static;

                    double del_z = position.z - pivot_position.z;
                    double z_ratio = del_z / z_denom;

                    if (z_ratio > 1) z_ratio = 1;
                    else if (z_ratio < 0) z_ratio = 0;
                    double del_long = z_ratio * (local_gps_rb_long - local_gps_lb_long);
                    robot_long = del_long + local_gps_lb_long;

                    double del_x = pivot_position.x - position.x;
                    double x_ratio = del_x / x_denom;
                    if (x_ratio > 1) x_ratio = 1;
                    else if (x_ratio < 0) x_ratio = 0;

                    double del_lat = x_ratio * (local_gps_lu_lat - local_gps_lb_lat);
                    robot_lat = del_lat + local_gps_lb_lat;

                    var msg = new NavSatFix_ArtLab
                    {
                        //header = new RosSharp.RosBridgeClient.MessageTypes.Std.Header()
                        //{
                        //    frame_id = "map",
                        //},

                        latitude = robot_lat,
                        longitude = robot_long,
                        //latitude = 30.62269,
                        //longitude = -96.334283,
                    };

                    if (robot_lat_last != robot_lat || robot_lon_last != robot_long)
                    {
                        robot_lat_last = robot_lat;
                        robot_lon_last = robot_long;
                        double[] cmd_list = new double[9];
                        cmd_list[0] = 0;        //goal point mode
                        var mode_msg = new NavSatFix_ArtLab
                        {
                            position_covariance = cmd_list,
                        };
                        m_Ros.RosSocket.Publish(ModePubID, mode_msg);
                        m_Ros.RosSocket.Publish(GoalPointPubId, msg);
                    }
                }
                else if (send_stop)
                {

                    publish_stop();
                }
            }
            else
            {
                if (!stop_sent)
                {
                    publish_stop();
                    stop_sent = true;
                }
            }

            if (enable_calibrate)
            {
                Quaternion rot = transform.parent.Find("CalibrationDirection").transform.rotation;
                rot = TransformUtil.Unity2Ros(rot);
                Cal_pose = transform.parent.Find("CalibrationDirection").transform.position;
                Vector3 position = Cal_pose;

                local_gps_lu_lat = gps_pivot.gps_lu_latitude_static;
                local_gps_lb_lat = gps_pivot.gps_lb_latitude_static;
                local_gps_lb_long = gps_pivot.gps_lb_longitude_static;
                local_gps_rb_long = gps_pivot.gps_rb_longitude_static;

                double del_z = position.z - pivot_position.z;
                double z_ratio = del_z / z_denom;

                if (z_ratio > 1) z_ratio = 1;
                else if (z_ratio < 0) z_ratio = 0;
                double del_long = z_ratio * (local_gps_rb_long - local_gps_lb_long);
                double cal_long = del_long + local_gps_lb_long;

                double del_x = pivot_position.x - position.x;
                double x_ratio = del_x / x_denom;
                if (x_ratio > 1) x_ratio = 1;
                else if (x_ratio < 0) x_ratio = 0;

                double del_lat = x_ratio * (local_gps_lu_lat - local_gps_lb_lat);
                double cal_lat = del_lat + local_gps_lb_lat;

                var cal_msg = new Odometry_ArtLab
                {

                    pose = new RosSharp.RosBridgeClient.MessageTypes.Geometry.PoseWithCovariance
                    {
                        pose = new RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose
                        {
                            position = new RosSharp.RosBridgeClient.MessageTypes.Geometry.Point
                            {
                                //x = robot_position.z,
                                //y = robot_position.x,
                                x = cal_long,
                                y = cal_lat,
                            },
                            orientation = new RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion
                            {
                                x = rot.x,
                                y = rot.y,
                                z = rot.z,
                                w = rot.w,
                            },
                        }
                    },

                };
                m_Ros.RosSocket.Publish(CalibrationPubId, cal_msg);
                //time_1 = Time.time;
                //cal_delay = true;
            }
            
            if (gps_enabled != gps_enabled_last)
            {
                gps_enabled_last = gps_enabled;
                var gps_enabled_msg = new RosSharp.RosBridgeClient.MessageTypes.Std.Bool
                {
                    data = gps_enabled,
                };
                m_Ros.RosSocket.Publish(GPS_EnabledID, gps_enabled_msg);
            }

            if (sd_enabled != sd_enabled_last)
            {
                sd_enabled_last = sd_enabled;
                var sd_enabled_msg = new RosSharp.RosBridgeClient.MessageTypes.Std.Bool
                {
                    data = sd_enabled,
                };
                m_Ros.RosSocket.Publish(SD_EnabledID, sd_enabled_msg);
            }
        }
    }

    public void publish_stop()
    {
        var msg = new NavSatFix_ArtLab
        {
            latitude = 0,
            longitude = 0,
        };
        robot_lat_last = 0;
        robot_lon_last = 0;
        double[] cmd_list = new double[9];
        cmd_list[0] = 0;        //goal point mode
        var mode_msg = new NavSatFix_ArtLab
        {
            position_covariance = cmd_list,
        };
        m_Ros.RosSocket.Publish(ModePubID, mode_msg);
        m_Ros.RosSocket.Publish(GoalPointPubId, msg);
    }

    private float wrapToPi(float angle)
    {
        float mappedAngle = Mathf.Repeat(angle + 180.0f, 360.0f) - 180.0f;
        if (mappedAngle > 180.0f)
        {
            mappedAngle -= 360.0f;
        }
        return mappedAngle;
    }

    private void loca_update(NavSatFix_ArtLab msg)
    {
        change_pose = true;
        local_gps_lu_lat = gps_pivot.gps_lu_latitude_static;
        local_gps_lb_lat = gps_pivot.gps_lb_latitude_static;
        local_gps_lb_long = gps_pivot.gps_lb_longitude_static;
        local_gps_rb_long = gps_pivot.gps_rb_longitude_static;

        double x_pos = 0;
        double z_pos = 0;


        if (msg.latitude <= local_gps_lb_lat) x_pos = 100;
        else if (msg.latitude >= local_gps_lu_lat) x_pos = 0;
        else
        {
            double ratio = (msg.latitude - local_gps_lb_lat) / (local_gps_lu_lat - local_gps_lb_lat);
            x_pos = pivot_position.x - ratio * (pivot_position.x - lu_position.x);
        }

        if (msg.longitude <= local_gps_lb_long) z_pos = 0;
        else if (msg.longitude >= local_gps_rb_long) z_pos = 100;
        else
        {
            double ratio = (msg.longitude - local_gps_lb_long) / (local_gps_rb_long - local_gps_lb_long);
            z_pos = pivot_position.z + ratio * (rb_position.z - pivot_position.z);
        }


        pos_temp.x = (float)x_pos;
        pos_temp.z = (float)z_pos;

        //print("pose_x:" + pos_temp.x);
        //print("pose_z:" + pos_temp.z);
        vehicle_pose = new Vector3(pos_temp.x, 0, pos_temp.z);
        //transform.position = new Vector3(pos_temp.x, 0, pos_temp.z);
        heading_angle = (float)msg.altitude * 180 / Mathf.PI;
    }

    void calibration_received_update(RosSharp.RosBridgeClient.MessageTypes.Std.Bool msg)
    {
        enable_calibrate = false;
    }

    void reach_goal_update(RosSharp.RosBridgeClient.MessageTypes.Std.Bool msg)
    {
        reach_goal_enter = true;
        enable_goal_point_force = false;
        send_stop = true;
    }

}
