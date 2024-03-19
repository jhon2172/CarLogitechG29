using UnityEngine;
using System.Collections.Generic;

public class WaypointSystem : MonoBehaviour
{
    public GameObject waypointPrefab; // Prefab del waypoint
    public Color connectionColor = Color.red; // Color de la conexión entre waypoints

    private List<Transform> waypoints = new List<Transform>(); // Lista de waypoints

    // Método para agregar un nuevo waypoint
    public void AddWaypoint()
    {
        GameObject newWaypoint = Instantiate(waypointPrefab, Vector3.zero, Quaternion.identity);
        newWaypoint.transform.SetParent(transform);
        waypoints.Add(newWaypoint.transform);
    }

    // Método para conectar dos waypoints
    public void ConnectWaypoints(Transform waypoint1, Transform waypoint2)
    {
        // Dibujar una línea entre los dos waypoints
        Debug.DrawLine(waypoint1.position, waypoint2.position, connectionColor);
    }

    // Método para eliminar todos los waypoints
    public void DeleteWaypoints()
    {
        foreach (Transform waypoint in waypoints)
        {
            Destroy(waypoint.gameObject);
        }
        waypoints.Clear();
    }

    // Método para eliminar un waypoint específico
    public void DeleteSelectedWaypoint(Transform waypoint)
    {
        if (waypoints.Contains(waypoint))
        {
            waypoints.Remove(waypoint);
            Destroy(waypoint.gameObject);
        }
    }
}
