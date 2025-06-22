using System.Collections.Generic;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    [RequireComponent(typeof(Collider))]
    public class CollisionTriggerHelper : MonoBehaviour
    {
        [Tooltip("Manually assigned receivers.")]
        public List<MonoBehaviour> manualReceivers = new();

        private readonly List<ITriggerReceiver> _triggerReceivers = new();
        private readonly List<ICollisionReceiver> _collisionReceivers = new();

        private void Awake()
        {
            InitializeReceivers();
        }

        public void InitializeReceivers()
        {
            _triggerReceivers.Clear();
            _collisionReceivers.Clear();

            foreach (var receiver in manualReceivers)
            {
                if (receiver == null) continue;

                var components = receiver.GetComponents<MonoBehaviour>();

                foreach (var comp in components)
                {
                    if (comp is ITriggerReceiver trigger && !_triggerReceivers.Contains(trigger))
                        _triggerReceivers.Add(trigger);

                    if (comp is ICollisionReceiver collision && !_collisionReceivers.Contains(collision))
                        _collisionReceivers.Add(collision);
                }
            }
        }

        public void AddReceiver(MonoBehaviour receiver)
        {
            if (receiver == null) return;

            var components = receiver.GetComponents<MonoBehaviour>();

            foreach (var comp in components)
            {
                if (!manualReceivers.Contains(comp))
                    manualReceivers.Add(comp);

                if (comp is ITriggerReceiver trigger && !_triggerReceivers.Contains(trigger))
                    _triggerReceivers.Add(trigger);

                if (comp is ICollisionReceiver collision && !_collisionReceivers.Contains(collision))
                    _collisionReceivers.Add(collision);
            }
        }

        public void RemoveReceiver(MonoBehaviour receiver)
        {
            if (receiver == null) return;

            manualReceivers.Remove(receiver);

            if (receiver is ITriggerReceiver trigger)
                _triggerReceivers.Remove(trigger);

            if (receiver is ICollisionReceiver collision)
                _collisionReceivers.Remove(collision);
        }

        // Trigger Events
        private void OnTriggerEnter(Collider other)
        {
            foreach (var receiver in _triggerReceivers)
                receiver.OnTriggerEnterReceived(other);
        }

        private void OnTriggerStay(Collider other)
        {
            foreach (var receiver in _triggerReceivers)
                receiver.OnTriggerStayReceived(other);
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (var receiver in _triggerReceivers)
                receiver.OnTriggerExitReceived(other);
        }

        // Collision Events
        private void OnCollisionEnter(Collision collision)
        {
            foreach (var receiver in _collisionReceivers)
                receiver.OnCollisionEnterReceived(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (var receiver in _collisionReceivers)
                receiver.OnCollisionStayReceived(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            foreach (var receiver in _collisionReceivers)
                receiver.OnCollisionExitReceived(collision);
        }
    }
}
