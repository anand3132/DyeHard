using UnityEngine;
using DG.Tweening;
using PaintIn3D;

namespace RedGaint.Games.DyeHard
{
    public class BombPowerUp : MonoBehaviour, IPowerUp, IInjectUser,ICollisionReceiver
    {
        public GameObject explotlotion;
        public GameObject smoke;
        public ParticleSystem shrapnels;
        public ParticleSystem explotionFX;
        public GameObject bombColorElement;

        private ICharacterPowerUpUser user;

        public void SetUser(ICharacterPowerUpUser user)
        {
            this.user = user;
        }

        public void Initialize() { }

        public void Activate()
        {
            InkBlasting();
            CoroutineRunner.Instance.StartCoroutine(DelayedCleanup(1.5f)); // adjust duration
        }

        public void Cleanup()
        {
            Destroy(explotlotion.gameObject);
            Destroy(gameObject);
        }

        public void SetBombFor(GlobalEnums.GameTeam team)
        {
            var tData = TeamManager.Instance.GetTeamData(team);
            bombColorElement.GetComponent<CollisionTriggerHelper>().AddReceiver(this);
            bombColorElement.GetComponent<CollisionTriggerHelper>().InitializeReceivers();
            if (bombColorElement.TryGetComponent(out CwPaintDecal decal))
            {
                
                decal.Color = tData.TeamColor;
            }
        }

        private void InkBlasting()
        {
            bombColorElement.transform.SetParent(null);
            explotlotion.transform.SetParent(null);

            explotlotion.transform.DOScale(0.5f, 0.1f).OnComplete(() =>
            {
                explotlotion.GetComponent<MeshRenderer>().material.DOFloat(4.5f, "_Displacement_Strength", 0.5f);
                
                smoke.transform.DOScale(3f, 0.45f);
                smoke.GetComponent<MeshRenderer>().material.DOFloat(2f, "_Displacement_Strength", 0.5f);

                Invoke("InkParticle", 0.275f);
                Invoke("InksplashParticle", 0.15f);

                explotlotion.GetComponent<MeshRenderer>().material.DOFloat(1f, "_Noise_Appear", 0.5f).SetDelay(0.2f);
                smoke.GetComponent<MeshRenderer>().material.DOFloat(4f, "_Noise_Appear", 0.5f).SetDelay(0.2f);
            });
        }

        private void InkParticle() => shrapnels.Play();
        private void InksplashParticle() => explotionFX.Play();

        private System.Collections.IEnumerator DelayedCleanup(float delay)
        {
            yield return new WaitForSeconds(delay);
            Cleanup();
        }

        public void OnCollisionEnterReceived(Collision collision)
        {
            Destroy(bombColorElement.gameObject);

        }

        public void OnCollisionStayReceived(Collision collision)
        {
        }

        public void OnCollisionExitReceived(Collision collision)
        {
        }
    }
}
