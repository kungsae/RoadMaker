using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asd : MonoBehaviour
{
    [SerializeField] [Range(0f, 10f)] private float speed = 1;
    [SerializeField] [Range(0f, 10f)] private float radius = 1;
    public bool A = true;
    public bool B = true;
    public bool C = true;
    public GameObject a;
    public GameObject par;
    private float runningTime = 0f;
    private float yPos = 0f;
     private Vector3 newPos = new Vector3();
    public int q;
    public int w;
    float y = 0;
    float x = 0;
    // Use this for initialization
    private void Update()
	{
 
        runningTime += Time.deltaTime * speed;
        if(A)
        x = par.transform.position.x+ radius * Mathf.Cos(runningTime+q);
        if(B)
        y = par.transform.position.y+ radius * Mathf.Sin(runningTime+w);
        Debug.Log(x+" @ "+y +" @ " + runningTime);
        newPos = new Vector3(x, y,par.transform.position.z);
        if (C)
        {
            GameObject b = Instantiate(a, transform.position, transform.rotation);
            Destroy(b, 2f);
        }

        this.transform.position = newPos;
    }
}
