using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float jumpThrust;
	public float speed;
	public float sensitivity;
	public float smoothing;
	public float maxHookDistance;
	public Transform gunEndPoint;
	public float pushForce;

	private float playerCamMin = -90.0f;
	private float playerCamMax = 90.0f;
	private WaitForSeconds hookDuration = new WaitForSeconds(0.1f);
	private Vector2 mouseLook;
	private Vector2 smoothV;
	private Rigidbody playerRB;
	private bool jumping = false;
	private Camera mainCamera;
	private Camera playerCamera;
	private LineRenderer hookLine;

	void Start () {
		playerRB = GetComponent<Rigidbody>();

		mainCamera = Camera.main;
		mainCamera.gameObject.SetActive(false);

		playerCamera = GetComponentInChildren<Camera>();
		hookLine = GetComponent<LineRenderer>();

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

		if(Input.GetMouseButtonDown(0))
		{
			ShootHook();
		}

		if(hookLine.enabled)
		{
			hookLine.SetPosition(0, gunEndPoint.position);
		}
	}

	void Movement()
	{
		float xMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
		float zMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		transform.Translate(zMovement, 0.0f, xMovement);
	}

	void Jump()
	{
		playerRB.velocity = new Vector3(0.0f, jumpThrust, 0.0f);
	}

	void PlayerCameraController()
	{
		Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
		smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
		smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);

		mouseLook += smoothV;
		mouseLook.y = Mathf.Clamp(mouseLook.y, playerCamMin, playerCamMax);

		transform.localRotation = Quaternion.Euler(-mouseLook.y, mouseLook.x, 0.0f);
	}

	void ShootHook()
	{
		Ray ray = playerCamera.ScreenPointToRay(new Vector3(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2, 0));
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, maxHookDistance))
		{
			if(hit.transform.gameObject.tag == "Ground" || hit.transform.gameObject.tag == "Underside")
			{
				Vector3 forceVector = hit.point - transform.position;
				playerRB.velocity = forceVector.normalized * 20.0f;

				hookLine.SetPosition(0, gunEndPoint.position);
				hookLine.SetPosition(1, hit.point);
				hookLine.enabled = true;
			}
			
			if(hit.transform.gameObject.tag == "Player")
			{
				Rigidbody enemyRB = hit.transform.gameObject.GetComponent<Rigidbody>();
				enemyRB.AddForce(playerCamera.transform.forward * pushForce, ForceMode.Impulse);

				hookLine.SetPosition(0, gunEndPoint.position);
				hookLine.SetPosition(1, hit.point);
				StartCoroutine(HookEffect());
			}
		}
	}

	private IEnumerator HookEffect()
	{
		hookLine.enabled = true;
		yield return hookDuration;
		hookLine.enabled = false;
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
			hookLine.enabled = false;
		}
	}
}
