using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shield : MonoBehaviour
{
    public MeshRenderer SheidMesh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            ShieldAppear();
        }
        if(Input.GetKeyUp(KeyCode.R))
        {
            ShieldDisappear();
        }
        
    }
    public void ShieldAppear()
    {
      SheidMesh.material.DOFloat(2f, "_Appear", 1f).SetEase(Ease.InSine,2);  
    }
    public void ShieldDisappear()
    {
       SheidMesh.material.DOFloat(0f, "_Appear", 1f).SetEase(Ease.InSine,2);  
    }
}
