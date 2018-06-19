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

    private GameObject gun;
    private GameObject temp_bullet;
    private GameObject camera;
    public int i;

    public void setFingerCommand()
    {   

    }

    public void FireBullet()
    {
        var bullet = (GameObject)Instantiate( temp_bullet, temp_bullet.transform.position, gun.transform.rotation);

        // UnityEngine.Debug.Log("fire bullet");

        bullet.GetComponent<Rigidbody>().velocity = gun.transform.forward * 6;

        Destroy(bullet, 5.0f);

        mySerialManager.SendVibrationMessage(1);

        //UnityEngine.Debug.Log("x pos : " + gun.transform.position.x);
        //UnityEngine.Debug.Log("y pos : " + gun.transform.position.y);
        //UnityEngine.Debug.Log("z pos : " + gun.transform.position.z);
        //UnityEngine.Debug.Log("x ang : " + gun.transform.rotation.x);
        //UnityEngine.Debug.Log("y ang : " + gun.transform.rotation.y);
        //UnityEngine.Debug.Log("z ang : " + gun.transform.rotation.z);

    }

    // Use this for initialization
    void Start()
    {
        temp_bullet = (GameObject.FindGameObjectWithTag("EditorOnly"));
        gun = (GameObject.FindGameObjectWithTag("gun"));

        mySerialManager = new SerialManager();
        mySerialManager.SetSerialPort("COM3");
        mySerialManager.SetSerialOpen();

        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");

        try
        {
            temp_rotation = mySerialManager.GetAngValue();
            temp_rotation.x += 90;

            temp_position = mySerialManager.GetPosValue();
            temp_position.x += gun.transform.position.x;
            temp_position.y += gun.transform.position.y;
            temp_position.z += gun.transform.position.z;

            camera.transform.position = gun.transform.position;

            this.transform.rotation = Quaternion.Euler(temp_rotation);
            this.transform.position = temp_position;
            fingerCommand = mySerialManager.GetFingerValue();
        } catch ( System.Exception )
        {
            //UnityEngine.Debug.Log("null exception:serial manage is null");
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            FireBullet();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            mySerialManager.StopSerialThread();
        }

    }
}
