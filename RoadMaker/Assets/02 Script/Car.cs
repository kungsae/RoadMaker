using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour
{
    public NavMeshAgent nav;
    public Transform trn;
    [SerializeField]LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (Physics.Raycast(transform.position,Vector3.down, out RaycastHit hit, 999f, mask))
        {   
            if (hit.collider.CompareTag("Ground")&& nav.enabled!=false&&GameManager.Instance.isStart)
            {
                GameOver();
            }
        }
    }
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Car"))
		{
			if (nav.enabled != false)
			{
                GameOver();

            }

		}

    }
    private void GameOver()
    {
        if (!GameManager.Instance.isGameOver)
        {
            GameManager.Instance.GameOver();
        }
        nav.ResetPath();
        nav.enabled = false;
    }
	private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.CompareTag("Stop"))
        {
            nav.ResetPath();

        }
        if (other.gameObject.CompareTag("Finish"))
        {
            gameObject.SetActive(false);
        }
    }
	private void OnTriggerExit(Collider other)
	{
        if (other.gameObject.CompareTag("Stop"))
        {
            Debug.Log("다시 출발");
            nav.destination = trn.position;
        }
    }
}
