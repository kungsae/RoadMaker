using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class StageDataList
{
	public List<int> stageStar = new List<int>();
}

public class StageManager : MonoBehaviour
{
	
	public int LastClearStage;
	public StageDataList stageStar = new StageDataList();

	private void Awake()
	{
		for (int i = 0; i < 10; i++)
		{
			stageStar.stageStar.Add(0);
		}
		DontDestroyOnLoad(gameObject);
	}
}
