using System.Collections;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var runnerObj = new GameObject("CoroutineRunner");
                    _instance = runnerObj.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(runnerObj);
                }

                return _instance;
            }
        }

        /// <summary>
        /// Start a coroutine through the runner.
        /// </summary>
        public static Coroutine Run(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        /// <summary>
        /// Stop a coroutine through the runner.
        /// </summary>
        public static void Stop(Coroutine coroutine)
        {
            if (coroutine != null && _instance != null)
            {
                _instance.StopCoroutine(coroutine);
            }
        }
    }
}