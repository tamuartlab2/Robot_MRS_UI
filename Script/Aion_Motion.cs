using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.WSA;

public class Aion_Motion : MonoBehaviour
{
    public Rigidbody rb;
    public float linear_speed = 0.0f;       // m/s
    public float angular_speed = 0.0f;      // rad/s
    public float maximum_linear_speed = 0.8f;    // m/s
    public float maximum_angular_speed = 0.5f;    // rad/s

    //private Force_on_robot Force_SD;
    //private float t_now = 0.0f, t_last = 0.0f;
    //private float period_time = 0.5f;
    //private float F_x = 0.0f, F_y = 0.0f;
    private Vector3 init_pose = new Vector3(0, 0, -1);
    private Vector3 init_angle = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Force_SD = GetComponent<Force_on_robot>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
