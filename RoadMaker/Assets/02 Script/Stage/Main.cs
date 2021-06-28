using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Main : MonoBehaviour
{
	[SerializeField] GameObject title;
	[SerializeField] GameObject button;
	public void GameStart()
	{
		SceneManager.LoadScene("Stage");
	}
	private void Start()
	{
		button.transform.DOScale(button.transform.localScale + new Vector3(0.1f,0.1f), 0.5f).SetLoops(-1, LoopType.Yoyo);
	}
}
