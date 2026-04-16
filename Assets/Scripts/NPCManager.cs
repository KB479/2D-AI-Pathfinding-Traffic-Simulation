using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public enum Direction { North, South, East, West }

    [Header("Ayarlar")]
    public GameObject npcPrefab; 
    public float spawnInterval = 3f;
    public int maxNPCCount = 10;

    [Header("Spawn Noktalarý")]
    public Transform spawnNorth;
    public Transform spawnSouth;
    public Transform spawnEast;
    public Transform spawnWest;

    [Header("Exit Noktalarý")]
    public Transform exitNorth;
    public Transform exitSouth;
    public Transform exitEast;
    public Transform exitWest;

    [Header("Kavþak Duraklarý")]
    public Transform wayNW; 
    public Transform wayNE; 
    public Transform waySW; 
    public Transform waySE; 

    private float timer = 0f;

    void Update()
    {
        int currentNPCs = FindObjectsOfType<NPCMotor>().Length;
        if (currentNPCs >= maxNPCCount) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandomNPC();
            timer = 0f;
        }
    }

    void SpawnRandomNPC()
    {
        Direction spawnDir = (Direction)Random.Range(0, 4);

        Direction exitDir;
        do
        {
            exitDir = (Direction)Random.Range(0, 4);
        } while (exitDir == spawnDir);

        Transform spawnPoint = GetSpawnTransform(spawnDir);
        if (spawnPoint == null) return; 

        List<Vector3> route = new List<Vector3>();

        switch (spawnDir)
        {
            case Direction.West: 
                if (exitDir == Direction.East) route.Add(exitEast.position); 
                else if (exitDir == Direction.North) { route.Add(waySE.position); route.Add(exitNorth.position); }
                else if (exitDir == Direction.South) { route.Add(waySW.position); route.Add(exitSouth.position); }
                break;

            case Direction.East: 
                if (exitDir == Direction.West) route.Add(exitWest.position); 
                else if (exitDir == Direction.North) { route.Add(wayNE.position); route.Add(exitNorth.position); }
                else if (exitDir == Direction.South) { route.Add(wayNW.position); route.Add(exitSouth.position); }
                break;

            case Direction.South: 
                if (exitDir == Direction.North) route.Add(exitNorth.position); 
                else if (exitDir == Direction.East) { route.Add(waySE.position); route.Add(exitEast.position); }
                else if (exitDir == Direction.West) { route.Add(wayNE.position); route.Add(exitWest.position); }
                break;

            case Direction.North: 
                if (exitDir == Direction.South) route.Add(exitSouth.position); 
                else if (exitDir == Direction.East) { route.Add(waySW.position); route.Add(exitEast.position); }
                else if (exitDir == Direction.West) { route.Add(wayNW.position); route.Add(exitWest.position); }
                break;
        }

        GameObject newNPC = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        newNPC.GetComponent<NPCMotor>().SetRoute(route);
    }

    Transform GetSpawnTransform(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return spawnNorth;
            case Direction.South: return spawnSouth;
            case Direction.East: return spawnEast;
            case Direction.West: return spawnWest;
            default: return null;
        }
    }
}