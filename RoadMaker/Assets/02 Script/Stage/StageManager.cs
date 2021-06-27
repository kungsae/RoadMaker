using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
	
	public int LastClearStage;
	public class StageData
	{
		public int stageLevel;
		public int stageStar = 0; 
	}
	public List<StageData> stageDatas = new List<StageData>();
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			stageDatas.Add(new StageData());
		}
		DontDestroyOnLoad(gameObject);
	}
	public void save()
	{
		
	}
}
