using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InkBomb : MonoBehaviour
{
    public GameObject SphereBomb;
    public GameObject InkMesh;
    public ParticleSystem InkBlast;
    public ParticleSystem Inksplash;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
           InkBlasting(); 
        }
        
    }
    void InkBlasting()
    {
        SphereBomb.transform.DOScale(0.5f, 0.1f).OnComplete(() => 
        {
         SphereBomb.GetComponent<MeshRenderer>().material.DOFloat(4.5f, "_Displacement_Strength", 0.5f);
         InkMesh.transform.DOScale(3f, 0.45f);
         InkMesh.GetComponent<MeshRenderer>().material.DOFloat(2f, "_Displacement_Strength", 0.5f);
         Invoke("InkParticle",0.275f);
         Invoke("InksplashParticle",0.15f); 
         SphereBomb.GetComponent<MeshRenderer>().material.DOFloat(1f, "_Noise_Appear", 0.5f).SetDelay(0.2f);
         InkMesh.GetComponent<MeshRenderer>().material.DOFloat(4f, "_Noise_Appear", 0.5f).SetDelay(0.2f);

        });
     
       
    }
    void InkParticle()
    {
         InkBlast.Play();
    }
    void InksplashParticle()
    {
        Inksplash.Play();
    }
         

}
