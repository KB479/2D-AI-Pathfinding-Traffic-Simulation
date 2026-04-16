using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string MAIN_MENU = "MainMenu";

    public static GameManager Instance { get; private set; }

    [Header("Sistem Baðlantýlarý")]
    public CarBrain playerCar;
    public UIManager uiManager;
    public TMP_Dropdown algorithmDropdown;

    [Header("Görsel Ýþaretler")]
    public Transform startMarker;
    public Transform targetMarker;
    public LineRenderer pathRenderer;

    private Node startNode;
    private Node targetNode;
    private List<Node> previewPath;

    [HideInInspector] public bool isSimulationStarted = false;
    private bool isGlobalPaused = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1f;
    }

    void Start()
    {
        if (startMarker != null) startMarker.gameObject.SetActive(false);
        if (targetMarker != null) targetMarker.gameObject.SetActive(false);
        if (playerCar != null) playerCar.gameObject.SetActive(false);
        if (pathRenderer != null) pathRenderer.positionCount = 0;

        if (uiManager != null)
        {
            uiManager.SetButtonStates(false, false, false, true);
        }
    }

    void Update()
    {
        if (isSimulationStarted) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // SOL TIK
        if (Input.GetMouseButtonDown(0))
        {
            SetNodeFromMouseClick(ref startNode, startMarker);

            if (startNode != null && playerCar != null)
            {
                playerCar.gameObject.SetActive(true);

                Vector3 spawnPos = startNode.worldPosition;
                spawnPos.y = playerCar.transform.position.y;

                playerCar.GetComponent<CarMotor>().StopMotor();
                playerCar.transform.position = spawnPos;

                previewPath = null;

                if (targetNode == null)
                {
                    uiManager.UpdateStatus("Baþlangýç seçildi. Lütfen sað týk ile hedef noktayý seçin.");
                    uiManager.SetButtonStates(false, false, false, true);
                }
                else
                {
                    uiManager.UpdateStatus("Rota güncellendi. Baþlatmaya hazýr!");
                    uiManager.SetButtonStates(true, false, false, true);

                    playerCar.SetTarget(targetNode.worldPosition);
                    CalculatePreviewPath();
                }
            }
        }

        // SAÐ TIK
        if (Input.GetMouseButtonDown(1))
        {
            if (startNode == null)
            {
                uiManager.UpdateStatus("HATA: Önce sol týk ile baþlangýç noktasýný seçmelisiniz!");
                return;
            }

            SetNodeFromMouseClick(ref targetNode, targetMarker);

            if (playerCar != null && targetNode != null)
            {
                startNode = GridManager.Instance.NodeFromWorldPoint(playerCar.transform.position);
                playerCar.SetTarget(targetNode.worldPosition);
                CalculatePreviewPath();

                uiManager.UpdateStatus("Hedef seçildi. Algoritma seçimini yapýp simülasyonu baþlatabilirsiniz!");
                uiManager.SetButtonStates(true, false, false, true);
            }
        }
    }

    void LateUpdate()
    {
        DrawPathWithLineRenderer();
    }

    private void DrawPathWithLineRenderer()
    {
        if (pathRenderer == null || !pathRenderer.enabled)
        {
            if (pathRenderer != null) pathRenderer.positionCount = 0;
            return;
        }

        List<Node> pathToShow = (isSimulationStarted && playerCar != null) ? playerCar.GetActivePath() : previewPath;

        if (pathToShow != null && pathToShow.Count > 0)
        {
            pathRenderer.positionCount = pathToShow.Count;
            for (int i = 0; i < pathToShow.Count; i++)
            {
                Vector3 pathPos = pathToShow[i].worldPosition + Vector3.up * 0.5f;
                pathRenderer.SetPosition(i, pathPos);
            }
        }
        else
        {
            pathRenderer.positionCount = 0;
        }
    }

    private void SetNodeFromMouseClick(ref Node nodeToSet, Transform marker)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int ignoreMask = ~LayerMask.GetMask("Vehicle", "TrafficLight", "TrafficEntity");

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ignoreMask))
        {
            Node clickedNode = GridManager.Instance.NodeFromWorldPoint(hit.point);
            if (clickedNode != null && clickedNode.walkable)
            {
                nodeToSet = clickedNode;
                if (marker != null)
                {
                    marker.position = clickedNode.worldPosition + Vector3.up * 0.1f;
                    marker.gameObject.SetActive(true);
                }
            }
            else
            {
                uiManager.UpdateStatus("Uyarý: Týklanan yer yürünemez bir alan!");
            }
        }
    }

    private void SetAlgorithmToCar()
    {
        if (playerCar != null && algorithmDropdown != null)
        {
            playerCar.currentAlgorithm = (CarBrain.SearchAlgorithm)algorithmDropdown.value;
        }
    }

    public void OnAlgorithmChanged()
    {
        if (startNode != null && targetNode != null && !isSimulationStarted)
        {
            SetAlgorithmToCar();
            CalculatePreviewPath();
            uiManager.UpdateStatus($"Algoritma {algorithmDropdown.options[algorithmDropdown.value].text} olarak deðiþtirildi.");
        }
    }

    private void CalculatePreviewPath()
    {
        if (startNode == null || targetNode == null) return;

        SetAlgorithmToCar();
        playerCar.CalculatePath(false);
        previewPath = playerCar.GetActivePath();
    }

   
    // UI buton fonksiyonlarý 
    public void TogglePathVisualizationButton()
    {
        if (pathRenderer != null)
        {
            pathRenderer.enabled = !pathRenderer.enabled;
            string status = pathRenderer.enabled ? "AÇIK" : "KAPALI";
            uiManager.UpdateStatus($"Rota görünümü {status}.");
        }
    }

    public void StartSimulationButton()
    {
        if (playerCar != null && targetNode != null)
        {
            playerCar.StartSimulation();
            isSimulationStarted = true;
            previewPath = null;

            uiManager.UpdateStatus("Simülasyon çalýþýyor...");
            uiManager.SetButtonStates(false, true, true, true);
        }
        else
        {
            uiManager.UpdateStatus("Lütfen önce Sol Týk ile aracý, Sað Týk ile hedefi seçin!");
        }
    }

    public void ToggleCarPauseButton()
    {
        if (!isSimulationStarted) return;

        if (playerCar != null)
        {
            playerCar.TogglePause();
            uiManager.UpdateStatus("Araç motoru durduruldu / çalýþtýrýldý (El Freni).");
        }
    }

    public void ToggleGlobalPauseButton()
    {
        if (!isSimulationStarted) return;

        isGlobalPaused = !isGlobalPaused;

        if (isGlobalPaused)
        {
            Time.timeScale = 0f;
            uiManager.UpdateStatus("Simülasyon Duraklatýldý.");
            uiManager.SetPauseOverlay(true); 
        }
        else
        {
            Time.timeScale = 1f;
            uiManager.UpdateStatus("Simülasyon çalýþýyor...");
            uiManager.SetPauseOverlay(false); 
        }
    }

    public void RestartSimulationButton()
    {
        Time.timeScale = 1f;
        isGlobalPaused = false;

        if (UILogger.Instance != null) UILogger.Instance.ClearLogs();

        isSimulationStarted = false;
        startNode = null;
        targetNode = null;
        previewPath = null;

        if (pathRenderer != null) pathRenderer.positionCount = 0;
        if (startMarker != null) startMarker.gameObject.SetActive(false);
        if (targetMarker != null) targetMarker.gameObject.SetActive(false);

        if (playerCar != null)
        {
            playerCar.GetComponent<CarMotor>().StopMotor();
            playerCar.gameObject.SetActive(false);
        }

        NPCMotor[] npcs = FindObjectsOfType<NPCMotor>();
        foreach (NPCMotor npc in npcs)
        {
            Destroy(npc.gameObject);
        }

        uiManager.UpdateStatus("Simülasyon sýfýrlandý. Lütfen sol týk ile baþlangýç noktasýný seçin.");
        uiManager.SetButtonStates(false, false, false, true);
        uiManager.SetPauseOverlay(false); 
    }

    public void OnTargetReached()
    {
        uiManager.UpdateStatus("Hedefe baþarýyla ulaþýldý! Araç trafiði açmak için haritadan çekiliyor...");
        uiManager.SetButtonStates(false, false, false, true);

        StartCoroutine(HideCarAfterDelay(2f));
    }

    private IEnumerator HideCarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerCar != null)
        {
            playerCar.gameObject.SetActive(false);
        }
    }

    public void ExitToMainMenuButton()
    {
        Time.timeScale = 1f;
        isGlobalPaused = false;

        if (UILogger.Instance != null) UILogger.Instance.ClearLogs();

        NPCMotor[] npcs = FindObjectsOfType<NPCMotor>();
        foreach (NPCMotor npc in npcs)
        {
            Destroy(npc.gameObject);
        }

        SceneManager.LoadScene(MAIN_MENU);
    }

    void OnDrawGizmos()
    {
        List<Node> pathToShow = (isSimulationStarted && playerCar != null) ? playerCar.GetActivePath() : previewPath;

        if (pathToShow != null && pathToShow.Count > 0)
        {
            Gizmos.color = Color.cyan;

            for (int i = 0; i < pathToShow.Count; i++)
            {
                Vector3 pathPos = pathToShow[i].worldPosition + Vector3.up * 1.5f;
                Gizmos.DrawSphere(pathPos, GridManager.Instance.nodeRadius * 0.5f);

                if (i > 0)
                {
                    Vector3 prevPos = pathToShow[i - 1].worldPosition + Vector3.up * 1.5f;
                    Gizmos.DrawLine(prevPos, pathPos);
                }
            }
        }
    }
}