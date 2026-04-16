using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; 

public class NodeDebugger : MonoBehaviour
{
    [Header("Ayarlar")]
    public Color selectionColor = Color.magenta;

    [Header("UI Baðlantýlarý")]
    public TextMeshProUGUI debugTextUI;

    [Header("UI Görünüm Ayarlarý")]
    public float instructionFontSize = 24f;
    public float debugFontSize = 18f;

    private string defaultInstruction = "Düðüm (Node) detaylarýný görmek için farenizi harita üzerinde gezdirin.";

    private Node selectedNode;

    void Start()
    {
        ShowInstruction();
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedNode != null)
            {
                selectedNode = null;
                ShowInstruction();
            }
            return; 
        }

        InspectNodeUnderMouse();
    }

    private void InspectNodeUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Node candidateNode = GridManager.Instance.NodeFromWorldPoint(hitPoint);

            if (candidateNode != null)
            {
                selectedNode = candidateNode;

                string dirInfo = (selectedNode.trafficDirection == Vector2Int.zero) ? "Serbest (Omni)" : selectedNode.trafficDirection.ToString();

                TrafficLight attachedLight = null;

                float searchRadius = GridManager.Instance.nodeRadius * 0.4f;
                Collider[] colliders = Physics.OverlapSphere(selectedNode.worldPosition, searchRadius);

                foreach (var col in colliders)
                {
                    TrafficLight light = col.GetComponent<TrafficLight>();
                    if (light == null) light = col.GetComponentInParent<TrafficLight>();

                    if (light != null)
                    {
                        attachedLight = light;
                        break;
                    }
                }

                string dynamicPenaltyInfo = "--- Dynamic Penalty ---\n";
                if (attachedLight != null)
                {
                    int currentPenalty = attachedLight.GetPenaltyCost();
                    string colorName = attachedLight.currentState.ToString();
                    float timeLeft = attachedLight.timeRemaining;

                    dynamicPenaltyInfo += $"Status: {colorName.ToUpper()}\n";
                    dynamicPenaltyInfo += $"Time: {timeLeft:F1} sn\n";
                    dynamicPenaltyInfo += $"PENALTY SCORE: {currentPenalty}";
                }
                else
                {
                    dynamicPenaltyInfo += "No Traffic Light (0)";
                }

                string info = $"--- NODE DEBUGGER ---\n" +
                              $"Grid: [{selectedNode.gridX}, {selectedNode.gridY}]\n" +
                              $"Walkable: {selectedNode.walkable}\n" +
                              $"Pos: {selectedNode.worldPosition.x:F1}, {selectedNode.worldPosition.y:F1}, {selectedNode.worldPosition.z:F1}\n" +
                              $"Direction: {dirInfo}\n\n" +
                              $"--- COST DATA ---\n" +
                              $"G Cost: {selectedNode.gCost}\n" +
                              $"H Cost: {selectedNode.hCost}\n" +
                              $"F Cost: {selectedNode.fCost}\n\n" +
                              dynamicPenaltyInfo;

                ShowDebugInfo(info);
                return;
            }
        }

        if (selectedNode != null)
        {
            selectedNode = null;
            ShowInstruction();
        }
    }

    private void ShowInstruction()
    {
        if (debugTextUI != null)
        {
            debugTextUI.text = defaultInstruction;
            debugTextUI.fontSize = instructionFontSize;
            debugTextUI.alignment = TextAlignmentOptions.Center;
        }
    }

    private void ShowDebugInfo(string info)
    {
        if (debugTextUI != null)
        {
            debugTextUI.text = info;
            debugTextUI.fontSize = debugFontSize;
            debugTextUI.alignment = TextAlignmentOptions.TopLeft;
        }
    }

    void OnDrawGizmos()
    {
        if (selectedNode != null && GridManager.Instance != null)
        {
            Gizmos.color = selectionColor;
            Gizmos.DrawWireCube(selectedNode.worldPosition + Vector3.up * 0.1f, Vector3.one * (GridManager.Instance.nodeRadius * 2));

            Gizmos.color = new Color(selectionColor.r, selectionColor.g, selectionColor.b, 0.3f);
            Gizmos.DrawCube(selectedNode.worldPosition + Vector3.up * 0.1f, Vector3.one * (GridManager.Instance.nodeRadius * 1.8f));
        }
    }
}