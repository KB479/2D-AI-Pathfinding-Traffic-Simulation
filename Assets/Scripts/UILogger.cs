using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILogger : MonoBehaviour
{
    public static UILogger Instance { get; private set; }

    [Header("UI Bileþenleri")]
    public TextMeshProUGUI logText;
    public ScrollRect scrollRect;

    [Header("Ayarlar")]
    public int maxLines = 50;

    private Queue<string> logQueue = new Queue<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Log && type != LogType.Warning) return;

        logQueue.Enqueue(logString);

        if (logQueue.Count > maxLines)
        {
            logQueue.Dequeue();
        }

        UpdateLogText();
    }

    public void ClearLogs()
    {
        logQueue.Clear();
        UpdateLogText();
    }

    private void UpdateLogText()
    {
        if (logText != null)
        {
            logText.text = string.Join("\n\n", logQueue);
        }

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; 
        }
    }
}