using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float jumpThrust = 10.0f;
	public float speed = 5.0f;
	public float sensitivity = 10.0f;
	public float smoothing = 10.0f;
	public float playerCamMin = -20.0f;
	public float playerCamMax = 10.0f;

	private Vector2 mouseLook;
	private Vector2 smoothV;
	private Rigidbody playerRB;
	private bool jumping = false;
	private Camera mainCamera;
	private Camera playerCamera;

	void Start () {
		playerRB = GetComponent<Rigidbody>();

		mainCamera = Camera.main;
		mainCamera.gameObject.SetActive(false);

		playerCamera = GetComponentInChildren<Camera>();

		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update () {

		Movement();
		PlayerCameraController();

		if (Input.GetKeyDown(KeyCode.Space) && !jumping)
		{
			jumping = true;
			Jump();
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}

	void Movement()
	{
		float xMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
		float zMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		transform.Translate(-xMovement, 0.0f, zMovement);
	}

	void Jump()
	{
		playerRB.AddForce(0, jumpThrust, 0, ForceMode.Impulse);
	}

	void PlayerCameraController()
	{
		Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
		smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
		smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);

		mouseLook += smoothV;
		mouseLook.y = Mathf.Clamp(mouseLook.y, playerCamMin, playerCamMax);

		playerCamera.transform.localRotation = Quaternion.Euler(-mouseLook.y, -90f, 0.0f);
		transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
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
