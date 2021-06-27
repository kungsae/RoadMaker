using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
	
	[SerializeField] Image[] starImage;
	[SerializeField] Image lockImage;
	[SerializeField] Sprite[] onStar;
	public void Init(bool isLock,int star)
	{
		if (isLock)
		{
			lockImage.gameObject.SetActive(true);
			for (int i = 0; i < starImage.Length; i++)
			{
				starImage[i].gameObject.SetActive(false);
			}
		}
		else
		{
			lockImage.gameObject.SetActive(false);
			for (int i = 0; i < starImage.Length; i++)
			{
				starImage[i].gameObject.SetActive(true);
				starImage[i].sprite = onStar[0];
			}
			for (int i = 0; i < star; i++)
			{
				starImage[i].sprite = onStar[1];
			}
		}
	}
	public void Init(bool isLock)
	{
		if (isLock)
		{
			lockImage.gameObject.SetActive(true);
			for (int i = 0; i < starImage.Length; i++)
			{
				starImage[i].gameObject.SetActive(false);
			}
		}
		else
		{
			lockImage.gameObject.SetActive(false);
			for (int i = 0; i < starImage.Length; i++)
			{
				starImage[i].gameObject.SetActive(true);
			}
		}
	}
}
