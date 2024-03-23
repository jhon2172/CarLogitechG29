using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public List<Vector3> waypointPosition = new List<Vector3>();
    public Material lineMaterial;
    public Color lineColor = Color.red;
    public float lineWidth = 1f;
    public int selectedWaypointIndex = 0; // Nuevo campo para almacenar el índice del waypoint seleccionado
    public int select01 = 0; // Nuevo campo para almacenar el índice del primer waypoint seleccionado
    public int select02 = 0; // Nuevo campo para almacenar el índice del segundo waypoint seleccionado

    public Transform pointB; // Punto B (puedes asignarlo desde el editor o por código si es necesario)
    public Transform pointAA; // Punto B (puedes asignarlo desde el editor o por código si es necesario)

    public LineRenderer lineRenderer; // Referencia al Line Renderer
    public TextMeshProUGUI  distanceText; // Texto para mostrar la distancia
    public Vector3 cubeSize = new Vector3(2f, 2f, 2f); // Tamaño predeterminado del cubo
    public bool isTrigger = false;
    public Material cubeMaterial;




    // Método para agregar un nuevo waypoint (cubo)
     void Start()
    {
        

    }

    void principal(){

        UpdateWaypointPositions();
        
        //PrintWaypointPositions();


        // Establecer las posiciones de los puntos en el LineRenderer
        lineRenderer.positionCount = waypointPosition.Count; // Establecer el número de puntos en el LineRenderer


        // Establecer las posiciones de los puntos en el LineRenderer
        for (int i = 0; i < waypointPosition.Count; i++)
        {
            if (i < lineRenderer.positionCount) // Verificar que el índice esté dentro del rango del LineRenderer
            {
                lineRenderer.SetPosition(i, waypointPosition[i]);
                //Debug.Log("WPos: "+i+" , "+ waypointPosition[i]);
            }
            else
            {
                Debug.LogWarning("Index out of bounds: " + i);
            }
        }
    }

    
    void Update()
    {
        // Obtener la posición actual del jugador (punto A)
        Vector3 pointA = transform.position;

        

        // Actualizar la posición del punto A en el Line Renderer
       //lineRenderer.SetPosition(0, pointA);

        // Obtener la posición de punto B
        Vector3 pointBPosition = pointB.position;

        // Actualizar la posición del punto B en el Line Renderer
        //lineRenderer.SetPosition(1, pointBPosition);

        // Calcular la distancia entre los puntos A y B
        float distance = Vector3.Distance(pointA, pointBPosition);
        
        int roundedDistance = Mathf.RoundToInt(distance);

        principal();
    

        // Mostrar la distancia en el texto de TextMeshPro
        distanceText.text = roundedDistance.ToString();
    }

    void UpdateWaypointPositions()
    {
        waypointPosition.Clear(); // Limpiar la lista antes de actualizar las posiciones
        Vector3 pointA = pointAA.position;
         waypointPosition.Add(pointA);
         

        foreach (Transform waypoint in waypoints)
        {
            
           // waypointPosition.Add(pointA); // Agregar la posición de cada waypoint a la lista
            waypointPosition.Add(waypoint.position); // Agregar la posición de cada waypoint a la lista
        }

        Vector3 pointBPosition = pointB.position;
        waypointPosition.Add(pointBPosition);


    }

    void PrintWaypointPositions()
    {
        //Debug.Log("Posiciones de los waypoints:");
        for (int i = 0; i < waypointPosition.Count; i++)
        {
           // Debug.Log("Waypoint " + i + ": " + waypointPosition[i]);
        }
    }
    public void AddWaypoint()
    {
        GameObject newWaypointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newWaypointObj.name = "Waypoint " + (waypoints.Count + 1);
        newWaypointObj.transform.position = transform.position;
            newWaypointObj.transform.localScale = cubeSize; // Cambia los valores según necesites

        newWaypointObj.transform.parent = transform;

        // Obtener el renderer del cubo
        Renderer renderer = newWaypointObj.GetComponent<Renderer>();

        // Verificar si se ha asignado un material personalizado
        if (cubeMaterial != null)
        {
            // Asignar el material al renderer del cubo
            renderer.material = cubeMaterial;
        }


        // Establecer si el cubo es un trigger
        Collider collider = newWaypointObj.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = isTrigger;
        }

        newWaypointObj.AddComponent<ColliderWayPoint>();
        waypoints.Add(newWaypointObj.transform);
        
    }

    // Método para borrar todos los waypoints
    public void DeleteAllWaypoints()
    {
        foreach (Transform waypoint in waypoints)
        {
            DestroyImmediate(waypoint.gameObject);
        }
        waypoints.Clear();
    }

    // Método para conectar dos waypoints
    public void ConnectWaypoints(Transform waypoint1, Transform waypoint2)
    {
        Handles.color = lineColor;
        Handles.DrawLine(waypoint1.position, waypoint2.position);
        Debug.Log("Conectando waypoints...");
    }

    // Método para borrar un waypoint seleccionado
    public void DeleteSelectedWaypoint()
    {
        if (selectedWaypointIndex >= 0 && selectedWaypointIndex < waypoints.Count)
        {
            DestroyImmediate(waypoints[selectedWaypointIndex].gameObject);
            waypoints.RemoveAt(selectedWaypointIndex);
            selectedWaypointIndex = -1;
        }
    }

    
     
}

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : Editor
{
    SerializedProperty lineMaterialProp;
    SerializedProperty lineColorProp;
    SerializedProperty lineWidthProp;
    SerializedProperty selectedWaypointIndexProp; // Nuevo campo para el índice del waypoint seleccionado
    SerializedProperty select01Prop; // Nuevo campo para el índice del primer waypoint seleccionado
    SerializedProperty select02Prop; // Nuevo campo para el índice del segundo waypoint seleccionado

    private void OnEnable()
    {
        lineMaterialProp = serializedObject.FindProperty("lineMaterial");
        lineColorProp = serializedObject.FindProperty("lineColor");
        lineWidthProp = serializedObject.FindProperty("lineWidth");
        selectedWaypointIndexProp = serializedObject.FindProperty("selectedWaypointIndex"); // Asignar el campo del índice del waypoint seleccionado
        //select01Prop = serializedObject.FindProperty("select01"); // Asignar el campo del índice del primer waypoint seleccionado
        //select02Prop = serializedObject.FindProperty("select02"); // Asignar el campo del índice del segundo waypoint seleccionado
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WaypointManager waypointManager = (WaypointManager)target;

        //EditorGUILayout.PropertyField(lineMaterialProp);
        //EditorGUILayout.PropertyField(lineColorProp);
        //EditorGUILayout.PropertyField(lineWidthProp);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Add Waypoint"))
        {
            waypointManager.AddWaypoint();
        }

        if (GUILayout.Button("Delete All Waypoints"))
        {
            waypointManager.DeleteAllWaypoints();
        }

        if (GUILayout.Button("Connect Waypoints"))
        {
            int waypointCount = waypointManager.waypoints.Count;
            if (waypointCount >= 2)
            {
                // Mostrar las listas desplegables para seleccionar los waypoints
                if (waypointManager.waypoints.Count >= 2)
                {
                    select01Prop.intValue = EditorGUILayout.Popup("Select Waypoint 1", select01Prop.intValue, GetWaypointNames(waypointManager.waypoints));
                    int selectedWaypointIndex1 = select01Prop.intValue;

                    select02Prop.intValue = EditorGUILayout.Popup("Select Waypoint 2", select02Prop.intValue, GetWaypointNames(waypointManager.waypoints));
                    int selectedWaypointIndex2 = select02Prop.intValue;

                    if (selectedWaypointIndex1 != selectedWaypointIndex2)
                    {
                        Transform waypoint1 = waypointManager.waypoints[selectedWaypointIndex1];
                        Transform waypoint2 = waypointManager.waypoints[selectedWaypointIndex2];
                        
                    }
                    else
                    {
                        Debug.LogWarning("Please select two different waypoints to connect.");
                    }
                }
                else
                {
                    Debug.LogWarning("There must be at least two waypoints to connect.");
                }
            }
        }

        if (GUILayout.Button("Delete Selected Waypoint"))
        {
            // Setea el índice del waypoint seleccionado
            waypointManager.DeleteSelectedWaypoint();
        }
    }

    private string[] GetWaypointNames(List<Transform> waypoints)
    {
        string[] waypointNames = new string[waypoints.Count];
        for (int i = 0; i < waypoints.Count; i++)
        {
            waypointNames[i] = waypoints[i].name;
        }
        return waypointNames;
    }

    private void OnSceneGUI()
    {
        WaypointManager waypointManager = (WaypointManager)target;

        if (waypointManager.waypoints.Count < 2)
        {
            return;
        }

        Vector3[] positions = new Vector3[waypointManager.waypoints.Count];
        for (int i = 0; i < waypointManager.waypoints.Count; i++)
        {
            positions[i] = waypointManager.waypoints[i].position;
        }

        Handles.color = waypointManager.lineColor;
        Handles.DrawAAPolyLine(waypointManager.lineWidth, positions);
    }
}
