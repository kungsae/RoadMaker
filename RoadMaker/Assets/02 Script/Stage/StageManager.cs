using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
	
	public int LastClearStage;
	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
}
