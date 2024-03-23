using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInside2 : MonoBehaviour {

	public Transform MinimapCam;
	public float MinimapSize;
	Vector3 TempV3;
	    public GameObject targetObject;


	void Update () {
		TempV3 = transform.parent.transform.position;
		TempV3.y = transform.position.y;
		transform.position = TempV3;
		Rotate();
	}

	void LateUpdate () {
		transform.position = new Vector3 (
			Mathf.Clamp(transform.position.x, MinimapCam.position.x-MinimapSize, MinimapSize+MinimapCam.position.x),
			transform.position.y,
			Mathf.Clamp(transform.position.z, MinimapCam.position.z-MinimapSize, MinimapSize+MinimapCam.position.z)
		);
	}

	void Rotate(){
		// Verificar si el objeto de destino se ha asignado
        if (targetObject != null)
        {
             float targetRotationZ = targetObject.transform.rotation.eulerAngles.z;

            // Aplicar la misma rotación en el eje Z al objeto que contiene este script
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetRotationZ);
        }
        else
        {
            Debug.LogError("No se ha asignado ningún GameObject de destino para obtener la rotación.");
        }
	}
}
