using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionController : MonoBehaviour
{
    [Header("Iþýk Gruplarý")]
    public List<TrafficLight> groupA; 
    public List<TrafficLight> groupB; 

    [Header("Süre Ayarlarý")]
    public float greenDuration = 10f; 
    public float yellowDuration = 2f; 

    private void Start()
    {
        DisableAutoCycle(groupA);
        DisableAutoCycle(groupB);

        StartCoroutine(TrafficCycle());
    }

    void DisableAutoCycle(List<TrafficLight> lights)
    {
        foreach (var light in lights)
        {
            light.autoCycle = false;
        }
    }

    IEnumerator TrafficCycle()
    {
        SetLights(groupA, LightState.Green, greenDuration);
        SetLights(groupB, LightState.Red, greenDuration);

        yield return new WaitForSeconds(greenDuration);

        while (true)
        {
            SetLights(groupA, LightState.Red, greenDuration + yellowDuration);

            SetLights(groupB, LightState.Yellow, yellowDuration);

            yield return new WaitForSeconds(yellowDuration);

            SetLights(groupB, LightState.Green, greenDuration);

            yield return new WaitForSeconds(greenDuration);

            SetLights(groupB, LightState.Red, greenDuration + yellowDuration);

            SetLights(groupA, LightState.Yellow, yellowDuration);

            yield return new WaitForSeconds(yellowDuration);

            SetLights(groupA, LightState.Green, greenDuration);

            yield return new WaitForSeconds(greenDuration);

        }
    }

    void SetLights(List<TrafficLight> lights, LightState state, float duration)
    {
        foreach (var light in lights)
        {
            light.SetState(state, duration);
        }
    }
}