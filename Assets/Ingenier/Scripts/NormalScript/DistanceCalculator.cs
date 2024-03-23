using UnityEngine;
using TMPro;

public class DistanceCalculator : MonoBehaviour
{
    public Transform pointB; // Punto B (puedes asignarlo desde el editor o por código si es necesario)
    public LineRenderer lineRenderer; // Referencia al Line Renderer
    public TextMeshProUGUI  distanceText; // Texto para mostrar la distancia

    void Update()
    {
        // Obtener la posición actual del jugador (punto A)
        Vector3 pointA = transform.position;

        // Actualizar la posición del punto A en el Line Renderer
        lineRenderer.SetPosition(0, pointA);

        // Obtener la posición de punto B
        Vector3 pointBPosition = pointB.position;

        // Actualizar la posición del punto B en el Line Renderer
        lineRenderer.SetPosition(1, pointBPosition);

        // Calcular la distancia entre los puntos A y B
        float distance = Vector3.Distance(pointA, pointBPosition);
        
        int roundedDistance = Mathf.RoundToInt(distance);


        // Mostrar la distancia en el texto de TextMeshPro
        distanceText.text = roundedDistance.ToString();
    }
}
