using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OperationCMD : MonoBehaviour
{
    public bool operationStart = false;
    public Text ARM_status;
    //void Start()
    //{

    //}

    //void Update()
    //{

    //}
    public void ARMOnButtonClick()
    {
        operationStart = !operationStart;
        if (operationStart)
        {
            ARM_status.text = "ARMED";
        }
        else
        {
            ARM_status.text = "DISARMED";
        }
    }
}
