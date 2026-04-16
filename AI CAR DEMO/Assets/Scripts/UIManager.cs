using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI statusText;
    public Button startButton;
    public Button carPauseButton;    
    public Button globalPauseButton; 
    public Button restartButton;

    [Header("Paneller")]
    public GameObject pauseOverlayPanel; 

    void Start()
    {
        UpdateStatus("Lütfen sol týk ile baþlangýç noktasýný seçin.");
        SetButtonStates(false, false, false, false); 
        SetPauseOverlay(false); 
    }

    public void UpdateStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }

    public void SetButtonStates(bool canStart, bool canCarPause, bool canGlobalPause, bool canRestart)
    {
        if (startButton != null) startButton.interactable = canStart;
        if (carPauseButton != null) carPauseButton.interactable = canCarPause;
        if (globalPauseButton != null) globalPauseButton.interactable = canGlobalPause;
        if (restartButton != null) restartButton.interactable = canRestart;
    }

    public void SetPauseOverlay(bool isPaused)
    {
        if (pauseOverlayPanel != null)
        {
            pauseOverlayPanel.SetActive(isPaused);
        }
    }
}