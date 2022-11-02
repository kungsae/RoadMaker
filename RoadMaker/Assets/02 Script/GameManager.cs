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

    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject[] button;
    [SerializeField] private GameObject nextStageButton;
    [SerializeField] private Text[] timeText;
    [SerializeField] private float[] timeLimit;
    [SerializeField] NavMeshSurface[] nav;
    [SerializeField] Image[] star;
    [SerializeField] Button[] roadSet;
    [SerializeField] Sprite[] starImage;
    [SerializeField] Text[] timeLimitText;
    [SerializeField] private int stageNum = 1;
    [SerializeField] private Text clearChcekText;
    public Text priceText;
    public int maxPrice;
    public bool isStart = false;
    public bool isGameOver = false;
    private GridBuildingSystem grid;
    private bool isClear = false;
    private bool completed = true;
    private float sec = 0;
    private float min = 0;
    StageManager stageManager;
    Sound sound;
    Fade fade;


    void Start()
    {
        sound = FindObjectOfType<Sound>();
        fade = FindObjectOfType<Fade>();
        grid = FindObjectOfType<GridBuildingSystem>();
        stageManager = FindObjectOfType<StageManager>();
#if UNITY_ANDROID
        roadSet[0].onClick.AddListener(() =>
        {
            grid.bulidMode = GridBuildingSystem.BulidMode.Build;
            ButtonInteract(0);
        });
        roadSet[1].onClick.AddListener(() =>
        {
            grid.bulidMode = GridBuildingSystem.BulidMode.Rotate;

            ButtonInteract(1);

        });
        roadSet[2].onClick.AddListener(() =>
        {
            grid.bulidMode = GridBuildingSystem.BulidMode.Delete;
            ButtonInteract(2);
        });
#else
        for (int i = 0; i < 3; i++)
        {
            roadSet[i].gameObject.SetActive(false);
        }
#endif
        for (int i = 0; i < 3; i++)
		{
            timeLimitText[i].text = $"{timeLimit[i].ToString("00.00")}";
            star[i].sprite = starImage[0];
        }
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
        if (!isStart)
        {
            sound.playSound(4);
            if (maxPrice >= grid.allPrice)
            {
                isStart = true;
                Time.timeScale = 1;
                for (int i = 0; i < button.Length; i++)
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
                        _car.SetRigidbody(false);
                    }
                }

                for (int i = 0; i < grid.ghostTrn.Length; i++)
                {
                    grid.ghostTrn[i].gameObject.SetActive(false);
                }
            }
        }
        else if (isStart)
        {
            EndGame();
        }
      
    }
    public void EndGame()
    {
        //SceneManager.LoadScene("Stage" + stageNum);
        sound.playSound(4);
        isStart = false;
		isClear = false;
        completed = true;
        isGameOver = false;
        for (int i = 0; i < grid.carObj.Count; i++)
        {
            Car c = grid.carObj[i].GetComponent<Car>();
            NavMeshAgent car = c.nav;
            c.SetRigidbody(true);
            car.gameObject.SetActive(true);
            if (car.destination != null&&car.enabled ==true)
            {
                car.isStopped = true;
                car.ResetPath();
            }

            grid.carObj[i].position = grid.carPosList[i].GetPos();
            grid.carObj[i].eulerAngles = grid.carPosList[i].GetRot();
            car.velocity = new Vector3(0, 0, 0);
            
            //car.Stop();
        }
        sec = 0;
		min = 0;    
		clearUI.gameObject.SetActive(false);
		timeText[0].gameObject.SetActive(true);
		timeText[1].gameObject.SetActive(true);
        //nextStageButton.transform.localScale = new Vector3(1, 1);
        timeText[0].text = $"{min.ToString("00")}:{(sec % 60).ToString("00.00")}";
		for (int i = 0; i < button.Length; i++)
		{
			button[i].SetActive(true);
		}
		
		for (int i = 0; i < grid.startObj.Count; i++)
		{
			if (grid.startObj[i].GetComponent<Car>() != null)
			{
				Car _car = grid.startObj[i].GetComponent<Car>();
				_car.nav.enabled = true;
			}
		}
        grid.ghostTrn[grid.roadObjIndex].gameObject.SetActive(true);
       

    }
    public void ResetGame()
    {
        sound.playSound(4);
        StartCoroutine(fade.FadeOut("Stage",stageNum));
    }
    public void RoadUI(int num)
    {
        grid.roadObjIndex = num;
        grid.roadObj[grid.roadObjIndex].GetComponent<RoadObj>().Rotate(grid.dir);
		for (int i = 0; i < grid.ghostTrn.Length; i++)
		{
            grid.ghostTrn[i].GetComponent<RoadObj>().Rotate(grid.dir);
        }
        grid.Ghost();
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
        if (stageManager != null)
        {
            if (isClear && stageManager.stageStar.LastClearStage < stageNum + 1)
            {
                stageManager.stageStar.LastClearStage = stageNum + 1;
            }
        }
        clearUI.gameObject.SetActive(true);
        clearUI.transform.localPosition = new Vector3(0, -650);
        clearUI.transform.DOLocalMoveY(0, 0.8f).SetEase(Ease.OutSine);
        clearChcekText.text = "Clear";
        clearChcekText.color = Color.yellow;
        timeText[1].text = $"{min.ToString("00")}:{(sec % 60).ToString("00.00")}";
        nextStageButton.SetActive(true);
        int starCount = 0;
		for (int i = 0; i < 3; i++)
		{
            if (timeLimit[i] > sec)
			{
                starCount++;
                star[i].sprite = starImage[1];
            }
		}
        if (stageManager != null)
        {
            if (stageManager.stageStar.stageStar[stageNum - 1] < starCount)
                stageManager.stageStar.stageStar[stageNum - 1] = starCount;
        }
        sound.playSound(2);
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
        sound.playSound(3);
    }
	public void BackToStage()
    {
        if (isClear && stageManager.stageStar.LastClearStage < stageNum + 1)
        {
            stageManager.stageStar.LastClearStage = stageNum+1;
        }
        sound.playSound(4);
        StartCoroutine(fade.FadeOut("Stage"));
    }
    public void NextStage()
    {
        if (isClear && stageManager.stageStar.LastClearStage < stageNum + 1)
        {
            stageManager.stageStar.LastClearStage = stageNum + 1;
        }
        sound.playSound(4);
        StartCoroutine(fade.FadeOut("Stage",(stageNum+1)));
    }
    public void ButtonInteract(int idx)
    {
        for (int i = 0; i < roadSet.Length; i++)
        {
            roadSet[i].interactable = true;
        }
        roadSet[idx].interactable = false;
    }
}
