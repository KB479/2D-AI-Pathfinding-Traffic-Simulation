using UnityEngine;
using TMPro;

public enum LightState
{
    Green,
    Yellow,
    Red
}

public class TrafficLight : MonoBehaviour
{
    [Header("Bileþenler")]
    public Renderer lightRenderer;
    public TextMeshProUGUI timeText;

    [Header("Otomatik Test Modu")]
    public bool autoCycle = false; 
    public float greenDuration = 10f;
    public float yellowDuration = 3f;
    public float redDuration = 10f;

    [Header("Anlýk Durum")]
    public LightState currentState;
    public float timeRemaining;

    public Node associatedNode;

    void Start()
    {
        if (GridManager.Instance != null)
            associatedNode = GridManager.Instance.NodeFromWorldPoint(transform.position);

        if (lightRenderer == null) lightRenderer = GetComponent<Renderer>();

        if (autoCycle && timeRemaining <= 0)
        {
            SetState(currentState, GetDurationForState(currentState));
        }
        else
        {
            UpdateVisuals();
        }
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timeText != null)
                timeText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
        else if (autoCycle) 
        {
            CycleNextState();
        }
    }

    void CycleNextState()
    {
        switch (currentState)
        {
            case LightState.Green:
                SetState(LightState.Red, redDuration);
                break;
            case LightState.Yellow:
                SetState(LightState.Green, greenDuration);
                break;
            case LightState.Red:
                SetState(LightState.Yellow, yellowDuration);
                break;
        }
    }

    float GetDurationForState(LightState state)
    {
        switch (state)
        {
            case LightState.Green: return greenDuration;
            case LightState.Yellow: return yellowDuration;
            case LightState.Red: return redDuration;
            default: return 5f;
        }
    }

    public void SetState(LightState newState, float duration)
    {
        currentState = newState;
        timeRemaining = duration;
        UpdateVisuals();
    }

    public int GetPenaltyCost()
    {
        switch (currentState)
        {
            case LightState.Green:
                return 0;

            case LightState.Yellow:
                return 0; 

            case LightState.Red:
                return Mathf.CeilToInt(timeRemaining) * 15;

            default: return 0;
        }
    }

    void UpdateVisuals()
    {
        Color color = Color.white;
        switch (currentState)
        {
            case LightState.Green: color = Color.green; break;
            case LightState.Yellow: color = Color.yellow; break;
            case LightState.Red: color = Color.red; break;
        }

        if (lightRenderer != null) lightRenderer.material.color = color;
        if (timeText != null)
        {
            timeText.color = Color.black;
            timeText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
    }
}