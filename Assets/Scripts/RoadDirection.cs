using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoadDirection : MonoBehaviour
{
    [Tooltip("Bu yolun aktigi yon (X, Y)")]
    public Vector2Int direction = new Vector2Int(0, 1); 

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); 

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = Color.white;
        Vector3 dir = new Vector3(direction.x, 0, direction.y);
        Gizmos.DrawLine(Vector3.zero, dir * .5f);
        // Gizmos.DrawSphere(dir * 1f, 0.5f); // Ok ucu
    }
}