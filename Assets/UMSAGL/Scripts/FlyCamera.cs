using UnityEngine;
using System.Linq;
using System;
using TMPro;


	public class FlyCamera : MonoBehaviour
	{
		/*
		Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.
		Converted to C# 27-02-13 - no credit wanted.
		Reformatted and cleaned by Ryan Breaker 23-6-18

		Original comment:
		Simple flycam I made, since I couldn't find any others made public.
		Made simple to use (drag and drop, done) for regular keyboard layout.

		Controls:
		WASD  : Directional movement
		Shift : Increase speed
		Space : Moves camera directly up per its local Y-axis
		*/

		public float mainSpeed = 40.0f;		  // Regular speed
		public float shiftAdd = 95.0f;		  // Amount to accelerate when shift is pressed
		public float maxShift = 150.0f;		  // Maximum speed when holding shift
		public float camSens = 0.15f;		  // Mouse sensitivity
		public bool goToTable = false;        // go to separate Table
		public bool frontView = false;
		public bool goToDefaultPosition = false;
		public bool sideView = false;
		public bool cameraIsInFront;
		public bool lookAt = true;

		public Vector3 searchedPosition;
		public Transform searchedTable;

		public Vector3 movement;
		public Vector3 lastTablePosition;

		public string direction;


	public GameObject[] IgnoredInputs;

		private Vector3 lastMouse = new Vector3(255, 255, 255);  // kind of in the middle of the screen, rather than at the top (play)
		private float totalRun = 1.0f;


	//TODO: refactor this code...
	void Update()
	{
		if (goToTable)
			flyToTable(searchedPosition, searchedTable);

		if (goToDefaultPosition)
			FlyToDefaultPosition();


		if (Input.GetMouseButton(1))
		{
			lastMouse = Input.mousePosition - lastMouse;
			lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
			lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
			transform.eulerAngles = lastMouse;
		}
		lastMouse = Input.mousePosition;

		if (IgnoredInputs.Any(item => item.GetComponentInChildren<TMP_InputField>()?.isFocused == true))
		{
			return;
		}

		// Keyboard commands
		Vector3 p = GetBaseInput();
		if (Input.GetKey(KeyCode.LeftShift))
		{
			totalRun += Time.deltaTime;
			p *= totalRun * shiftAdd;
			p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
			p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
			p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
		}
		else
		{
			totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
			p *= mainSpeed;
		}

		p *= Time.deltaTime;
		transform.Translate(p);
	}
	

	//camera will fly to default position
	public void FlyToDefaultPosition()
	{
		goToDefaultPosition = true;
		Vector3 defaultPos = new Vector3(0, 0, -400);
		transform.position = Vector3.MoveTowards(transform.position, defaultPos, 800 * Time.deltaTime);
		transform.LookAt(Vector3.forward);

		if (transform.position == defaultPos)
		{		
			goToDefaultPosition = false;
			transform.LookAt(Vector3.forward);
		}
	}

	//fly with camera to some table, and look on table
	public void flyToTable(Vector3 searchedPosition, Transform searchedTable)
	{
		transform.position = Vector3.MoveTowards(transform.position, searchedPosition, 800 * Time.deltaTime);
		transform.LookAt(searchedTable);

		if (transform.position == searchedPosition)
		{
		goToTable = false;
		transform.LookAt(searchedTable);
		}
				
	}


	// Returns the basic values, if it's 0 than it's not active.
	private Vector3 GetBaseInput()
		{
			Vector3 p_Velocity = new Vector3();

			// Forwards
			if (Input.GetKey(KeyCode.W))
				p_Velocity += new Vector3(0, 0, 1);

			// Backwards
			if (Input.GetKey(KeyCode.S))
				p_Velocity += new Vector3(0, 0, -1);

			// Left
			if (Input.GetKey(KeyCode.A))
				p_Velocity += new Vector3(-1, 0, 0);

			// Right
			if (Input.GetKey(KeyCode.D))
				p_Velocity += new Vector3(1, 0, 0);

			// Up
			if (Input.GetKey(KeyCode.Space))
				p_Velocity += new Vector3(0, 1, 0);

			// Down
			if (Input.GetKey(KeyCode.LeftControl))
				p_Velocity += new Vector3(0, -1, 0);

			return p_Velocity;
		}
	}
