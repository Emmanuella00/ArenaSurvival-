using UnityEngine;

// =====================================================================
// WaypointPath - ALGORITHM 1: waypoint patrol + nearest-point search.
//
// Problem it solves: robots need a believable patrol route, and after
//   losing the player they should resume from the CLOSEST point instead
//   of walking all the way back to point 0.
//
// GetNextIndex  - O(1): one addition and one modulo, whatever the route size.
// GetNearestIndex - O(n): checks every point once (linear search).
//   Uses sqrMagnitude instead of Distance to skip the square root:
//   comparing squared distances gives the same answer, faster.
//
// Each robot keeps its OWN index (in PatrolState), so many robots can
// share one route without interfering.
// =====================================================================
public class WaypointPath : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    public int Count { get { return points == null ? 0 : points.Length; } }

    private void Awake()
    {
        // Convenience: if no points were dragged in, use this object's children
        if (points == null || points.Length == 0)
        {
            points = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) points[i] = transform.GetChild(i);
        }
    }

    public Vector3 GetPoint(int index)
    {
        return points[index].position;
    }

    // O(1): the point after "current", wrapping from the last back to the first
    public int GetNextIndex(int current)
    {
        return (current + 1) % points.Length;
    }

    // O(n): linear search for the closest point to "position"
    public int GetNearestIndex(Vector3 position)
    {
        int best = 0;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            float d = (points[i].position - position).sqrMagnitude;
            if (d < bestDistance)
            {
                bestDistance = d;
                best = i;
            }
        }
        return best;
    }

    // Draws the route in the Scene view (useful for the video)
    private void OnDrawGizmos()
    {
        Transform[] drawPoints = points;
        if (drawPoints == null || drawPoints.Length == 0)
        {
            drawPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) drawPoints[i] = transform.GetChild(i);
        }
        if (drawPoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < drawPoints.Length; i++)
        {
            if (drawPoints[i] == null) continue;
            Gizmos.DrawSphere(drawPoints[i].position, 0.3f);
            Transform next = drawPoints[(i + 1) % drawPoints.Length];
            if (next != null) Gizmos.DrawLine(drawPoints[i].position, next.position);
        }
    }
}
