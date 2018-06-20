using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainScript : MonoBehaviour
{
    private GameObject main_camera;
    private GameObject gun;

    float ax, ay, az;
    float px, py, pz;

    float d;

    private void updateGunRotation()
    {
        ax = main_camera.transform.rotation.x;
        ay = main_camera.transform.rotation.y;
        az = main_camera.transform.rotation.z;

        px = (float)(d * Math.Sin(az) * Math.Cos(ax));
        py = (float)(d * Math.Sin(ax) * Math.Sin(az));
        pz = (float)(d * Math.Cos(az));

        gun.transform.rotation = main_camera.transform.rotation;
        gun.transform.position = main_camera.transform.position;


    }

	// Use this for initialization
	void Start ()
    {
        gun = GameObject.FindGameObjectWithTag("gun") as GameObject;
        main_camera = GameObject.FindGameObjectWithTag("MainCamera") as GameObject;

        d = Vector3.Distance(Vector3.zero, gun.transform.position);
	}
	
	// Update is called once per frame
	void Update ()
    {
        updateGunRotation();
	}
}
