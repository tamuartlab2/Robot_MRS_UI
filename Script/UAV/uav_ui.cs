using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Nav;

public class uav_ui : MonoBehaviour
{
    public GameObject UAV, VideoPanel;
    public OperationCMD operationCMD;
    public gps_pivot gps_pivot;
    public RosConnector m_Ros;
    public string cmd_topic = "uav/set_pose";
    public string uav_gps_topic = "uav/gps_pose";
    public Text CurrentModeText, AirSpeedText, AltitudeText, UAVBattery;
    public double air_speed = 1.0, set_altitude = 5.0;
    public Dropdown UAV_Mode_Dropdown;
    public Slider AirSpeedSlider;
    public Slider AltitudeSlider;

    private string cmd_topic_pub;
    private double x_pos = 0.0, y_pos = 0.0, z_pos = 0.0, battery_lv, heading;
    private bool receive_pose_flag = false, video_enabled = false;
    private int uav_mode;
    private float t_now = 0.0f, t_last = 0.0f;
    public float period_time = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_Ros.IsConnected.WaitOne(1 * 1000))
            Debug.LogWarning("Failed to subscribe: RosConnector not connected");

        m_Ros.RosSocket.Subscribe<NavSatFix_ArtLab>(uav_gps_topic, loca_update, (int)(0.1 * 1000.0));
        cmd_topic_pub = m_Ros.RosSocket.Advertise<NavSatFix_ArtLab>(cmd_topic);

        operationCMD = GetComponent<OperationCMD>();

        VideoPanel.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        air_speed = AirSpeedSlider.value * 9.0 + 1.00;
        AirSpeedText.text = "Air Speed: " + air_speed.ToString("F2") + "m/s";
        set_altitude = AltitudeSlider.value * 45.0 +5.0;
        AltitudeText.text = "Altitude:" + set_altitude.ToString("F2") + "m";
        if (receive_pose_flag == true)
        {
            receive_pose_flag = false;
            //print(new Vector3((float)x_pos, (float)y_pos, (float)z_pos));
            //UAV.transform.position = new Vector3((float)x_pos, 0, (float)z_pos);
            y_pos = y_pos / 5.0;
            UAV.transform.Find("Aion_robot").transform.position = new Vector3((float)x_pos, (float)y_pos, (float)z_pos);
            //UAV.transform.Find("Aion_robot").transform.Find("Selection_Indicator").transform.position = new Vector3((float)x_pos, 0, (float)z_pos);
            UAV.transform.Find("Aion_robot").transform.rotation = Quaternion.Euler(0f, (float)heading, 0f);
            UAVBattery.text = "UAV Battery: " + battery_lv.ToString("F2") + "%";
            if (uav_mode == 0)
                CurrentModeText.text = "Mode: LAND";
            else if (uav_mode == 1)
                CurrentModeText.text = "Mode: RTL";
            else if (uav_mode == 2)
                CurrentModeText.text = "Mode: GUIDED";
        }

        t_now = Time.time;
        float dt = t_now - t_last;
        if (dt > period_time)
        {
            t_last = t_now;
            if (operationCMD.operationStart && uav_mode == 2)
            {
                Vector3 goal_pose = UAV.transform.Find("Goal Point").transform.position;
                Vector2 gps_p_temp = gps_pivot.pointToGPS(goal_pose);
                double[] sendspeed = new double[9];
                sendspeed[0] = air_speed;
                var msg = new NavSatFix_ArtLab
                {
                    //header = new RosSharp.RosBridgeClient.MessageTypes.Std.Header()
                    //{
                    //    frame_id = "map",
                    //},

                    latitude = gps_p_temp.x,
                    longitude = gps_p_temp.y,
                    altitude = set_altitude,
                    position_covariance = sendspeed,
                };
                m_Ros.RosSocket.Publish(cmd_topic_pub, msg);
            }
        }
    }

    void loca_update(NavSatFix_ArtLab msg)
    {
        receive_pose_flag = true;
        double uav_lat = msg.latitude;
        double uav_lon = msg.longitude;
        double uav_alt = msg.altitude;
        heading = msg.position_covariance[0];
        battery_lv = msg.position_covariance[1];
        uav_mode = (int)msg.position_covariance[2]; //0:LAND, 1:RTL, 2:GUIDED
        Vector2 p_tem = gps_pivot.gpsToPoint(uav_lat, uav_lon);
        x_pos = p_tem.x;
        y_pos = uav_alt;
        z_pos = p_tem.y;
        
    }

    public void OnSendModeButtonClick()
    {
        if (UAV_Mode_Dropdown.value == 0)  //TakeOff
        {
            var msg = new NavSatFix_ArtLab
            {
                //header = new RosSharp.RosBridgeClient.MessageTypes.Std.Header()
                //{
                //    frame_id = "map",
                //},

                latitude = 0.0,
                longitude = 0.0,
                altitude = 1.0,
            };
            m_Ros.RosSocket.Publish(cmd_topic_pub, msg);
        }
        else if (UAV_Mode_Dropdown.value == 1)  //RTL
        {
            var msg = new NavSatFix_ArtLab
            {
                //header = new RosSharp.RosBridgeClient.MessageTypes.Std.Header()
                //{
                //    frame_id = "map",
                //},

                latitude = 0.0,
                longitude = 0.0,
                altitude = 0.0,
            };
            m_Ros.RosSocket.Publish(cmd_topic_pub, msg);
        }
        else if (UAV_Mode_Dropdown.value == 2)  //LAND
        {
            var msg = new NavSatFix_ArtLab
            {
                //header = new RosSharp.RosBridgeClient.MessageTypes.Std.Header()
                //{
                //    frame_id = "map",
                //},

                latitude = 0.0,
                longitude = 0.0,
                altitude = -1.0,
            };
            m_Ros.RosSocket.Publish(cmd_topic_pub, msg);
        }
    }

    public void OnVideoButtonClick()
    {
        video_enabled = !video_enabled;
        VideoPanel.GetComponent<MeshRenderer>().enabled = video_enabled;
    }

}
