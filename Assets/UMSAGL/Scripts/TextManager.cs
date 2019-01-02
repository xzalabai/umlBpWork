using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour {

	public Text tableNumber;
	public Text metricText;
	public UnityEngine.UI.Slider numberOfTableSlider;
	public GameObject ahoj;

	// Use this for initialization
	void Start () {
		//tableNumber = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//change the Table Number under the slider (when moving with Slider)
	public void ChangeTextOnSlider()
	{
		
		tableNumber.text = "Table n. " + numberOfTableSlider.value.ToString();

	}

	//change Show Metric on Hide Metric (or reverse)
	public bool isHidden = true;
	public void ChangeTextOnMetricButton()
	{
		if (isHidden)
		{
			metricText.text = "Hide metrics";
			isHidden = false;
		}
		else
		{
			metricText.text = "Show metrics";
			isHidden = true;
		}
	}
}
