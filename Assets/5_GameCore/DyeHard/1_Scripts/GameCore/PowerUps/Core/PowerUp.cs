using UnityEngine;
using System;
using DG.Tweening;

namespace RedGaint.Games.DyeHard
{
    public class PowerUp : MonoBehaviour,IBugsBunny,ITriggerReceiver
    {
        [SerializeField] private float _moveDistance = 1;
        [SerializeField] private float _moveDuration = 1;
        [SerializeField] private float _rotationDuration = 2.5f;
        [SerializeField] private Ease _moveEase = Ease.InOutSine;
        [SerializeField] private Ease _rotationEase = Ease.Linear;
        [SerializeField] private LoopType _moveLoopType = LoopType.Yoyo;
        [SerializeField] private LoopType _rotationLoopType = LoopType.Restart;
        [SerializeField] private Transform[] _targets;
        
        public bool LogThisClass { get; } = false;
        public event Action<int> OnPowerUpConsumed;
        int positionIndex;
        public GlobalEnums.PowerUpType powerUpType;
        private bool isActive=false;
        
        public void Activate()
        {
            foreach (Transform t in _targets)
            {
                t.DOMoveY(this.transform.position.y + _moveDistance, _moveDuration)
                    .From(this.transform.position.y).SetEase(_moveEase).SetLoops(-1, _moveLoopType);
                t.DOLocalRotate(
                        new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y + 360,
                            this.transform.rotation.eulerAngles.z), _rotationDuration, RotateMode.FastBeyond360)
                    .From(this.transform.rotation.eulerAngles).SetEase(_rotationEase).SetLoops(-1, _rotationLoopType);
            }
        }
        
        public void Initialize(int _positionIndex)
        {
            var colliedr = GetComponentInChildren<CollisionTriggerHelper>();
            colliedr.AddReceiver(this);
            colliedr.InitializeReceivers();
            
            positionIndex=_positionIndex;
                isActive = true;
                BugsBunny.Log("PowerUp Initialized", this);
                Activate();
        }

        public void OnTriggerEnterReceived(Collider other)
        {
            var baseCharacter = other.GetComponentInParent<BaseCharacterController>();
            if (baseCharacter != null)
            {
                var powerUpBasket = other.GetComponentInParent<PowerUpBasket>();
                if (powerUpBasket != null && powerUpBasket.ActivateCurrentPowerUp(powerUpType))
                {
                    // Trigger the event to notify the generator
                    OnPowerUpConsumed?.Invoke(positionIndex);
                    // Destroy the power-up after collection
                    Destroy(gameObject);
                }

            }
            
        }

        public void OnTriggerStayReceived(Collider other) { }

        public void OnTriggerExitReceived(Collider other) { }
    }//PowerUp
}//RedGaint
