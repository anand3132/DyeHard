
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

namespace RedGaint
{
    public class GamePlayManager : Singleton<GamePlayManager>,IBugsBunny
    {
        public bool LogThisClass { get; } = false;

        // public GameObject Player;
        public GolbalGameData golbalGameData;
        public CinemachineCamera cinemachineCamera;
        public GameObject GetPlayer()
        {
            return currentPlayer;
        }

        public void ChangeCameraTarget(Transform followTarget) 
        {
            if (cinemachineCamera != null)
            {
                // Get the CinemachineOrbitalFollow component
                var orbitalFollow = cinemachineCamera.GetComponent<CinemachineCamera>();
                if (orbitalFollow != null)
                {
                    CameraTarget newTarget = new CameraTarget();
                    newTarget = orbitalFollow.Target;
                    newTarget.TrackingTarget = followTarget;
                    orbitalFollow.Target = newTarget;
                }
                else
                {
                    Debug.LogError("CinemachineOrbitalFollow component not found on the Cinemachine Camera.");
                }
            }
            else
            {
                Debug.LogError("Cinemachine Camera is not assigned!");
            }
        }
        private void Awake()
        {
            if (golbalGameData == null)
            {
                BugsBunny.LogRed("Game Core ERROR:: GolbalGameData is null please attach the GolbalGameData..!!",this);
                return;
            }
            GlobalStaticVariables.LoadFromScriptableObject(golbalGameData);
            if(currentPlayer == null)
                RespwanMainPlayer();
        }

        public Transform playerRespwanPosition;
        public GameObject mainPlayerPrefab;
        private GameObject currentPlayer;
        public void RespwanMainPlayer()
        {
            if (currentPlayer == null)
            {
                currentPlayer = Instantiate(mainPlayerPrefab);
                currentPlayer.transform.position = playerRespwanPosition.position;
                currentPlayer.transform.rotation = playerRespwanPosition.rotation;
            }
            else
            {
                currentPlayer.GetComponent<BaseCharacterController>().ResetAll();
            }
            StartCoroutine(RespwanMainPlayerCoroutine(1f));
        }

        private IEnumerator RespwanMainPlayerCoroutine(float seconds = 1f)
        {
            yield return new WaitForSeconds(seconds);
            currentPlayer.transform.position = playerRespwanPosition.position;
            currentPlayer.transform.rotation = playerRespwanPosition.rotation;
            ChangeCameraTarget(currentPlayer.transform);
            currentPlayer.SetActive(true);
        }
    }
}