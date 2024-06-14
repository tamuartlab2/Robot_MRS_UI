using System;
using System.Collections.Generic;
using System.Linq;

//using RosMessageTypes.Sensor; required for ROS - TCP 
//using RosMessageTypes.Std; required for ROS - TCP
//using RosMessageTypes.BuiltinInterfaces; required for ROS - TCP
//using Unity.Robotics.Core; required for ROS - TCP
using Unity.Robotics.Core_yuan; // adding for yuan
using UnityEngine;
//using Unity.Robotics.ROSTCPConnector; required for ROS - TCP
using UnityEngine.Serialization;

public class LaserScanSensor : MonoBehaviour
{
    public string topic;
    [FormerlySerializedAs("TimeBetweenScansSeconds")]
    public double PublishPeriodSeconds = 0.1f;
    public float RangeMetersMin = 0.1f;
    public float RangeMetersMax = 30.0f;
    public float ScanAngleStartDegrees = 180;
    public float ScanAngleEndDegrees = -179;
    // Change the scan start and end by this amount after every publish
    public float ScanOffsetAfterPublish = 0f;
    public int NumMeasurementsPerScan = 915;
    public float TimeBetweenMeasurementsSeconds = 0.0f;
    public string LayerMaskName = "TurtleBot3Manual";
    public string FrameId = "lidar_frame";

    public float F_obstacle_x = 0.1f;
    public float F_obstacle_y = 0.0f;
    public float lidar_effective_distance = 2.5f;

    public float K_e = 0.8f;

    float m_CurrentScanAngleStart;
    float m_CurrentScanAngleEnd;
    //ROSConnection m_Ros; required for ROS - TCP
    double m_TimeNextScanSeconds = -1;

    int m_NumMeasurementsTaken;
    List<float> ranges = new List<float>();

    bool isScanning = false;
    double m_TimeLastScanBeganSeconds = -1;

    protected virtual void Start()
    {
        //m_Ros = ROSConnection.GetOrCreateInstance(); required for ROS - TCP
        //m_Ros.RegisterPublisher<LaserScanMsg>(topic); required for ROS - TCP

        m_CurrentScanAngleStart = ScanAngleStartDegrees;
        m_CurrentScanAngleEnd = ScanAngleEndDegrees;

        m_TimeNextScanSeconds = Clock.Now + PublishPeriodSeconds;
    }

    void BeginScan()
    {
        isScanning = true;
        m_TimeLastScanBeganSeconds = Clock.Now;
        m_TimeNextScanSeconds = m_TimeLastScanBeganSeconds + PublishPeriodSeconds;
        m_NumMeasurementsTaken = 0;
    }

    public void EndScan()
    {
        if (ranges.Count == 0)
        {
            Debug.LogWarning($"Took {m_NumMeasurementsTaken} measurements but found no valid ranges");
        }
        else if (ranges.Count != m_NumMeasurementsTaken || ranges.Count != NumMeasurementsPerScan)
        {
            Debug.LogWarning($"Expected {NumMeasurementsPerScan} measurements. Actually took {m_NumMeasurementsTaken}" +
                             $"and recorded {ranges.Count} ranges.");
        }

        //var timestamp = new TimeStamp(Clock.time); required for ROS - TCP
        // Invert the angle ranges when going from Unity to ROS
        var angleStartRos = -m_CurrentScanAngleStart * Mathf.Deg2Rad;
        var angleEndRos = -m_CurrentScanAngleEnd * Mathf.Deg2Rad;
        if (angleStartRos > angleEndRos)
        {
            Debug.LogWarning("LaserScan was performed in a clockwise direction but ROS expects a counter-clockwise scan, flipping the ranges...");
            var temp = angleEndRos;
            angleEndRos = angleStartRos;
            angleStartRos = temp;
            ranges.Reverse();
        }

        /*var msg = new LaserScanMsg
        {
            header = new HeaderMsg
            {
                frame_id = FrameId,
                stamp = new TimeMsg
                {
                    sec = timestamp.Seconds,
                    nanosec = timestamp.NanoSeconds,
                }
            },
            range_min = RangeMetersMin,
            range_max = RangeMetersMax,
            angle_min = angleStartRos,
            angle_max = angleEndRos,
            angle_increment = (angleEndRos - angleStartRos) / NumMeasurementsPerScan,
            time_increment = TimeBetweenMeasurementsSeconds,
            scan_time = (float)PublishPeriodSeconds,
            intensities = new float[ranges.Count],
            ranges = ranges.ToArray(),
        };*/ // required for ROS - TCP

        //m_Ros.Publish(topic, msg); required for ROS - TCP

        //print(ranges[0]);

        int len_count = ranges.Count;
        float angle = ScanAngleStartDegrees * Mathf.PI / 180.0f;
        float angle_increment = (ScanAngleEndDegrees - ScanAngleStartDegrees) * Mathf.PI / (180.0f * len_count);
        float F_o_x = 0.0f;
        float F_o_y = 0.0f;
        for (int i = 0; i < len_count; i++)
        {
            if (ranges[i] < lidar_effective_distance)
            {
                float F = -K_e * angle_increment / ranges[i];
                F_o_x += F * Mathf.Cos(angle);
                F_o_y += F * Mathf.Sin(angle);
            }
            angle += angle_increment;;
        }
        F_obstacle_x = -F_o_x;
        F_obstacle_y = F_o_y;


        m_NumMeasurementsTaken = 0;
        ranges.Clear();
        isScanning = false;
        var now = (float)Clock.time;
        if (now > m_TimeNextScanSeconds)
        {
            Debug.LogWarning($"Failed to complete scan started at {m_TimeLastScanBeganSeconds:F} before next scan was " +
                             $"scheduled to start: {m_TimeNextScanSeconds:F}, rescheduling to now ({now:F})");
            m_TimeNextScanSeconds = now;
        }

        m_CurrentScanAngleStart += ScanOffsetAfterPublish;
        m_CurrentScanAngleEnd += ScanOffsetAfterPublish;
        if (m_CurrentScanAngleStart > 360f || m_CurrentScanAngleEnd > 360f)
        {
            m_CurrentScanAngleStart -= 360f;
            m_CurrentScanAngleEnd -= 360f;
        }

    }

    public void Update()
    {
        if (!isScanning)
        {
            if (Clock.NowTimeInSeconds < m_TimeNextScanSeconds)
            {
                return;
            }

            BeginScan();
        }


        var measurementsSoFar = TimeBetweenMeasurementsSeconds == 0 ? NumMeasurementsPerScan :
            1 + Mathf.FloorToInt((float)(Clock.time - m_TimeLastScanBeganSeconds) / TimeBetweenMeasurementsSeconds);
        if (measurementsSoFar > NumMeasurementsPerScan)
            measurementsSoFar = NumMeasurementsPerScan;

        var yawBaseDegrees = transform.rotation.eulerAngles.y;
        while (m_NumMeasurementsTaken < measurementsSoFar)
        {
            var t = m_NumMeasurementsTaken / (float)NumMeasurementsPerScan;
            var yawSensorDegrees = Mathf.Lerp(m_CurrentScanAngleStart, m_CurrentScanAngleEnd, t);
            var yawDegrees = yawBaseDegrees + yawSensorDegrees;
            var directionVector = Quaternion.Euler(0f, yawDegrees, 0f) * Vector3.forward;
            var measurementStart = RangeMetersMin * directionVector + transform.position;
            var measurementRay = new Ray(measurementStart, directionVector);
            var foundValidMeasurement = Physics.Raycast(measurementRay, out var hit, RangeMetersMax);
            // Only record measurement if it's within the sensor's operating range

            //print(foundValidMeasurement);
            //print(hit.distance);
            if (foundValidMeasurement)
            {
                ranges.Add(hit.distance);
            }
            else
            {
                ranges.Add(float.MaxValue);
            }

            // Even if Raycast didn't find a valid hit, we still count it as a measurement
            ++m_NumMeasurementsTaken;
        }


        if (m_NumMeasurementsTaken >= NumMeasurementsPerScan)
        {
            if (m_NumMeasurementsTaken > NumMeasurementsPerScan)
            {
                Debug.LogError($"LaserScan has {m_NumMeasurementsTaken} measurements but we expected {NumMeasurementsPerScan}");
            }
            //print("End scan enter");
            EndScan();
        }

    }
}
