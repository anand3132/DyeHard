using System.Collections;
using UnityEngine;
using DG.Tweening;
using PaintIn3D;

namespace RedGaint
{

    public class BombTrigger : MonoBehaviour
    {
        public GameObject SphereBomb;
        public GameObject InkMesh;
        public ParticleSystem InkBlast;
        public ParticleSystem Inksplash;
        public GameObject sphereInk;
        public GameObject bombEffect;
        public void SetBombFor(GlobalEnums.GameTeam team)
        {
            TeamData tData = TeamManager.Instance.GetTeamData(team);
            Color bombcolor = tData.TeamColor;
            sphereInk.GetComponent<CwPaintDecal>().Color = bombcolor;
        }

        public void InkBlasting()
        {
            sphereInk.transform.SetParent(null);
            bombEffect.transform.SetParent(null);
            // StartCoroutine(SphereInk(.5f));
            SphereBomb.transform.DOScale(0.5f, 0.1f).OnComplete(() =>
            {
                SphereBomb.GetComponent<MeshRenderer>().material.DOFloat(4.5f, "_Displacement_Strength", 0.5f);
                InkMesh.transform.DOScale(3f, 0.45f);
                InkMesh.GetComponent<MeshRenderer>().material.DOFloat(2f, "_Displacement_Strength", 0.5f);
                Invoke("InkParticle", 0.275f);
                Invoke("InksplashParticle", 0.15f);
                SphereBomb.GetComponent<MeshRenderer>().material.DOFloat(1f, "_Noise_Appear", 0.5f).SetDelay(0.2f);
                InkMesh.GetComponent<MeshRenderer>().material.DOFloat(4f, "_Noise_Appear", 0.5f).SetDelay(0.2f);
            });
        }

       private IEnumerator SphereInk(float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(sphereInk);
            Destroy(bombEffect);
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
}