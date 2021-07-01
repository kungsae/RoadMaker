using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoad : MonoBehaviour
{
	[SerializeField] List<Button> stageButton;
	[SerializeField] List<GameObject> lockImage;
	StageManager stageManager;
	private int totalStar = 0;
	private int maxOpenStage = 1;
	[SerializeField] private List<Star> stars = new List<Star>();
	private List<int> starCount = new List<int>();
	public GameObject panel;
	public GameObject exitPanel;
	Sound sound;
	Fade fade;

	private void Start()
	{
		fade = FindObjectOfType<Fade>();
		sound = FindObjectOfType<Sound>();
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
		{
			sound.playSound(4);
			StartCoroutine(fade.FadeOut("Stage", stageLevel));
		}
			
	}
	public void onHelp(bool on)
	{
		sound.playSound(4);
		panel.gameObject.SetActive(on);
	}
	public void OnExitUI()
	{
		sound.playSound(4);
		exitPanel.transform.DOScale(0, 0);
		exitPanel.transform.DOScale(1, 0.2f);
	}
	public void OffExitUI()
	{
		sound.playSound(4);
		exitPanel.transform.DOScale(1, 0);
		exitPanel.transform.DOScale(0, 0.2f);
	}
	public void ExitGame()
	{
#if UNITY_EDITOR
		sound.playSound(4);
		UnityEditor.EditorApplication.isPlaying = false;
#else
sound.playSound(4);
        Application.Quit();
#endif
	}
}
