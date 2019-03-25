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
	}
	
	// Update is called once per frame
	void Update () {

		if (moveTable)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1000 * Time.deltaTime);
			transform.LookAt(whereToLook);
			transform.LookAt(2 * transform.position - whereToLook);

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
			Debug.Log("sssss");
			transform.LookAt(2 * transform.position - whereToLook);
			whereToLook = Vector3.forward; //default look
		}

	}

}
