using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour {

	public Text tableNumber;
	public InputField inputText;
	public Text metricText;
	public UnityEngine.UI.Slider numberOfTableSlider;
	public GameObject ahoj;

	// Use this for initialization
	void Start () {
		//inputText.enabled(false);
		//inputText.enabled = false;
		inputText.gameObject.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	GameObject clickable;
	public void HideOrUnihdeInputField(bool hide)
	{
		if (hide)
			inputText.gameObject.SetActive(false);
		else
			inputText.gameObject.SetActive(true);
	}
	public void SetInputFieldText(string defaultText)
	{
		inputText.text = defaultText;
	}

	public string GetInputFieldText()
	{
		//Debug.Log(textfield.collider.transform.textI)
		return inputText.text;
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
