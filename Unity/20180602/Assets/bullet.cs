using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private int damage = 20;
    private float speed = 1000.0f;

	// Use this for initialization
	void Start ()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

}
