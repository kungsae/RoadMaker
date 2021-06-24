using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour
{
    [SerializeField] Image[] imange;
    public Sprite[] leftImage;
    public Sprite[] rightImage;
    public int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void button(int _index)
    {
        index += _index;
        if (index == rightImage.Length)
        {
            index = rightImage.Length - 1;
        }
        if (index == -1)
        {
            index = 0;
        }
        imange[0].sprite = leftImage[index];
        imange[1].sprite = rightImage[index];

    }
}
