using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;


public class SerialCommunication : MonoBehaviour
{
    private SerialManager mySerialManager;

    private int fingerCommand;
    private Vector3 temp_rotation, temp_position;

    public int i;

    public void setFingerCommand()
    {   

    }

    public void FireBullet()
    {

    }

    // Use this for initialization
    void Start()
    {
        mySerialManager = new SerialManager();
        mySerialManager.SetSerialOpen();

        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        temp_rotation = mySerialManager.GetAngValue();
        temp_rotation.x += 90;

        temp_position = mySerialManager.GetPosValue();
        temp_position.x += this.transform.position.x;
        temp_position.y += this.transform.position.y;
        temp_position.z += this.transform.position.z;

        this.transform.rotation = Quaternion.Euler(temp_rotation);
        this.transform.position = temp_position;
        fingerCommand = mySerialManager.GetFingerValue();

        i++;

        if (i == 1000) FireBullet();
    }
}
