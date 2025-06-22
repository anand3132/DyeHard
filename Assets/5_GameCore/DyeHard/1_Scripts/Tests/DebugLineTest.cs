using UnityEngine;

namespace RedGaint.Games.DyeHard.Game.Core._1_Scripts.Core
{
    public class DebugLineTest : MonoBehaviour
    {
        public float lineLength = 5f;
        public int lineCount = 10;

        private void Update()
        {
            Vector3 origin = transform.position;

            for (int i = 0; i < lineCount; i++)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                Vector3 end = origin + randomDirection * lineLength;

                Debug.DrawLine(origin, end, Color.Lerp(Color.green, Color.magenta, Random.value), 1f);
            }
        }
    }

}