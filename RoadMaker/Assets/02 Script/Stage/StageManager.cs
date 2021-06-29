using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

[Serializable]
public class StageDataList
{
	public List<int> stageStar = new List<int>();
	public int LastClearStage;
}

public class StageManager : MonoBehaviour
{

	public StageDataList stageStar = new StageDataList();

	private void Awake()
	{
		
		for (int i = 0; i < 10; i++)
		{
			stageStar.stageStar.Add(0);
		}
		DontDestroyOnLoad(gameObject);
		Load();
	}
	private void OnApplicationQuit()
	{
		Save();
	}

	public void Save()
	{
		string json = JsonUtility.ToJson(stageStar);
		Debug.Log(json);
		File.WriteAllText(Application.persistentDataPath + "/data.json", json);
	}
	public void Load()
	{
		if (File.Exists(Application.persistentDataPath + "/data.json"))
		{
			string json = File.ReadAllText(Application.persistentDataPath + "/data.json");
			StageDataList list = JsonUtility.FromJson<StageDataList>(json);
			stageStar.stageStar = list.stageStar;
			stageStar.LastClearStage = list.LastClearStage;
		}
	}
}
