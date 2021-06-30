using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Main : MonoBehaviour
{
	[SerializeField] GameObject title;
	[SerializeField] GameObject button;
	AudioSource audio;
	Fade fade;
	public AudioClip[] sound;
	Sound _sound;
	public void GameStart()
	{

		_sound.playSound(4);
		StartCoroutine(fade.FadeOut("Stage"));
		
	}
	private void Start()
	{
		_sound = FindObjectOfType<Sound>();
		fade = FindObjectOfType<Fade>();
		audio = GetComponent<AudioSource>();
		StartCoroutine(TilteStart());

		button.transform.DOScale(button.transform.localScale + new Vector3(0.1f,0.1f), 0.5f).SetLoops(-1, LoopType.Yoyo);
	}
	IEnumerator TilteStart()
	{
		yield return new WaitForSeconds(0.5f);
		audio.clip = sound[0];
		audio.Play();
		title.transform.DOLocalMoveY(60f, 1.1f).SetEase(Ease.InExpo);
		yield return new WaitForSeconds(1.1f);
		title.transform.DOShakeRotation(0.2f,5f);
		audio.clip = sound[1];
		audio.Play();
		button.SetActive(true);
	}
}
