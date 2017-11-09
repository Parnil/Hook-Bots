using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float jumpThrust = 10f;
	private float speed = 5f;
	private Rigidbody playerRB;
	private bool jumping = false;

	// Use this for initialization
	void Start () {
		playerRB = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

		Movement();

		if (Input.GetKeyDown(KeyCode.Space) && !jumping)
		{
			jumping = true;
			Jump();
		}
	}

	void Movement()
	{
		float movement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		transform.Translate(movement, 0, 0);
	}

	void Jump()
	{
		playerRB.AddForce(0, jumpThrust, 0, ForceMode.Impulse);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Respawn")
		{
			transform.SetPositionAndRotation(new Vector3(0, 2, 0), Quaternion.identity);
		}

		if(collision.gameObject.tag == "Ground")
		{
			jumping = false;
		}
	}
}
