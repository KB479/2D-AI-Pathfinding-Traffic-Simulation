using System.Collections.Generic;
using UnityEngine;

public class NPCMotor : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float turnSpeed = 10f;

    private List<Vector3> waypoints;
    private int currentWaypointIndex = 0;

    [HideInInspector] public bool isSensorBraking = false;

    public void SetRoute(List<Vector3> newRoute)
    {
        waypoints = newRoute;
        currentWaypointIndex = 0;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0) return;
        MoveAlongRoute();
    }

    private void MoveAlongRoute()
    {
        Vector3 targetPos = waypoints[currentWaypointIndex];

        targetPos.y = transform.position.y;

        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        if (isSensorBraking) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Count)
            {
                Destroy(gameObject);
            }
        }
    }
}