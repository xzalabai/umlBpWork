using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
	public float speed;
	public GameObject g;
	public Vector3 v = new Vector3(40, 40, 40);
	// Use this for initialization
	void Start () {
		g = GameObject.FindGameObjectWithTag("table");
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position = Vector3.MoveTowards(transform.position, v, 10 * Time.deltaTime);
	}
}
