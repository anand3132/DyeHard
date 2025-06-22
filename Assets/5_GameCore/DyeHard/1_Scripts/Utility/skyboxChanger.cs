using UnityEngine;

namespace RedGaint.Games.DyeHard
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