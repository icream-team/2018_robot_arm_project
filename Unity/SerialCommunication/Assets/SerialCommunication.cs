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
    private Vector3 temp_rotation, temp_position, camera_rotation, camera_position;

    private GameObject gun;
    private GameObject temp_bullet;
    private GameObject camera;
    public int i;
    private bool isBullet = false;

    public void SetFingerCommand()
    {   

    }

    public void FireBullet( )
    {
        // UnityEngine.Debug.Log("fire bullet");

        for ( int i = -1; i <= 1; i++ )
        {
            for ( int j = -1; j <= 1; j++ )
            {
                for( int k = -1; k <= 1; k++ )
                {
                    var bullet = (GameObject)Instantiate(temp_bullet, temp_bullet.transform.position, temp_bullet.transform.rotation);
                    bullet.GetComponent<Rigidbody>().velocity = new Vector3( i * 1.0f, j * 1.0f, k * 1.0f ) * 6;
                    Destroy(bullet, 5.0f);
                }
            }
        }

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
        //temp_bullet.renderer.material.color(1, 1, 1, 0);
        gun = (GameObject.FindGameObjectWithTag("gun"));
        temp_bullet = (GameObject.FindGameObjectWithTag("EditorOnly"));
        temp_bullet.transform.parent = gun.transform;

        mySerialManager = new SerialManager();
        mySerialManager.SetSerialPort("COM3");
        mySerialManager.SetReadTimeout(100);
        mySerialManager.SetWriteTimeout(100);
        mySerialManager.SetSerialOpen();

        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        camera = GameObject.FindGameObjectWithTag("Player");

        try
        {
            temp_rotation = mySerialManager.GetAngValue();
            //temp_rotation.z *= -1;

            //temp_position = mySerialManager.GetPosValue();
            //temp_position.x += gun.transform.position.x;
            //temp_position.y += gun.transform.position.y;
            //temp_position.z += gun.transform.position.z;
            //camera_position.x = temp_position.x - 2;
            //camera_position.y = temp_position.y - 1;
            //camera_position.z = temp_position.z - 3;



            this.transform.rotation = Quaternion.Euler( temp_rotation );
            //bullets.transform.rotation = Quaternion.Euler(temp_rotation);
            //this.transform.position = temp_position;

            //camera.transform.rotation = Quaternion.Euler(camera_rotation);
            //camera.transform.position = camera_position;

            fingerCommand = mySerialManager.GetFingerValue();
                
            if (fingerCommand == 24) FireBullet();

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
            Application.Quit();
        }

    }
}