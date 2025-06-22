using UnityEngine;
using DG.Tweening;

public class BloodBomb : MonoBehaviour
{
    public GameObject SphereBomb;
    public GameObject InkMesh;
    public ParticleSystem InkBlast;
    public ParticleSystem Inksplash;
    
    public void Blast()
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
