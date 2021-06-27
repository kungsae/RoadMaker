using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
	[SerializeField] List<Button> stageButton;
	[SerializeField] List<GameObject> lockImage;
	StageManager stageManager;
	public int maxOpenStage = 1;
	[SerializeField] private List<Star>stars = new List<Star>();
	public GameObject panel;
	private void Start()
	{
		stageManager = FindObjectOfType<StageManager>();
		if (maxOpenStage <= stageManager.LastClearStage)
		{
			maxOpenStage = stageManager.LastClearStage;
			
		}
		for (int i = 0; i < stageButton.Count; i++)
		{
			int a = i;
			stageButton[i].onClick.AddListener(() => { stageStart(a + 1); });
			stars[i].Init(true);

		}
		for (int i = 0; i < maxOpenStage; i++)
		{
			//lockImage[i].SetActive(false);
			stars[i].Init(false,stageManager.stageDatas[i].stageStar);
		}
		


	}
	private void stageStart(int stageLevel)
	{
		if (maxOpenStage >= stageLevel)
			SceneManager.LoadScene("Stage" + stageLevel);
	}
	public void onHelp(bool on)
	{
		panel.gameObject.SetActive(on);
	}
}
