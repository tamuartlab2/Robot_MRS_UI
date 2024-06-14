using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gps_pivot : MonoBehaviour
{
    //CC Farm 1
    //public double gps_home_latitude_st = 27.783494;
    //public double gps_home_longitude_st = -97.561476;

    //public double gps_lu_latitude = 27.783494;
    //public double gps_lu_longitude = -97.561476;

    //public double gps_lb_latitude = 27.782436; 
    //public double gps_lb_longitude = -97.561476;

    //public double gps_ru_latitude = 27.783494;
    //public double gps_ru_longitude = -97.559930;

    //public double gps_rb_latitude = 27.782436;
    //public double gps_rb_longitude = -97.559930;

    //CC Farm 2
    //public double gps_home_latitude_st = 27.782388;
    //public double gps_home_longitude_st = -97.562427;

    //public double gps_lu_latitude = 27.782388;
    //public double gps_lu_longitude = -97.562427;

    //public double gps_lb_latitude = 27.780952;
    //public double gps_lb_longitude = -97.562427;

    //public double gps_ru_latitude = 27.782388;
    //public double gps_ru_longitude = -97.560582;

    //public double gps_rb_latitude = 27.780952;
    //public double gps_rb_longitude = -97.560582;

    //Map_Simpson_Drill_Field
    //public double gps_home_latitude_st = 30.614555;
    //public double gps_home_longitude_st = -96.341894;

    //public double gps_lu_latitude = 30.614555;
    //public double gps_lu_longitude = -96.3429360132072;

    //public double gps_lb_latitude = 30.6136925039626;
    //public double gps_lb_longitude = -96.3429360132072;

    //public double gps_ru_latitude = 30.614555;
    //public double gps_ru_longitude = -96.341894;

    //public double gps_rb_latitude = 30.6136925039626;
    //public double gps_rb_longitude = -96.341894;

    //Cattle Farm
    public double gps_home_latitude_st = 30.561244153153154;
    public double gps_home_longitude_st = -96.41694171338352;

    public double gps_lu_latitude = 30.561244153153154;
    public double gps_lu_longitude = -96.41694171338352;

    public double gps_lb_latitude = 30.554937846846848;
    public double gps_lb_longitude = -96.41694171338352;

    public double gps_ru_latitude = 30.561244153153154;
    public double gps_ru_longitude = -96.40961828661648;

    public double gps_rb_latitude = 30.554937846846848;
    public double gps_rb_longitude = -96.40961828661648;

    //Thompson hall lab
    //public double gps_home_latitude_st = 30.6178609009009;
    //public double gps_home_longitude_st = -96.34211783941312;

    //public double gps_lu_latitude = 30.6178609009009;
    //public double gps_lu_longitude = -96.34211783941312;

    //public double gps_lb_latitude = 30.616059099099097;
    //public double gps_lb_longitude = -96.34211783941312;

    //public double gps_ru_latitude = 30.6178609009009;
    //public double gps_ru_longitude = -96.34002416058688;

    //public double gps_rb_latitude = 30.6178609009009;
    //public double gps_rb_longitude = -96.34002416058688;

    static public double gps_lu_latitude_static;
    static public double gps_lb_latitude_static;
    static public double gps_lb_longitude_static;
    static public double gps_rb_longitude_static;

    public double robot_long, robot_lat, x_pos, z_pos;


    private double local_gps_lu_lat;
    private double local_gps_lb_lat;
    private double local_gps_lb_long;
    private double local_gps_rb_long;
    private Vector3 pivot_position;
    private Vector3 rb_position;
    private Vector3 lu_position;
    private double x_denom;
    private double z_denom;

    // Start is called before the first frame update
    void Start()
    {
        gps_lu_latitude_static = gps_lu_latitude;
        gps_lb_latitude_static = gps_lb_latitude;
        gps_lb_longitude_static = gps_lb_longitude;
        gps_rb_longitude_static = gps_rb_longitude;

        GameObject lb_pivot = GameObject.Find("bottom_left_pivot");
        pivot_position = lb_pivot.transform.position;
        GameObject rb_pivot = GameObject.Find("bottom_right_pivot");
        rb_position = rb_pivot.transform.position;
        GameObject lu_pivot = GameObject.Find("up_left_pivot");
        lu_position = lu_pivot.transform.position;
        x_denom = pivot_position.x - lu_position.x;
        z_denom = rb_position.z - pivot_position.z;


        local_gps_lu_lat = gps_lu_latitude_static;
        local_gps_lb_lat = gps_lb_latitude_static;
        local_gps_lb_long = gps_lb_longitude_static;
        local_gps_rb_long = gps_rb_longitude_static;
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public Vector2 pointToGPS(Vector3 position)
    {

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
        return new Vector2((float)robot_lat, (float)robot_long);
    }

    public Vector2 gpsToPoint(double latitude, double longitude)
    {

        if (latitude <= local_gps_lb_lat) x_pos = 100;
        else if (latitude >= local_gps_lu_lat) x_pos = 0;
        else
        {
            double ratio = (latitude - local_gps_lb_lat) / (local_gps_lu_lat - local_gps_lb_lat);
            x_pos = pivot_position.x - ratio * (pivot_position.x - lu_position.x);
        }

        if (longitude <= local_gps_lb_long) z_pos = 0;
        else if (longitude >= local_gps_rb_long) z_pos = 100;
        else
        {
            double ratio = (longitude - local_gps_lb_long) / (local_gps_rb_long - local_gps_lb_long);
            z_pos = pivot_position.z + ratio * (rb_position.z - pivot_position.z);
        }

        return new Vector2((float)x_pos, (float)z_pos);
    }
}
