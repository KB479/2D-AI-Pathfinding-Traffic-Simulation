using UnityEngine;

[RequireComponent(typeof(CarMotor))]
[RequireComponent(typeof(CarBrain))]
public class CarSensor : MonoBehaviour
{
    [Header("Sensor Ayarlarý")]
    public float sensorDistance = 2.5f;
    public float sensorRadius = 0.5f;
    public float sensorHeight = 0.5f;

    [Header("Filtreleme")]
    public LayerMask obstacleMask;

    private CarMotor motor;
    private CarBrain brain;
    private TrafficLight lastReportedLight = null;

    [HideInInspector] public bool isWaitingAtLight = false;

    void Awake()
    {
        motor = GetComponent<CarMotor>();
        brain = GetComponent<CarBrain>();
    }

    void Update()
    {
        CheckForward();
    }

    void CheckForward()
    {
        Vector3 p1 = transform.position + Vector3.up * sensorHeight;
        Vector3 p2 = p1 + Vector3.up * 0.1f;

        bool hitSomething = Physics.CapsuleCast(p1, p2, sensorRadius, transform.forward, out RaycastHit hit, sensorDistance, obstacleMask);

        if (hitSomething && hit.collider.transform.root != this.transform.root)
        {
            TrafficLight light = hit.collider.GetComponentInParent<TrafficLight>();

            if (light != null)
            {
                bool ignoreLight = false;


                if (light.associatedNode != null && light.associatedNode.trafficDirection != Vector2.zero)
                {
                    Vector3 lightRoadDir = new Vector3(light.associatedNode.trafficDirection.x, 0, light.associatedNode.trafficDirection.y).normalized;

                    float alignment = Vector3.Dot(transform.forward, lightRoadDir);

                    if (alignment < 0.5f)
                    {
                        ignoreLight = true; 
                    }
                }

                if (ignoreLight)
                {
                    motor.isSensorBraking = false;
                    isWaitingAtLight = false;
                    return;
                }

                ProcessTrafficLight(light);
                return;
            }
            else
            {
                motor.isSensorBraking = true;

                NPCSensor frontNPC = hit.collider.GetComponentInParent<NPCSensor>();
                if (frontNPC != null && frontNPC.isWaitingAtLight)
                {
                    isWaitingAtLight = true;
                }
                else
                {
                    isWaitingAtLight = false;
                }
                return;
            }
        }

        if (motor.isSensorBraking)
        {
            motor.isSensorBraking = false;
            isWaitingAtLight = false;
            lastReportedLight = null;
        }
    }

    void ProcessTrafficLight(TrafficLight light)
    {
        if (light.currentState == LightState.Red)
        {
            motor.isSensorBraking = true;
            isWaitingAtLight = true;

            if (lastReportedLight != light)
            {
                lastReportedLight = light;
                brain.OnObstacleDetected(light);
            }
        }
        else if (light.currentState == LightState.Yellow)
        {
            motor.isSensorBraking = true;
            isWaitingAtLight = true;
            lastReportedLight = null;
        }
        else if (light.currentState == LightState.Green)
        {
            motor.isSensorBraking = false;
            isWaitingAtLight = false;
            lastReportedLight = null;
        }
    }

    void OnDrawGizmos()
    {
        bool isBraking = (Application.isPlaying && motor != null && motor.isSensorBraking);
        Gizmos.color = isBraking ? new Color(1, 0, 0, 0.4f) : new Color(0, 1, 1, 0.3f);

        Vector3 p1 = transform.position + Vector3.up * sensorHeight;
        Vector3 hitCenter = p1 + transform.forward * sensorDistance;

        Gizmos.DrawLine(p1, hitCenter);
        Gizmos.DrawWireSphere(p1, sensorRadius);
        Gizmos.DrawSphere(hitCenter, sensorRadius);
    }
}