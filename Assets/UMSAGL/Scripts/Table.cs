using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Table : MonoBehaviour {
	// Use this for initialization
	Material mat1;
	Material mat2;
	public float moveSpeed = 0.01f;
	public float turnSpeed = 10f;
	public bool separateTable = false;
	public bool iamSeparated = false;
	void Start () {
		//GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
		//GetComponent<MeshRenderer>().sharedMaterial.color.a = 0.4f;
	}
	
	// Update is called once per frame
	void Update () {
		tablePreview();
	}

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
