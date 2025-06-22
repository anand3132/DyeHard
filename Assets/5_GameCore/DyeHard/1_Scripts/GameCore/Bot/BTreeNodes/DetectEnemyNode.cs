using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class DetectEnemyNode : BTNode
    {
        private BotController bot;
        private float sightRange;
        private float fovAngle;
        private Transform detectedPlayer;

        private float followDuration = 5f; // Time to stay in follow mode
        private float cooldownTime = 3f; // Cooldown before next detection
        private float detectionTime;
        private float cooldownEndTime;

        private enum DetectionState
        {
            Searching,
            Following,
            Cooldown
        }

        private DetectionState state = DetectionState.Searching;

        public DetectEnemyNode(BotController bot, float sightRange, float fovAngle)
        {
            this.bot = bot;
            this.sightRange = sightRange;
            this.fovAngle = fovAngle;
        }

        protected override BTStatus ExecuteNode()
        {
            float time = Time.time;

            switch (state)
            {
                case DetectionState.Searching:
                    detectedPlayer = DetectEnemyWithCustomFOV(360f, 32);
                    bot.detectedPlayer = detectedPlayer;

                    if (detectedPlayer != null)
                    {
                        detectionTime = time;
                        state = DetectionState.Following;
                        return BTStatus.Success;
                    }

                    return BTStatus.Failure;

                case DetectionState.Following:
                    if (time - detectionTime < followDuration)
                    {
                        // Still following
                        return BTStatus.Success;
                    }
                    else
                    {
                        // Switch to cooldown
                        state = DetectionState.Cooldown;
                        cooldownEndTime = time + cooldownTime;
                        return BTStatus.Failure;
                    }

                case DetectionState.Cooldown:
                    if (time >= cooldownEndTime)
                    {
                        state = DetectionState.Searching;
                    }

                    return BTStatus.Failure;
            }

            return BTStatus.Failure;
        }

        public Transform DetectEnemyWithCustomFOV(float customFovAngle, int rayCount = 16)
        {
            return PerformEnemyDetection(customFovAngle, rayCount);
        }

        private Transform PerformEnemyDetection(float angleRange, int rayCount)
        {
            Vector3 rayOrigin = GetRayOrigin();
            Vector3 forward = bot.transform.forward;
            float angleStep = angleRange / rayCount;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = -angleRange / 2f + (i * angleStep);
                Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;

                Transform enemy = CheckRayForEnemy(rayOrigin, direction);
                if (enemy != null)
                    return enemy;
            }

            return null;
        }

        private Vector3 GetRayOrigin()
        {
            CapsuleCollider capsuleCollider = bot.GetComponent<CapsuleCollider>();
            return capsuleCollider != null ? capsuleCollider.bounds.center : bot.transform.position;
        }

        private Transform CheckRayForEnemy(Vector3 origin, Vector3 direction)
        {
            RaycastHit[] hits = Physics.RaycastAll(origin, direction, sightRange, ~0, QueryTriggerInteraction.Collide);
            foreach (var hit in hits)
            {
                BaseCharacterController character = hit.collider.GetComponentInParent<BaseCharacterController>();
                if (IsValidEnemy(character))
                {
                    Debug.DrawRay(origin, direction * sightRange, Color.red, 0.1f);
                    return character.transform;
                }
            }

            return null;
        }

        private bool IsValidEnemy(BaseCharacterController character)
        {
            return character != null && character.CurrentTeam != bot.CurrentTeam;
        }
    }

}
