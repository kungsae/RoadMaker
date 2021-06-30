using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
	private Image fadePanel;
	private void Start()
	{
		fadePanel = GetComponent<Image>();
		fadePanel.DOFade(0, 0.3f);
	}
	public IEnumerator FadeOut(string SceneName,int SceneNum)
	{
		fadePanel.DOFade(1, 0.3f);
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(SceneName + SceneNum);
	}
	public IEnumerator FadeOut(string SceneName)
	{
		fadePanel.DOFade(1, 0.3f);
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(SceneName);
	}
}
