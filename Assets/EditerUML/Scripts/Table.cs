using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Table : MonoBehaviour {
	// Use this for initialization

	public bool separateTable = false;
	public bool moveTable = false;
	public bool distanceIsON = false;
	public Vector3 defaultPosition;

	public bool flyToFront = false;
	public Vector3 targetPositionToFront;

	public bool rotateTable = false;
	public Vector3 whereToLook;
	public bool iamSeparated = false;
	public int uniqueId;
	public GameObject cylinder;
	public Camera cam;
	

	public Vector3 targetPosition;

	public string direction = "";
	void Start () {
		//FlyCamera scriptCamera = cam.GetComponent<FlyCamera>();
		
		//CreateMetric();
		//GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
		//GetComponent<MeshRenderer>().sharedMaterial.color.a = 0.4f;
	}
	
	// Update is called once per frame
	void Update () {



		if (moveTable)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1000 * Time.deltaTime);
			transform.LookAt(whereToLook);

			//this case is when we want table in front of CAMERA (we will stop when table is 20f in front of camera)
			if (distanceIsON) {
				if (Vector3.Distance(transform.position, targetPosition) < 250) {
					moveTable = false;
					distanceIsON = false;
				}
			}
		}
			

		if (moveTable && transform.position == targetPosition)
		{
			moveTable = false;
			transform.LookAt(whereToLook);
			whereToLook = Vector3.forward; //default look
		}

		//if (flyToFront)
			//FlyToFront(targetPositionToFront);


	}

	/*public void FlyToFront(Vector3 targetPosition)
	{
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, 800 * Time.deltaTime);

		if (transform.position == targetPosition)
			flyToFront = false;
	}*/

	public void RotationRightTable()
	{
		Debug.Log(Time.deltaTime);
		transform.LookAt(Camera.main.transform.position);
		rotateTable = false;
		/*if (rotateTable)
			transform.Rotate(Vector3.up * Time.deltaTime * 10, Space.World);
		if (transform.rotation.y > 0.30f)
		{
			rotateTable = false;
		}*/
	}

	public void RotationLeftTable()
	{
		transform.LookAt(Camera.main.transform.position);
		rotateTable = false;
		/*if (rotateTable)
			transform.Rotate(Vector3.down * Time.deltaTime * 10, Space.World);
		if (transform.rotation.y < -0.30f)
		{

			rotateTable = false;
		}*/
	}

	public void moveTableToOrder()
	{

	}

	/*public void CreateMetric()
	{

		//this just for showcase, after there will be algorhitm on metrics
		System.Random rnd = new System.Random();
		int month = rnd.Next(20, 70);

		GameObject graph = this.transform.Find("Units").gameObject;
		Debug.Log(graph.name);

		foreach (Transform ch in transform)
		{
			//Debug.Log(ch);
		}

		var cy = GameObject.Instantiate(cylinder);
		cy.transform.position = new Vector3(transform.position.x-155f, transform.position.y+125f, transform.position.z-month);

		Vector3 scale = cy.transform.localScale;
		scale.y = month;
		cy.transform.localScale = scale;
		

		cy.transform.parent = this.transform;
	}*/

	public void tablePreview()
	{
		//move right
		if (separateTable && !iamSeparated)
		{
			transform.Translate(Vector3.right * 2f);
			if (transform.position.x >= 500.0f)
			{
				iamSeparated = true;
				separateTable = false;
			}
		}
		//move left
		else if (separateTable && iamSeparated)
		{
			transform.Translate(-Vector3.right * 2f);
			if (transform.position.x < 0.01f)
			{
				iamSeparated = false;
				separateTable = false;
			}
		}

	}

}
