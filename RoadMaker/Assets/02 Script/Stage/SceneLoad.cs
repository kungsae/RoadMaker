using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
	[SerializeField] List<Button> stageButton;
	[SerializeField] List<GameObject> lockImage;
	StageManager stageManager;
	private int totalStar = 0;
	private int maxOpenStage = 1;
	[SerializeField] private List<Star>stars = new List<Star>();
	private List<int> starCount = new List<int>();
	public GameObject panel;

	private void Start()
	{
		stageManager = FindObjectOfType<StageManager>();

		for (int i = 0; i < stageManager.stageStar.stageStar.Count; i++)
		{
			starCount.Add(stageManager.stageStar.stageStar[i]);
		}

		if (maxOpenStage <= stageManager.stageStar.LastClearStage)
		{
			maxOpenStage = stageManager.stageStar.LastClearStage;
			
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
			stars[i].Init(false, starCount[i]);
			totalStar += starCount[i];
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
