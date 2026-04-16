using System.Collections.Generic;
using UnityEngine;

public class CarMotor : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;

    private List<Node> currentPath;
    private int currentWaypointIndex = 0;

    private bool isMoving = false;

    [HideInInspector] public bool isSensorBraking = false;

    public List<Node> GetActivePath()
    {
        return currentPath;
    }

    public void SetPath(List<Node> newPath, bool autoStart = false)
    {
        currentPath = newPath;
        currentWaypointIndex = 0;
        isMoving = autoStart;
    }

    public void StartMotor()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            isMoving = true;
        }
    }

    public void StopMotor()
    {
        isMoving = false;
        currentPath = null;
    }

    public void ToggleMotorPause()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            isMoving = !isMoving;
        }
    }

    void Update()
    {
        if (!isMoving || currentPath == null || currentPath.Count == 0) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        Vector3 targetPos = currentPath[currentWaypointIndex].worldPosition;
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

            if (currentWaypointIndex >= currentPath.Count)
            {
                StopMotor();
                Debug.Log("Hedefe ulaþýldý!");

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnTargetReached();
                }


            }
        }
    }
}