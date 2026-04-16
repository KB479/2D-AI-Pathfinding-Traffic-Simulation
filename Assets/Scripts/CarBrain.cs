using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMotor))]
public class CarBrain : MonoBehaviour
{
    private CarMotor motor;
    private Vector3 currentTargetPos;
    private bool hasTarget = false;
    private bool isThinking = false;

    public enum SearchAlgorithm { AStar, BFS, Greedy }
    [HideInInspector] public SearchAlgorithm currentAlgorithm = SearchAlgorithm.AStar;

    public List<Node> GetActivePath()
    {
        return motor != null ? motor.GetActivePath() : null;
    }

    private Dictionary<Node, int> knownPenalties = new Dictionary<Node, int>();

    void Awake()
    {
        motor = GetComponent<CarMotor>();
    }

    public void SetTarget(Vector3 targetPosition)
    {
        currentTargetPos = targetPosition;
        hasTarget = true;
        //CalculatePath(false);
    }

    public void StartSimulation()
    {
        motor.StartMotor();
    }

    public void TogglePause()
    {
        motor.ToggleMotorPause();
    }

    public void CalculatePath(bool autoStart = true)
    {
        if (!hasTarget) return;

        List<Node> path = null;


        switch (currentAlgorithm)
        {
            case SearchAlgorithm.AStar:
                path = PathfindingSystem.FindPathAStar(transform.position, currentTargetPos, knownPenalties);
                break;
            case SearchAlgorithm.BFS:
                path = PathfindingSystem.FindPathBFS(transform.position, currentTargetPos);
                break;
            case SearchAlgorithm.Greedy:
                path = PathfindingSystem.FindPathGreedy(transform.position, currentTargetPos);
                break;
        }

        if (path != null && path.Count > 0)
        {
            motor.SetPath(path, autoStart);
        }
        else
        {
            Debug.LogWarning("CarBrain: Gidilecek gecerli bir yol bulunamadý!");
            motor.StopMotor();
        }
    }

    public void OnObstacleDetected(TrafficLight light)
    {
        if (isThinking) return;
        StartCoroutine(ThinkAndDecide(light));
    }

    private IEnumerator ThinkAndDecide(TrafficLight light)
    {
        isThinking = true;

        Node obstacleNode = light.associatedNode;
        int penaltyCost = light.GetPenaltyCost();
        float timeLeft = light.timeRemaining;

        // a* deðilse körleme bekle

        if (currentAlgorithm != SearchAlgorithm.AStar)
        {
            Debug.Log($"\n<color=#ff5555>[YAPAY ZEKA - GÖZLEM]</color> Kýrmýzý Iþýk Algýlandý! Mevcut Algoritma: {currentAlgorithm}");
            Debug.Log($"<color=#aaaaaa>[YAPAY ZEKA - KARAR]</color> Bu algoritma maliyet (ceza) hesaplayamaz. Yeþile dönmesi bekleniyor... ({timeLeft:F1} sn)\n");

            isThinking = false;
            yield break; 
        }


        // Hafýzayý güncelle
        if (!knownPenalties.ContainsKey(obstacleNode))
            knownPenalties.Add(obstacleNode, penaltyCost);
        else
            knownPenalties[obstacleNode] = penaltyCost;

        Debug.Log($"\n<color=#ff5555>[YAPAY ZEKA - GÖZLEM]</color> Dinamik engel (Kýrmýzý Iþýk) tespit edildi! Koordinat: ({obstacleNode.gridX}, {obstacleNode.gridY})");
        Debug.Log($"<color=#ffaa00>[YAPAY ZEKA - ANALÝZ]</color> Iþýðýn yeþile dönmesine {timeLeft:F1} saniye var. Algýlanan Ceza Puaný: {penaltyCost}");
        Debug.Log($"<color=#ffff55>[YAPAY ZEKA - KARAR AÞAMASI]</color> Araç 1 saniye boyunca alternatif rotalarý deðerlendiriyor...\n");

        yield return new WaitForSeconds(1f);

        Debug.Log("<color=#55ffff>[YAPAY ZEKA - HESAPLAMA]</color> Yeni rota için A* algoritmasý tetiklendi...");

        CalculatePath(true);

        List<Node> activePath = motor.GetActivePath();
        if (activePath != null && activePath.Count > 0)
        {
            if (activePath.Contains(obstacleNode))
            {
                Debug.Log($"<color=#ff55ff>[YAPAY ZEKA - SONUÇ]</color> Alternatif yollarýn maliyeti bekleme cezasýndan daha yüksek. Karar: <b>BEKLE</b> (Mevcut rota korunuyor).");
            }
            else
            {
                Debug.Log($"<color=#55ff55>[YAPAY ZEKA - SONUÇ]</color> Daha ucuz bir alternatif rota bulundu! Karar: <b>DOLAN</b> (Direksiyon çevriliyor).");
            }
        }

        isThinking = false;
    }
}