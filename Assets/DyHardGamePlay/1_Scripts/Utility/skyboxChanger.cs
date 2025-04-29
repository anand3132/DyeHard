using UnityEngine;

namespace RedGaint
{
    public class SkyboxChanger : MonoBehaviour
    {
        public Material newSkybox;

        void Start()
        {
            RenderSettings.skybox = newSkybox;
        }
    }
}