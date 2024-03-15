using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;


namespace UnityEngine.XR.Content.Interaction
{

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    public InputActionProperty showButton;
    public TextMeshProUGUI speedText;
    public float velocidadReduccion = 0.99f; // Variable pública para el factor de reducción de velocidad
    public float reduccionVelocidadPorSegundo = 1f; // Reducción de velocidad constante en metros por segundo

    //private float slowDownTimer = 0f;


    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle, Frenado;
    public int currentSpeedKMH; // Variable para almacenar la velocidad actual en km/h como un entero

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    public XRKnob xrKnobScript;

    [SerializeField]
    [Tooltip("The value of the knob")]
    [Range(0.0f, 5.0f)]
    float sensibilidad = 2f;


    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        CalculateSpeed(); // Calcular la velocidad actual en km/h
        
    }

    private void GetInput()
    {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        float value = xrKnobScript.value;
        
       horizontalInput = (value - 0.5f) * sensibilidad;







        // ----------------------------------------------------------------

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");

        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.Space);

        // Verificar qué botón se está presionando actualmente
        string gancho2L = "OculusTouchControllerOpenXRtriggerpressed";
        string gancho2R = "OculusTouchControllerOpenXR1triggerpressed";
        foreach (var control in showButton.action.controls)
        {
            if (control.IsPressed())
            {
                string boton = control.device.name + control.name;
                // Debug.Log(boton);

                if (boton == gancho2R )
                {
                    verticalInput = 1f; // Acelerar al presionar el gancho izquierdo
                }
                else if (boton ==gancho2L)
                {
                    isBreaking = true; // Frenar al presionar el gancho derecho
                    verticalInput = -1f;
                }
            }
        }
    }


private void HandleMotor()
{
    if (verticalInput == 0f) // Si no se está acelerando
    {
        // Reducir la velocidad gradualmente
        GetComponent<Rigidbody>().velocity -= GetComponent<Rigidbody>().velocity.normalized * reduccionVelocidadPorSegundo * Time.fixedDeltaTime;

        // Si la velocidad es muy baja, establecerla en cero para evitar valores residuales
        if (GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    else
    {
        // Si se está acelerando, aplicar la fuerza del motor
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
    }

    if (isBreaking) // Si se está frenando
    {
        currentbreakForce = breakForce * Frenado; // Frenar más fuerte
    }
    else
    {
        currentbreakForce = 0f; // No frenar
    }
    
    ApplyBreaking();
}






    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    // Método para calcular la velocidad actual en km/h
    private void CalculateSpeed()
{
    float speedMS = GetComponent<Rigidbody>().velocity.magnitude; // velocidad en metros/segundo
    float speedKMH = speedMS * 3.6f; // Convertir a km/h
    currentSpeedKMH = Mathf.RoundToInt(speedKMH); // Redondear a un entero

    // Asegurarse de que la variable speedText no sea nula
    if (speedText != null)
    {
        speedText.text = "" + currentSpeedKMH.ToString() + " km/h"; // Actualizar el texto de la velocidad
    }
}
}
}
