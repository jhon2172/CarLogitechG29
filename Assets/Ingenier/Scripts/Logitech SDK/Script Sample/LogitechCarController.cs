using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogitechCarController : MonoBehaviour
{
    [SerializeField]
    private LogitechExample logitechExample;
    string[] activeForceAndEffect;

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
    [SerializeField] public  int currentSpeedKMH;
    [SerializeField] private bool stateReverse = false;
        public float reduccionVelocidadPorSegundo = 1f; // Reducción de velocidad constante en metros por segundo


    //public TextMeshProUGUI speedText;
    
    

      private void Start() {
        
        activeForceAndEffect = new string[9];
        StartCoroutine(ActivateSpringForce());
    }
    private void FixedUpdate() {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        CalculateSpeed();
        
    }

    

      IEnumerator ActivateSpringForce()
    {
        // Esperar unos segundos antes de activar la fuerza de resorte
        yield return new WaitForSeconds(0.5f);

        // Activar la fuerza de resorte
        LogitechGSDK.LogiPlaySpringForce(0, 0, 30, 30);
        activeForceAndEffect[0] = "Spring Force\n ";
        LogitechGSDK.LogiPlayLeds(0, 20, 20, 20);
    }


    private void GetInput() {
        // Steering Input
        horizontalInput = Input.GetAxis("HorizontalLogitec");


        // > <

        verticalInput = Input.GetAxis("VerticalLogitec");
        if(logitechExample.CurrentGear!= -1){
            stateReverse = false;
        }
        
        // CAMBIOS 1 A 6
        if(logitechExample.CurrentGear == 1){
            
            if (logitechExample.GasInput > 0 && currentSpeedKMH < 24)
            {   
                
                verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
            }else{
                verticalInput = 0;
            }
        }

        if(logitechExample.CurrentGear == 2){
            if (logitechExample.GasInput > 0 && currentSpeedKMH > 8 && currentSpeedKMH < 45 )
            {
                verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
            }else{
                verticalInput = 0;
            }
        }

        if(logitechExample.CurrentGear == 3){
            if (logitechExample.GasInput > 0 && currentSpeedKMH > 18 && currentSpeedKMH < 65 )
            {
                verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
            }else{
                verticalInput = 0;
            }
        }

        if(logitechExample.CurrentGear == 4){
            if (logitechExample.GasInput > 0 && currentSpeedKMH > 38 && currentSpeedKMH < 80 )
            {
                verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
            }else{
                verticalInput = 0;
            }
        }
        if(logitechExample.CurrentGear == 5){
            if (logitechExample.GasInput > 0 && currentSpeedKMH > 50 && currentSpeedKMH < 100 )
            {
                verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
            }else{
                verticalInput = 0;
            }
        }

        if(logitechExample.CurrentGear == 6){
            if (logitechExample.GasInput > 0 && currentSpeedKMH > 70 && currentSpeedKMH < 150 )
            {
                verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
            }else{
                verticalInput = 0;
            }
        }
        
        // REVERSA
        if(logitechExample.CurrentGear == -1){
            stateReverse = true;
            
            if (logitechExample.GasInput > 0 )
            {
                verticalInput = Mathf.Lerp(0f, 1f, logitechExample.GasInput); // Acelerar al presionar el gancho izquierdo
            }else{
                verticalInput = 0;
            }
        }
        
        // Brake Input
        float brakeInput = logitechExample.BreakInput;
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
    else if(stateReverse == false)
    {
        // Si se está acelerando, aplicar la fuerza del motor
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce * -1;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce * -1 ;
    }else if(stateReverse == true){
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce * 1;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce * 1 ;
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
    }

    
}