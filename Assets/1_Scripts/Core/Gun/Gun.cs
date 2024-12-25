using PaintIn3D;
using UnityEngine;

namespace RedGaint
{
    public class Gun : MonoBehaviour
    {
        public GlobalEnums.GunType gunType;
        public ParticleSystem gunNozile;
        [SerializeField] private Color currentColor;

        public void SetGunColor(Color color)
        {
            currentColor = color;
            gunNozile.gameObject.GetComponentInChildren<CwPaintSphere>().Color = color;
        }

        public void StartShoot()
        {
            gunNozile.Play();
        }
        public void  StopShoot()
        {
            gunNozile.Stop();
        }
    }
}