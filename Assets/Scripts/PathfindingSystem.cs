using System.Collections.Generic;
using UnityEngine;

public static class PathfindingSystem
{
    public static List<Node> FindPathAStar(Vector3 startPos, Vector3 targetPos, Dictionary<Node, int> knownPenalties = null)
    {
        GridManager grid = GridManager.Instance;
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable) return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                   (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                List<Node> finalPath = RetracePath(startNode, targetNode);
                Debug.Log($"<color=#00ff00>[A* ALGORÝTMASI]</color> Rota baþarýyla çizildi. Taranan Node: {closedSet.Count} | Adým Sayýsý: {finalPath.Count} | Toplam Maliyet (G-Cost): {currentNode.gCost}");
                return finalPath;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                int dynamicPenalty = 0;
                if (knownPenalties != null && knownPenalties.ContainsKey(neighbor))
                {
                    dynamicPenalty = knownPenalties[neighbor];
                }

                int newMovementCostToNeighbor = currentNode.gCost + 10 + dynamicPenalty;

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetManhattanDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        Debug.LogWarning("<color=red>[A* ALGORÝTMASI]</color> Geçerli bir rota bulunamadý! Hedef tamamen kapalý veya ulaþýlamaz durumda.");
        return null;
    }

    public static List<Node> FindPathBFS(Vector3 startPos, Vector3 targetPos)
    {
        GridManager grid = GridManager.Instance;
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable) return null;

        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        int exploredCount = 0;

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            exploredCount++;

            if (currentNode == targetNode)
            {
                List<Node> finalPath = RetracePath(startNode, targetNode);
                Debug.Log($"<color=#00ffff>[BFS ALGORÝTMASI]</color> Rota çizildi. Taranan Node: {exploredCount} | Adým Sayýsý: {finalPath.Count}");
                return finalPath;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || visited.Contains(neighbor)) continue;

                neighbor.parent = currentNode;
                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }
        Debug.LogWarning("<color=red>[BFS ALGORÝTMASI]</color> Geçerli bir rota bulunamadý!");
        return null;
    }

    public static List<Node> FindPathGreedy(Vector3 startPos, Vector3 targetPos)
    {
        GridManager grid = GridManager.Instance;
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable) return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                List<Node> finalPath = RetracePath(startNode, targetNode);
                Debug.Log($"<color=#ffaa00>[GREEDY ALGORÝTMASI]</color> Rota çizildi. Taranan Node: {closedSet.Count} | Adým Sayýsý: {finalPath.Count}");
                return finalPath;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                if (!openSet.Contains(neighbor))
                {
                    neighbor.hCost = GetManhattanDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    openSet.Add(neighbor);
                }
            }
        }
        Debug.LogWarning("<color=red>[GREEDY ALGORÝTMASI]</color> Geçerli bir rota bulunamadý!");
        return null;
    }

    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private static int GetManhattanDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return 10 * (dstX + dstY);
    }
}