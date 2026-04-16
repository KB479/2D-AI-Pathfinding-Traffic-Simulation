using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCMotor))]
public class NPCSensor : MonoBehaviour
{
    [Header("Sensör Ayarlarý")]
    public float sensorDistance = 2f;
    public float sensorRadius = 0.5f;
    public float sensorHeight = 0.5f;
    public LayerMask obstacleMask;

    private NPCMotor motor;

    private float stuckTimer = 0f;
    private bool isForcePushing = false;

    [HideInInspector] public bool isWaitingAtLight = false;

    void Awake()
    {
        motor = GetComponent<NPCMotor>();
    }

    void Update()
    {
        CheckForward();
        HandleStuckLogic();
    }

    void CheckForward()
    {
        if (isForcePushing)
        {
            motor.isSensorBraking = false;
            return;
        }

        Vector3 p1 = transform.position + Vector3.up * sensorHeight;
        Vector3 p2 = p1 + Vector3.up * 0.1f;

        bool hitSomething = Physics.CapsuleCast(p1, p2, sensorRadius, transform.forward, out RaycastHit hit, sensorDistance, obstacleMask);

        if (hitSomething && hit.collider.transform.root != this.transform.root)
        {
            TrafficLight light = hit.collider.GetComponentInParent<TrafficLight>();

            if (light != null)
            {
                if (light.currentState == LightState.Red || light.currentState == LightState.Yellow)
                {
                    motor.isSensorBraking = true;
                    isWaitingAtLight = true; 
                }
                else
                {
                    motor.isSensorBraking = false;
                    isWaitingAtLight = false;
                }
            }
            else
            {
                motor.isSensorBraking = true;

                NPCSensor frontNPC = hit.collider.GetComponentInParent<NPCSensor>();
                CarSensor frontPlayer = hit.collider.GetComponentInParent<CarSensor>();

                if (frontNPC != null && frontNPC.isWaitingAtLight)
                {
                    isWaitingAtLight = true;
                }
                else if (frontPlayer != null && frontPlayer.isWaitingAtLight)
                {
                    isWaitingAtLight = true; 
                }
                else
                {
                    isWaitingAtLight = false;
                }
            }
        }
        else
        {
            motor.isSensorBraking = false;
            isWaitingAtLight = false;
        }
    }

    void HandleStuckLogic()
    {
        if (motor.isSensorBraking && !isWaitingAtLight && !isForcePushing)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= 2f)
            {
                StartCoroutine(ForcePushRoutine());
            }
        }
        else if (!motor.isSensorBraking)
        {
            stuckTimer = 0f;
        }
    }

    IEnumerator ForcePushRoutine()
    {
        isForcePushing = true;
        yield return new WaitForSeconds(1.5f);
        isForcePushing = false;
        stuckTimer = 0f;
    }

    void OnDrawGizmos()
    {
        bool isBraking = (Application.isPlaying && motor != null && motor.isSensorBraking);
        Gizmos.color = isBraking ? new Color(1, 0, 0, 0.4f) : new Color(0, 1, 1, 0.3f);

        if (isForcePushing) Gizmos.color = new Color(1, 0, 1, 0.5f);

        Vector3 p1 = transform.position + Vector3.up * sensorHeight;
        Vector3 hitCenter = p1 + transform.forward * sensorDistance;

        Gizmos.DrawLine(p1, hitCenter);
        Gizmos.DrawWireSphere(p1, sensorRadius);
        Gizmos.DrawSphere(hitCenter, sensorRadius);
    }
}