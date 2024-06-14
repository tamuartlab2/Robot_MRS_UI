using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockInfo : MonoBehaviour
{
    public double[] gps_lat, gps_lon;
    public double row_number, row_width;
    public float rotate;
    //public int field_type = 0;
    public gps_pivot gps_pivot;

    private Vector3 oldEulerAngles, oldPosition;

    void Start()
    {
        gps_lat = new double[4];
        gps_lon = new double[4];

        Color c = GetComponent<MeshRenderer>().material.color;
        c.a = 0.3f;
        GetComponent<Renderer>().material.color = c;
        float x_0 = transform.position.x + transform.localScale.x / 2.0f;
        float z_0 = transform.position.z - transform.localScale.z / 2.0f;
        transform.Find("cube_0").transform.position = new Vector3(x_0, 0, z_0);
        Vector2 v_temp = gps_pivot.pointToGPS(new Vector3 (x_0, 0, z_0));
        gps_lat[0] = v_temp.x;
        gps_lon[0] = v_temp.y;

        float x_1 = transform.position.x - transform.localScale.x / 2.0f;
        float z_1 = transform.position.z - transform.localScale.z / 2.0f;
        transform.Find("cube_1").transform.position = new Vector3(x_1, 0, z_1);
        v_temp = gps_pivot.pointToGPS(new Vector3(x_1, 0, z_1));
        gps_lat[1] = v_temp.x;
        gps_lon[1] = v_temp.y;

        float x_2 = transform.position.x - transform.localScale.x / 2.0f;
        float z_2 = transform.position.z + transform.localScale.z / 2.0f;
        transform.Find("cube_2").transform.position = new Vector3(x_2, 0, z_2);
        v_temp = gps_pivot.pointToGPS(new Vector3(x_2, 0, z_2));
        gps_lat[2] = v_temp.x;
        gps_lon[2] = v_temp.y;

        float x_3 = transform.position.x + transform.localScale.x / 2.0f;
        float z_3 = transform.position.z + transform.localScale.z / 2.0f;
        transform.Find("cube_3").transform.position = new Vector3(x_3, 0, z_3);
        v_temp = gps_pivot.pointToGPS(new Vector3(x_3, 0, z_3));
        gps_lat[3] = v_temp.x;
        gps_lon[3] = v_temp.y;

        //oldEulerAngles = transform.rotation.eulerAngles;
        //oldPosition = transform.position;
        transform.Rotate(0.0f, rotate, 0.0f);
    }

    void Update()
    {
        if (oldEulerAngles == transform.rotation.eulerAngles && oldPosition == transform.position)
        {

        }
        else
        {
            oldEulerAngles = transform.rotation.eulerAngles;
            oldPosition = transform.position;
            float x_0 = transform.Find("cube_0").transform.position.x;
            float z_0 = transform.Find("cube_0").transform.position.z;
            float x_1 = transform.Find("cube_1").transform.position.x;
            float z_1 = transform.Find("cube_1").transform.position.z;
            float x_2 = transform.Find("cube_2").transform.position.x;
            float z_2 = transform.Find("cube_2").transform.position.z;
            float x_3 = transform.Find("cube_3").transform.position.x;
            float z_3 = transform.Find("cube_3").transform.position.z;

            Vector2 v_temp = gps_pivot.pointToGPS(new Vector3(x_0, 0, z_0));
            gps_lat[0] = v_temp.x;
            gps_lon[0] = v_temp.y;
            v_temp = gps_pivot.pointToGPS(new Vector3(x_1, 0, z_1));
            gps_lat[1] = v_temp.x;
            gps_lon[1] = v_temp.y;
            v_temp = gps_pivot.pointToGPS(new Vector3(x_2, 0, z_2));
            gps_lat[2] = v_temp.x;
            gps_lon[2] = v_temp.y;
            v_temp = gps_pivot.pointToGPS(new Vector3(x_3, 0, z_3));
            gps_lat[3] = v_temp.x;
            gps_lon[3] = v_temp.y;
        }
    }
}
