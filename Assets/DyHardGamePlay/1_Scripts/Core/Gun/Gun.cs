using PaintIn3D;
using UnityEngine;

namespace RedGaint
{
    public class Gun : MonoBehaviour
    {
        public GlobalEnums.GunType gunType;
        public ParticleSystem gunNozile; // Assign this in the Inspector
        [SerializeField] private Color currentColor;

        public void SetGunColor(Color color)
        {
            // Update current color
            currentColor = color;

            // Get all particle systems in the hierarchy
            ParticleSystem[] allParticleSystems = gunNozile.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem ps in allParticleSystems)
            {
                // Update the Particle System Renderer material color
                ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = color;
                }

                // Update the Trail Module material color if enabled
                if (ps.trails.enabled)
                {
                    Material trailMaterial = renderer.trailMaterial;
                    if (trailMaterial != null)
                    {
                        trailMaterial.color = color;
                    }
                }
            }

            // Optionally update CwPaintSphere color if required
            CwPaintSphere paintSphere = gunNozile.gameObject.GetComponentInChildren<CwPaintSphere>();
            if (paintSphere != null)
            {
                paintSphere.Color = color;
            }
        }

        public void StartShoot()
        {
            gunNozile.Play();
        }

        public void StopShoot()
        {
            gunNozile.Stop();
        }
    }
}