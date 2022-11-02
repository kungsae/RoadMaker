using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObj : MonoBehaviour
{
    [SerializeField] Transform Prefab;
	public void Rotate(int dir)
	{
		dir *= 90;
		Prefab.transform.rotation = Quaternion.Euler(0,dir, 0);
	}
	public int GetRotate()
	{
		return (int)Prefab.transform.rotation.eulerAngles.y;
	}
}
