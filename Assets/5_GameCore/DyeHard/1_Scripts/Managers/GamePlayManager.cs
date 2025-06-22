
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using RedGaint.Games.DyeHard.UI;
using UnityEngine.SceneManagement;

namespace RedGaint.Games.DyeHard
{
    public class GamePlayManager : SingletonSimple<GamePlayManager>,IBugsBunny
    {
        public bool GodMode = false;

        public bool LogThisClass { get; } = false;
        public string bootStrapSceneName = "GameBootStrap"; 
        public GolbalGameData golbalGameData;
        public CinemachineCamera cinemachineCamera;
        public GameObject GetPlayer()
        {
            return currentPlayer.gameObject;
        }
        public void OnPlayerDeadth()
        {
            // currentPlayer.SetActive(false);
            RespawnScreenContext context = new RespawnScreenContext
            {
                respawnTime = 5f,
                onRespawnComplete = () =>
                {
                    RespwanMainPlayer();
                    UXController.Instance.ShowScreen(UIScreen.HUD);
                }
            };
            UXController.Instance.ShowScreen(UIScreen.Respawn,context);
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
            PositionHandler.Instance.Initialize();
            GlobalStaticVariables.LoadFromScriptableObject(golbalGameData);
            if(currentPlayer == null)
                RespwanMainPlayer();
            UXController.Instance.ShowScreen(UIScreen.HUD);
            
        }

        public Transform playerRespwanPosition;
        public GameObject mainPlayerPrefab;
        private PlayerController currentPlayer;
        private void RespwanMainPlayer()
        {
            if (currentPlayer == null)
            {
                
                currentPlayer = Instantiate(mainPlayerPrefab).GetComponent<PlayerController>();
                currentPlayer.gameObject.SetActive(true);
                currentPlayer.transform.position = playerRespwanPosition.position;
                currentPlayer.transform.rotation = playerRespwanPosition.rotation;
            }
            else
            {
               currentPlayer.Deactivate();
            }

            if (GodMode)
            {
                currentPlayer.isGodMode=true;
            }
            StartCoroutine(RespwanMainPlayerCoroutine(1f));
        }

        private IEnumerator RespwanMainPlayerCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            currentPlayer.transform.position = playerRespwanPosition.position;
            currentPlayer.transform.rotation = playerRespwanPosition.rotation;
            ChangeCameraTarget(currentPlayer.transform);
            currentPlayer.ActivateTheActor();
            currentPlayer.UnfreezeInput();

        }

        public void OnGameEndTimeUpTriggered()
        {
            GameEndScreenContext context = new GameEndScreenContext
            {
                winningTeamMessage =GameProgressBar.Instance.GetCurrentHighestColorName() + " WINS!"
            };
            UXController.Instance.ShowScreen(UIScreen.GameOver,context);
            currentPlayer.GetComponent<PlayerController>().FreezeInput();
            
            StartCoroutine(LoadBootstrap(3f));
        }

        private IEnumerator LoadBootstrap(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(bootStrapSceneName);
        }
    }
}