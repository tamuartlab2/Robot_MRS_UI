using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static Vector3 Ros2Unity(Vector3 vector3)
    {
        return new Vector3(-vector3.y, vector3.z, vector3.x);
    }

    public static Vector3 Unity2Ros(Vector3 vector3)
    {
        return new Vector3(vector3.z, -vector3.x, vector3.y);
    }

    public static Vector3 Unity2RosV2(Vector3 vector3)
    {
        return new Vector3(-vector3.z, vector3.x, -vector3.y);
    }
    public static Vector3 Ros2UnityScale(Vector3 vector3)
    {
        return new Vector3(vector3.y, vector3.z, vector3.x);
    }

    public static Vector3 Unity2RosScale(Vector3 vector3)
    {
        return new Vector3(vector3.z, vector3.x, vector3.y);
    }

    public static Quaternion Ros2Unity(Quaternion quaternion)
    {
        return new Quaternion(quaternion.y, -quaternion.z, -quaternion.x, quaternion.w);
    }

    public static Quaternion Unity2Ros(Quaternion quaternion)
    {
        return new Quaternion(-quaternion.z, quaternion.x, -quaternion.y, quaternion.w);
    }

    public static Quaternion Unity2RosV2(Quaternion quaternion)
    {
        return new Quaternion(quaternion.z, -quaternion.x, quaternion.y, quaternion.w);
    }
}
