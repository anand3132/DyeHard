using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum PanelID
// {
//     id1,
//     id2,
//     id3,
//     id4
// }

public class Test : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    private int counter = 0;
    public int currentSelected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void OnclickCell(int id)
    {
        if (counter == 0)
        {
            counter++;
            currentSelected = id;
            return;
        }
        else
        {
            if (currentSelected == id)
            {
                Debug.Log("remove");
            }
            else
            {
                //reset
            }
        }

    }
}
