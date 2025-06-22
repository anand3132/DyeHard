using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class SimpleBotFollower : MonoBehaviour
{
    public GameObject root;
    public float detectionRange = 10f;
    public float detectionFOV = 90f;
    public float moveSpeed = 2f;
    public Transform enemyTarget;

    private CapsuleCollider actorCollider;

    private void Start()
    {
        actorCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (enemyTarget == null)
        {
            DetectEnemy();
        }
        else
        {
            MoveTowardsTarget(enemyTarget.position);
        }
    }

    private void DetectEnemy()
    {
        Vector3 origin = actorCollider.bounds.center;
        Vector3 forward = transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin, detectionRange);
        foreach (var hit in hits)
        {
            if (hit.transform == this.transform) continue; // skip self

            Vector3 toTarget = (hit.transform.position - origin).normalized;
            float angle = Vector3.Angle(forward, toTarget);

            // FOV check
            if (angle < detectionFOV * 0.5f)
            {
                enemyTarget = hit.transform;
                Debug.Log("Enemy Detected: " + hit.name);
                break;
            }
        }

        // Debug visuals
        Debug.DrawRay(origin, forward * detectionRange, Color.yellow);
        DebugDrawFOVCone(origin, forward, detectionRange, detectionFOV, 16, Color.red);
    }

    private void MoveTowardsTarget(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - root.transform.position).normalized;
        root.transform.position += dir * moveSpeed * Time.deltaTime;
        root.transform.forward = Vector3.Lerp(root.transform.forward, dir, Time.deltaTime * 5f);
    }

    private void DebugDrawFOVCone(Vector3 origin, Vector3 forward, float range, float fov, int segments, Color color)
    {
        float step = fov / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = -fov / 2 + step * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;
            Debug.DrawRay(origin, dir * range, color);
        }
    }
}
