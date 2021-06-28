using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridBuildingSystem : MonoBehaviour
{
	public Transform[] roadObj;
	[SerializeField] private Transform defaultRoad;
	public List<Vector3> originPos = new List<Vector3>();
	public List<Transform> roads;
	public List<Transform> startObj;
	public List<Transform> carObj;
	public List<CarPos> carPosList = new List<CarPos>();
	public Transform ghostTrn;

	private GridXZ<GridObject> grid;
	[SerializeField] LayerMask tileLayer;
	[SerializeField] LayerMask roadLayer;
	public int roadObjIndex = 0;
	public int allPrice;

	int roadsIndex;
	[HideInInspector] public int dir = 0;

	[SerializeField] int gridX;
	[SerializeField] int gridZ;

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
		{ return pos; }
		public Vector3 GetRot()
		{ return rot; }
	}
	private void Awake()
	{
		int girdwidth = gridX;
		int gridHeigth = gridZ;
		float cellSize = 4f;
		grid = new GridXZ<GridObject>(girdwidth, gridHeigth, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
		roadObj[roadObjIndex].GetComponent<RoadObj>().Rotate(dir);
		ghostTrn.GetComponent<RoadObj>().Rotate(dir);
		
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
		Ghost();
		if (Input.GetMouseButtonDown(0))
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				grid.GetXZ(GetMouseWorldPosition(), out int x, out int z);
				GridObject gridObject = grid.GetGridObject(x, z);
				if (GetMouseWorldPosition() != Vector3.zero&& !GetMouseCol().CompareTag("Cant"))
				{
					
					if (gridObject.CanBuild() && roadObjIndex != 4)
					{
						Transform builtTransform = Instantiate(roadObj[roadObjIndex], grid.GetWorldPosition(x, z), Quaternion.identity);
						gridObject.SetTrn(builtTransform);
						roads.Add(builtTransform);
						gridObject.price = (roadObjIndex + 1) * 100;
						allPrice += gridObject.price;
						SetText();
					}
					if (gridObject.CanOtherBuild() && roadObjIndex == 4)
					{
						Transform builtTransform = Instantiate(roadObj[roadObjIndex], grid.GetWorldPosition(x, z), Quaternion.identity);
						gridObject.SetOtherTrn(builtTransform);
						roads.Add(builtTransform);
						gridObject.otherPrice = 300;
						allPrice += gridObject.otherPrice;
						SetText();
					}
				}
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				grid.GetXZ(GetMouseWorldPosition(), out int x, out int z);
				GridObject gridObject = grid.GetGridObject(x, z);
				if (!gridObject.CanBuild())
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out RaycastHit hit, 999f, roadLayer))
					{
						Collider builtObj = hit.collider;	

						if (gridObject.CanOtherBuild())
						{
							allPrice -= gridObject.price;
						}
						else
						{
							allPrice -= gridObject.otherPrice;
						}
						if (builtObj.CompareTag("Stop"))
						{
							gridObject.ClrearOtherTrn();
						}
						else
						{
							gridObject.ClrearTrn();
						}
						Destroy(builtObj.gameObject);
						roadsIndex = roads.Count;
						SetText();
						for (int i = 0; i < roadsIndex; i++)
						{
							if (roads[i] == null)
							{
								roads.RemoveAt(i);
								break;
							}

						}
					}

				}
			}
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			dir++;
			dir %= 4;
			roadObj[roadObjIndex].GetComponent<RoadObj>().Rotate(dir);
			ghostTrn.GetComponent<RoadObj>().Rotate(dir);
		}

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
	private void Ghost()
	{
		grid.GetXZ(GetMouseWorldPosition(), out int x, out int z);
		if (GetMouseWorldPosition() != Vector3.zero)
			ghostTrn.position = grid.GetWorldPosition(x, z);

	}
	private void SetText()
	{
		GameManager.Instance.priceText.text = $"{allPrice}/{GameManager.Instance.maxPrice}";
		if (GameManager.Instance.maxPrice < allPrice)
		{
			GameManager.Instance.priceText.color = Color.red;
		}
		else
			GameManager.Instance.priceText.color = Color.white;
	}
	public void Init()
	{
		//for (int i = 0; i < roads.Count; i++)
		//{
		//	if(roads[i]!=null)
		//	Destroy(roads[i].gameObject);
		//}
		//for (int i = 0; i < startObj.Count; i++)
		//{
		//	startObj[i].position = originPos[i];
		//}

		//roads.Clear();

	}
}