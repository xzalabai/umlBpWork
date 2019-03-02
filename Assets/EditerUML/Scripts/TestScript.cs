using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
	public float speed;
	public GameObject g;
	public Vector3 v = new Vector3(40, 40, 40);
	// Use this for initialization

	public int numObjects = 10;
    public GameObject prefab;
 
     void Start() {
		/*Vector3 center = Camera.main.transform.position;
		for (int i = 0; i < numObjects; i++)
		{
			Vector3 pos = RandomCircle(center, 70.0f);
			Quaternion rot = Quaternion.FromToRotation(Vector3.left, center - pos);
			Instantiate(g, pos, rot);
		}*/
	}
	
	float startAng = -90;
     Vector3 RandomCircle ( Vector3 center ,   float radius){
		float ang = startAng;
		startAng += 18;
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		pos.y = center.y;
		return pos;
	}
 
	
	// Update is called once per frame
	void Update () {
		//transform.position = Vector3.MoveTowards(transform.position, v, 10 * Time.deltaTime);
	}
}
