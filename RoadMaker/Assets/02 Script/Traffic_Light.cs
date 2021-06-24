using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Traffic_Light : MonoBehaviour
{
	private int delay = 3;
	[SerializeField] GameObject traffic;
	[SerializeField] Material[] trafficLight;
	[SerializeField] private MeshRenderer nowLight;
	[SerializeField] GameObject UIPrefab;
	[SerializeField] private Text delayText;
	private float time = 0f;
	private void Start()
	{
		//StartCoroutine(Traffic());
		UIPrefab.transform.position = Camera.main.WorldToScreenPoint(transform.position);
		delayText = UIPrefab.GetComponentInChildren<Text>();
		delayText.text = $"{delay}s";

	}
	private void Update()
	{
		if (GameManager.Instance.isStart)
		{
			UIPrefab.SetActive(false);
			time += Time.deltaTime;
			if (time >= delay)
			{
				traffic.transform.localPosition = new Vector3(0, 10, 0);
				nowLight.material = trafficLight[1];
				if (time >= delay * 2)
				{
					time = 0;
				}
			}
			else
			{
				traffic.transform.localPosition = new Vector3(0, 0, 0);
				nowLight.material = trafficLight[0];
			}
		}
		else
		{
			UIPrefab.SetActive(true);
			time = 0;
			traffic.transform.localPosition = new Vector3(0, 0, 0);
		}
	}
	IEnumerator Traffic()
	{
		while (true)
		{
			if (GameManager.Instance.isStart)
			{
				UIPrefab.SetActive(false);
				yield return new WaitForSeconds(delay);
				traffic.transform.localPosition = new Vector3(0, 10, 0);
				nowLight.material = trafficLight[1];
				yield return new WaitForSeconds(delay);
				traffic.transform.localPosition = new Vector3(0, 0, 0);
				nowLight.material = trafficLight[0];
			}
			yield return null;
		}
	}
	public void DelayButton(int input)
	{
		if (delay >= 1&&delay<100)
		{
			delay+= input;
		}
		if (delay == 0)
		{
			delay = 1;
		}
		if (delay == 100)
		{
			delay = 99;
		}
		delayText.text = $"{delay}s";
	}
}
