using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridBuildingSystem : MonoBehaviour
{
	[SerializeField] private Transform defaultRoad;
	[SerializeField] LayerMask tileLayer;
	[SerializeField] LayerMask roadLayer;
	[SerializeField] int gridX;
	[SerializeField] int gridZ;
	public Transform[] roadObj;
	public List<Transform> startObj;
	public List<Transform> carObj;
	public List<CarPos> carPosList = new List<CarPos>();
	public Transform[] ghostTrn;
	public Transform ghost;
	public Camera mainCamera;
	public GameObject ground;
	public float cameraSize;
	public Vector3 groundSize;
	public enum BulidMode
	{
		None,
		Build,
		Rotate,
		Delete
	}
	public BulidMode bulidMode = BulidMode.None;

	private Vector3 cameraPos;
	private Sound sound;
	private GridXZ<GridObject> grid;
	[HideInInspector] public List<Vector3> originPos = new List<Vector3>();
	[HideInInspector] public List<Transform> roads;
	[HideInInspector] public int roadObjIndex = 0;
	[HideInInspector] public int allPrice;
	[HideInInspector] int roadsIndex;
	[HideInInspector] public int dir = 0;

	public class CarPos
	{
		Vector3 pos;
		Vector3 rot;

		public CarPos(Vector3 _pos, Vector3 _rot)
		{
			pos = _pos;
			rot = _rot;
		}
		public Vector3 GetPos()
		{ 
			return pos; 
		}
		public Vector3 GetRot()
		{ 
			return rot; 
		}
	}
	
	private void Awake()
	{
		int girdwidth = gridX;
		int gridHeigth = gridZ;
		float cellSize = 4f;
		sound = FindObjectOfType<Sound>();
		grid = new GridXZ<GridObject>(girdwidth, gridHeigth, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
		roadObj[roadObjIndex].GetComponent<RoadObj>().Rotate(dir);
		ground = gameObject;
		for (int i = 0; i < ghostTrn.Length; i++)
		{
			ghostTrn[i].GetComponent<RoadObj>().Rotate(dir);
		}
	}

	private void Start()
	{
		for (int i = 0; i < carObj.Count; i++)
		{
			var car = new CarPos(carObj[i].position, carObj[i].rotation.eulerAngles);
			carPosList.Add(car);

		}
		foreach (var item in startObj)
		{
			originPos.Add(item.position);
			grid.GetXZ(item.position, out int x, out int z);
			GridObject gridObject = grid.GetGridObject(x, z);
			Transform builtTransform = Instantiate(defaultRoad, grid.GetWorldPosition(x, z), Quaternion.identity);
			gridObject.SetTrn(builtTransform);
		}
		SetText();
		for (int i = 0; i < ghostTrn.Length; i++)
		{
			if (i != roadObjIndex)
			{
				ghostTrn[i].gameObject.SetActive(false);
			}
			else if (i == roadObjIndex)
			{
				ghostTrn[i].gameObject.SetActive(true);
			}
		}
		ghost.transform.position = grid.GetWorldPosition(2, 2);
	}
	public class GridObject
	{
		private GridXZ<GridObject> grid;
		private int x;
		private int z;
		private Transform trn;
		private Transform otherTrn;
		public int price;
		public int otherPrice;

		public GridObject(GridXZ<GridObject> _grid, int _x, int _z)
		{
			grid = _grid;
			x = _x;
			z = _z;
		}
		public void SetTrn(Transform transform)
		{
			trn = transform;
			grid.TriggerGridObjectChanged(x, z);
		}
		public void SetOtherTrn(Transform transform)
		{
			otherTrn = transform;
			grid.TriggerGridObjectChanged(x, z);
		}
		public bool CanBuild()
		{
			return trn == null;
		}
		public bool CanOtherBuild()
		{
			return otherTrn == null && !CanBuild();
		}
		public void ClrearTrn()
		{
			trn = null;
			grid.TriggerGridObjectChanged(x, z);
		}
		public void ClrearOtherTrn()
		{
			otherTrn = null;
			grid.TriggerGridObjectChanged(x, z);
		}
	}
	private void Update()
	{
		if (GameManager.Instance.isStart)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			RotateRoad();
		}
#if UNITY_ANDROID

		if(Input.GetMouseButton(0))
			Ghost();
		if (Input.GetMouseButtonUp(0)&&!EventSystem.current.IsPointerOverGameObject()&& GetMouseWorldPosition() != Vector3.zero)
		{
			grid.GetXZ(GetMouseWorldPosition(), out int x, out int z);
			GridObject gridObject = grid.GetGridObject(x, z);
			TouchEvent(gridObject);
		}
#else
		Ghost();
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()&& GetMouseWorldPosition() != Vector3.zero)
		{
			grid.GetXZ(GetMouseWorldPosition(), out int x, out int z);
			GridObject gridObject = grid.GetGridObject(x, z);
			if (gridObject.CanBuild() || (gridObject.CanOtherBuild() && roadObjIndex == 4))
			{
				InstantiateRoad();
			}
			else if ((!gridObject.CanBuild() && roadObjIndex != 4) || !gridObject.CanOtherBuild())
			{
				DelRoad();
			}
		}

#endif

	}
	public void TouchEvent(GridObject gridObject)
	{
        switch (bulidMode)
        {
            case BulidMode.None:
                break;
            case BulidMode.Build:
				if (gridObject.CanBuild() || (gridObject.CanOtherBuild() && roadObjIndex == 4))
				{
					InstantiateRoad();
				}
                break;
            case BulidMode.Rotate:
				RotateRoad();
                break;
            case BulidMode.Delete:
				if ((!gridObject.CanBuild() && roadObjIndex != 4) || !gridObject.CanOtherBuild())
				{
					DelRoad();
				}
				break;
        }
    }
	public void InstantiateRoad()
	{
		grid.GetXZ(ghost.position, out int x, out int z);
		GridObject gridObject = grid.GetGridObject(x, z);
		if (!GetGhostCol().CompareTag("Cant"))
		{
			if (gridObject.CanBuild() && roadObjIndex != 4)
			{
				Transform builtTransform = Instantiate(roadObj[roadObjIndex], ghost.transform.position, Quaternion.identity);
				gridObject.SetTrn(builtTransform);
				roads.Add(builtTransform);
				gridObject.price = (roadObjIndex + 1) * 100;
				allPrice += gridObject.price;
				SetText();
				sound.playSound(0);
			}
			else if (gridObject.CanOtherBuild() && roadObjIndex == 4)
			{
				Transform builtTransform = Instantiate(roadObj[roadObjIndex], ghost.transform.position, Quaternion.identity);
				gridObject.SetOtherTrn(builtTransform);
				roads.Add(builtTransform);
				gridObject.otherPrice = 300;
				allPrice += gridObject.otherPrice;
				SetText();
				sound.playSound(0);
			}
		}	
	}
	public void DelRoad()
	{
		grid.GetXZ(ghost.position, out int x, out int z);
		GridObject gridObject = grid.GetGridObject(x, z);
		Ray ray = new Ray(ghost.transform.position + new Vector3(2, 1, 2), Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hit, 999f, roadLayer))
		{
			Collider builtObj = hit.collider;

			if (hit.collider.transform.position == grid.GetWorldPosition(x, z))
			{
				if  (gridObject.CanOtherBuild())
				{
					allPrice -= gridObject.price;
					gridObject.ClrearTrn();
				}
				else
				{
					allPrice -= gridObject.otherPrice;
					gridObject.ClrearOtherTrn();
				}
				roads.Remove(builtObj.gameObject.transform);
				Destroy(builtObj.gameObject);
				sound.playSound(1);
				roadsIndex = roads.Count;
				SetText();
			}

		}
	}
	public void RotateRoad()
	{
#if UNITY_ANDROID
		if (GetGhostRoadCol() == null) return;
		RoadObj road = GetGhostRoadCol().GetComponent<RoadObj>();
		dir = (int)(road.GetRotate()/ 90);
		dir++;
		dir %= 4;
		road.Rotate(dir);
		for (int i = 0; i < ghostTrn.Length; i++)
		{
			ghostTrn[i].GetComponent<RoadObj>().Rotate(dir);
		}
#else
		dir++;
		dir %= 4;
		roadObj[roadObjIndex].GetComponent<RoadObj>().Rotate(dir);
		for (int i = 0; i < ghostTrn.Length; i++)
		{
			ghostTrn[i].GetComponent<RoadObj>().Rotate(dir);
		}
#endif
	}
	private Vector3 GetMouseWorldPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, 999f, tileLayer))
		{
			return hit.point;
		}
		else return Vector3.zero;
	}
	private Collider GetMouseCol()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, 999f, tileLayer))
		{
			return hit.collider;
		}
		else return null;
	}
	private Collider GetGhostCol()
	{
		Ray ray = new Ray(ghost.transform.position + new Vector3(2, 1, 2), Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hit, 999f, tileLayer))
		{
			return hit.collider;
		}
		else return null;
	}
	private Collider GetGhostRoadCol()
	{
		Ray ray = new Ray(ghost.transform.position + new Vector3(2, 1, 2), Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hit, 999f, roadLayer))
		{
			return hit.collider;
		}
		else return null;
	}
	public void Ghost()
	{
		grid.GetXZ(GetMouseWorldPosition(), out int x, out int z);
		for (int i = 0; i < ghostTrn.Length; i++)
		{
			if (i != roadObjIndex)
			ghostTrn[i].gameObject.SetActive(false);
			else if (i == roadObjIndex)
				ghostTrn[i].gameObject.SetActive(true);
		}
		if (GetMouseWorldPosition() != Vector3.zero)
			ghost.transform.position = grid.GetWorldPosition(x, z);


	}
	private void SetText()
	{
		GameManager.Instance.priceText.text = $"{allPrice}/{GameManager.Instance.maxPrice}";
		if (GameManager.Instance.maxPrice < allPrice)
		{
			GameManager.Instance.priceText.color = Color.red;
		}
		else
		{
			GameManager.Instance.priceText.color = Color.white;
		}
	}
}