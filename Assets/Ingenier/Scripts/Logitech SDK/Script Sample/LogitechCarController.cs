using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogitechCarController : MonoBehaviour
{
    [SerializeField]
    private LogitechExample logitechExample;

    [SerializeField] private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    // Settings
    [SerializeField] 
    private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    [SerializeField] private  int currentSpeedKMH;
        public float reduccionVelocidadPorSegundo = 1f; // Reducción de velocidad constante en metros por segundo

    public TextMeshProUGUI speedText;
    

    private void Start() {
        LogitechGSDK.LogiPlaySpringForce(0, 0, 30, 30);
    }
    private void FixedUpdate() {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput() {
        // Steering Input
        horizontalInput = Input.GetAxis("HorizontalLogitec");
        verticalInput = Input.GetAxis("VerticalLogitec");

        // Acceleration Input
        if (logitechExample.GasInput > 0 )
        {
            verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
        }else{
            verticalInput = 0;
        }

        // Brake Input
        // Brake Input
        float brakeInput = logitechExample.BreakInput;


        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.Space);
        isBreaking = brakeInput > 0;
    }

    private void HandleMotor() {
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
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce * -1;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce * -1 ;
    }
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking() {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering() {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels() {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform) {
        Vector3 pos;
        Quaternion rot; 
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void CalculateSpeed()
    {
        float speedMS = GetComponent<Rigidbody>().velocity.magnitude; // velocidad en metros/segundo
        float speedKMH = speedMS * 3.6f; // Convertir a km/h
        currentSpeedKMH = Mathf.RoundToInt(speedKMH); // Redondear a un entero

         //Asegurarse de que la variable speedText no sea nula
        if (speedText != null)
        {
            speedText.text = "" + currentSpeedKMH.ToString() + " km/h"; // Actualizar el texto de la velocidad
            
        }
    }
}