using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<GameManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newStageManagerton = new GameObject("GameManager").AddComponent<GameManager>();
                    instance = newStageManagerton;
                }
            }
            return instance;
        }
    }

    public bool isStart = false;
    private bool isClear = false;
    private bool completed = true;
    [SerializeField] private GameObject clearUI;
    [SerializeField]
    NavMeshSurface[] nav;
    private GridBuildingSystem grid;
    [SerializeField] private GameObject[] button;
    public int maxPrice;
    public Text priceText;
    [SerializeField] private Text[] timeText;
    [SerializeField] private Text clearChcekText;
    private float sec = 0;
    private float min = 0;
    private float tiem = 0;
    [SerializeField] private int stageNum = 1;
    StageManager stageManager;
    public bool isGameOver = false;
    [SerializeField] private float[] timeLimit;
    void Start()
    {
        grid = FindObjectOfType<GridBuildingSystem>();
        stageManager = FindObjectOfType<StageManager>();
        Time.timeScale = 0;
    }
	private void Update()
	{
        if (isStart)
        {
            ClearCheck();
            if (!isClear)
                Timer();
        }     
    }
	public void StartGame()
    {
        if (maxPrice >= grid.allPrice)
        {
            isStart = true;
            Time.timeScale = 1;
            for (int i = 0; i < button.Length-1; i++)
            {
                button[i].SetActive(false);
            }
            for (int i = 0; i < nav.Length; i++)
            {
                nav[i].BuildNavMesh();
            }

            for (int i = 0; i < grid.startObj.Count; i++)
            {
                if (grid.startObj[i].GetComponent<Car>() != null)
                {
                    Car _car = grid.startObj[i].GetComponent<Car>();
                    _car.nav.enabled = false;
                    _car.nav.enabled = true;
                    _car.nav.destination = _car.trn.position;

                }
            }

            grid.ghostTrn.gameObject.SetActive(false);
        }
    }
    public void EndGame()
    {
		//SceneManager.LoadScene("Stage" + stageNum);
		isStart = false;
		isClear = false;
        completed = true;
        isGameOver = false;
        sec = 0;
		min = 0;
		clearUI.gameObject.SetActive(false);
		timeText[0].gameObject.SetActive(true);
		timeText[1].gameObject.SetActive(true);
		button[button.Length - 1].transform.localScale = new Vector3(1, 1);
		timeText[0].text = $"{min.ToString("00")}:{(sec % 60).ToString("00.00")}";
		for (int i = 0; i < button.Length - 1; i++)
		{
			button[i].SetActive(true);
		}
		for (int i = 0; i < grid.carObj.Count; i++)
		{
			grid.carObj[i].gameObject.SetActive(true);
			grid.carObj[i].position = grid.carPosList[i].GetPos();
			grid.carObj[i].eulerAngles = grid.carPosList[i].GetRot();
		}
		for (int i = 0; i < grid.startObj.Count; i++)
		{
			if (grid.startObj[i].GetComponent<Car>() != null)
			{
				Car _car = grid.startObj[i].GetComponent<Car>();
				_car.nav.enabled = true;
				_car.nav.ResetPath();
			}
		}
		Time.timeScale = 0;
		grid.ghostTrn.gameObject.SetActive(true);
	}
    public void ResetGame()
    {
         SceneManager.LoadScene("Stage"+stageNum);
    }
    public void RoadUI(int num)
    {
        grid.roadObjIndex = num;
        grid.roadObj[grid.roadObjIndex].GetComponent<RoadObj>().Rotate(grid.dir);
        grid.ghostTrn.GetComponent<RoadObj>().Rotate(grid.dir);
    }
    public void ClearCheck()
    {
        int lenth = grid.startObj.Count;
        int carLenth = grid.carObj.Count;
        int check = 0;
        for (int i = 0; i < lenth; i++)
		{
            if (grid.startObj[i] == null)
            {
                grid.startObj.RemoveAt(i);
                break;
            }
		}
        for (int i = 0; i < carLenth; i++)
        {
            if (grid.carObj[i] != grid.carObj[i].gameObject.activeSelf)
            {
                check++;
            }
        }
        
        if (isStart && check == grid.carObj.Count)
        {
            isClear = true;
        }
        if (isClear&& completed)
        {
            Clear();
        }
    }
    private void Clear()
    {
        if (isClear&&stageManager.LastClearStage < stageNum + 1)
        {
            stageManager.LastClearStage = stageNum + 1;
        }
        clearUI.gameObject.SetActive(true);
        clearUI.transform.localPosition = new Vector3(0, -650);
        clearUI.transform.DOLocalMoveY(0, 0.8f).SetEase(Ease.OutSine);
        clearChcekText.text = "Clear";
        clearChcekText.color = Color.yellow;
        timeText[1].text = $"{min.ToString("00")}:{(sec % 60).ToString("00.00")}";
        button[button.Length - 1].SetActive(true);
        int starCount = 0;
		for (int i = 0; i < 3; i++)
		{
			if (timeLimit[i] > sec)
			{
                starCount++;
            }
		}
        if (stageManager.stageDatas[stageNum - 1].stageStar < starCount)
            stageManager.stageDatas[stageNum - 1].stageStar = starCount;
        completed = false;
    }
    private void Timer()
    {
        sec += Time.deltaTime;
        min = (int)sec / 60;
        timeText[0].text = $"{min.ToString("00")}:{(sec % 60).ToString("00.00")}";
    }
    public void GameOver()
    {
        clearUI.gameObject.SetActive(true);
        clearUI.transform.localPosition = new Vector3(0, -650);
        isGameOver = true;
        clearUI.transform.DOLocalMoveY(0, 0.8f).SetEase(Ease.OutSine);
        clearChcekText.text = "Fail";
        clearChcekText.color = Color.red;
        timeText[1].gameObject.SetActive(false);
        button[button.Length - 1].SetActive(false);
    }
	public void BackToStage()
    {
        if (isClear && stageManager.LastClearStage < stageNum + 1)
        {
            stageManager.LastClearStage = stageNum+1;
        }
        SceneManager.LoadScene("Stage");
    }
    public void NextStage()
    {
        if (isClear && stageManager.LastClearStage < stageNum + 1)
        {
            stageManager.LastClearStage = stageNum + 1;
        }
        SceneManager.LoadScene("Stage" + (stageNum + 1));
    }
}