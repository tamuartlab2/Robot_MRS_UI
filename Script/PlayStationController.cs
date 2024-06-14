using UnityEngine;
//using RosMessageTypes.Sensor;
//using RosMessageTypes.Nav;
//using RosMessageTypes.Geometry;
//using RosMessageTypes.Std;
//using RosMessageTypes.BuiltinInterfaces;
//using Unity.Robotics.ROSTCPConnector;
//using UnityEngine.InputSystem;
//using static UnityEditor.PlayerSettings;
//using UnityEngine.WSA;


public class PlayStationController : MonoBehaviour
{
    public bool enable_controller = false;
    public string cmd_vel_topic = "cmd_vel";
    public float linear_speed_max = 1.225f;      //m/s
    public float angular_speed_max = 2.0f;       //rad/s
    public float current_linear_speed = 0.0f;
    public float current_angular_speed = 0.0f;
    public float period_time = 0.1f;

    //ROSConnection m_Ros;
    private float linear_speed = 0f;
    private float angular_speed = 0f;
    private float t_now, t_last;

    // Start is called before the first frame update
    void Start()
    {

        //for (int i = 0; i < Gamepad.all.Count; i++)
        //{
        //    Debug.Log(Gamepad.all[i].name);
        //}

        //m_Ros = ROSConnection.GetOrCreateInstance();
        //m_Ros.RegisterPublisher<TwistMsg>(cmd_vel_topic);

        current_linear_speed = 0.5f * linear_speed_max;
        current_angular_speed = 0.5f * angular_speed_max;
        t_now = Time.time;
        t_last = t_now;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.M))
        //{
        //    enable_controller = !enable_controller;
        //    if (enable_controller)
        //    {
        //        GameObject.Find("Manual Mode Indicator").GetComponent<Renderer>().enabled = true;
        //    }
        //    else
        //    {
        //        GameObject.Find("Manual Mode Indicator").GetComponent<Renderer>().enabled = false;
        //    }
        //}

        //if (enable_controller)
        //{
        //    linear_speed = Gamepad.all[0].leftStick.value.y * current_linear_speed;
        //    angular_speed = -Gamepad.all[0].rightStick.value.x * current_angular_speed;

        //    var vel_msg = new TwistMsg
        //    {
        //        linear = new Vector3Msg 
        //        {
        //            x = linear_speed,
        //        },
        //        angular = new Vector3Msg 
        //        { 
        //            z = angular_speed, 
        //        },
        //    };

        //    t_now = Time.time;
        //    float dt = t_now - t_last;
        //    if (dt > period_time)
        //    {
        //        t_last = t_now;
        //        m_Ros.Publish(cmd_vel_topic, vel_msg);
        //        //print("Speed published!");
        //        //print(vel_msg.linear.x);
        //        //print(vel_msg.angular.z);
        //    }

        //    //if (Gamepad.all[0].rightStick.left.isPressed)
        //    //{
        //    //    //print("move left");
        //    //}

        //    //if (Gamepad.all[0].rightStick.right.isPressed)
        //    //{
        //    //    //print("move right");
        //    //}

        //    //if (Gamepad.all[0].leftStick.up.isPressed)
        //    //{
        //    //    //print("move forward");
        //    //}

        //    //if (Gamepad.all[0].leftStick.down.isPressed)
        //    //{
        //    //    //print("move backward");
        //    //}

        //    if (Gamepad.all[0].dpad.up.wasReleasedThisFrame)
        //    {
        //        //print("increase speed");
        //        current_linear_speed += 0.1f * linear_speed_max;
        //        current_angular_speed += 0.1f * angular_speed_max;
        //        if (current_linear_speed > linear_speed_max)
        //        {
        //            current_linear_speed = linear_speed_max;
        //        }
        //        if (current_angular_speed > angular_speed_max)
        //        {
        //            current_angular_speed = angular_speed_max;
        //        }

        //    }
        //    if (Gamepad.all[0].dpad.down.wasReleasedThisFrame)
        //    {
        //        //print("decrease speed");
        //        current_linear_speed -= 0.1f * linear_speed_max;
        //        current_angular_speed -= 0.1f * angular_speed_max;
        //        if (current_linear_speed < 0f)
        //        {
        //            current_linear_speed = 0f;
        //        }
        //        if (current_angular_speed < 0f)
        //        {
        //            current_angular_speed = 0f;
        //        }
        //    }

        //}
    }
}
