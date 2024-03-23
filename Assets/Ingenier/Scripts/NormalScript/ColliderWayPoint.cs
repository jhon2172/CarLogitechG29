using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderWayPoint : MonoBehaviour
{
    // Start is called before the first frame update
    private WaypointManager waypointManager;
            public int waypointIndexToDelete;


     private void Start()
    {
        // Busca el objeto con el script WaypointManager y obtén una referencia a él
        waypointManager = FindObjectOfType<WaypointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que ha colisionado tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            // Verifica si el WaypointManager se encontró correctamente
            if (waypointManager != null)
            {
                // Verifica si la lista de waypoints no está vacía
                if (waypointManager.waypoints.Count > 0)
                {
                    // Elimina el primer waypoint de la lista
                    Transform firstWaypoint = waypointManager.waypoints[0];
                    Destroy(firstWaypoint.gameObject);
                    waypointManager.waypoints.RemoveAt(0);
                    Debug.Log("Se ha eliminado el primer waypoint de la lista.");
                }
                else
                {
                    Debug.LogWarning("La lista de waypoints está vacía.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró el WaypointManager.");
            }
        }
    }

}
