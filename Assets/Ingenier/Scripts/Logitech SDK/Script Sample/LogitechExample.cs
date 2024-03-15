using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LogitechExample : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;
        public float maxRotationAngle = 360f; // Define el rango máximo de rotación del volante


    public float xAxes,GasInput,BreakInput,ClutchInput;
    public bool Hshift = true;

    public int CurrentGear;
    public string CurrentGearText;

    public float rotationSpeed = 0.5f; // Velocidad de rotación del volante
    public float returnSpeed = 0.5f; // Velocidad de retorno del volante al centro

    public GameObject steeringWheel; // Objeto del volante que se rotará
     public TextMeshProUGUI SpeedShiftText;

    private void Start() {

            properties.wheelRange = 0;
            properties.forceEnable = true;
    }

    //r (><) 
    private void Update() {
        if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)){
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);
            HShifter(rec);
            RotateSteeringWheel(rec);


            xAxes = rec.lX / 16384; // -1 0 1
            if(rec.lY > 0){
                GasInput = 0;
            }else if(rec.lY < 0){
                GasInput = rec.lY / -32768f;
            }

            if(rec.lRz >0){
                BreakInput = 0;
            }else if(rec.lRz < 0){
                BreakInput = rec.lRz / -32768f;
            }

            if(rec.rglSlider[0] >0){
                ClutchInput = 0;
            }else if(rec.rglSlider[0] < 0){
                ClutchInput = rec.rglSlider[0] / -32768f;
            }
            


        }else{
            Debug.Log("No streering wheel connected!");
        }
    }

    void HShifter(LogitechGSDK.DIJOYSTATE2ENGINES shifter){

        for(int i=0; i< 128; i++){
            if(shifter.rgbButtons[i]==128){
                if(ClutchInput > 0.5f){
                    if(i ==12){
                        CurrentGear = 1;
                        CurrentGearText = "1";
                    }else if(i ==13){
                        CurrentGear = 2;
                        CurrentGearText = "2";
                            
                    }else if(i ==14){
                        CurrentGear = 3;
                        CurrentGearText = "3";
                    }else if(i ==15){
                        CurrentGear = 4;
                        CurrentGearText = "4";
                    }else if(i ==16){
                        CurrentGear = 5;
                        CurrentGearText = "5";
                    }else if(i ==17){
                        CurrentGear = 6;
                        CurrentGearText = "6";
                    }
                    else if(i ==18){
                        CurrentGear = -1;
                        CurrentGearText = "R";
                    }
                }
            }
        }

        SpeedShiftText.text = CurrentGearText ; // Actualizar el texto de la velocidad
    }

     private void RotateSteeringWheel(LogitechGSDK.DIJOYSTATE2ENGINES rec)
{
    // Obtener el valor del eje horizontal del volante Logitech y ajustar la velocidad de rotación
    float horizontalInput = rec.lX / 32768f;

    // Calcular la rotación deseada basada en el valor del eje horizontal
    float targetRotationZ = horizontalInput * maxRotationAngle * 2;

    // Calcular la rotación actual del GameObject del volante
    float currentRotationZ = steeringWheel.transform.localEulerAngles.z;

    // Calcular la rotación que se debe aplicar en este frame
    float rotationAmount = Mathf.MoveTowardsAngle(currentRotationZ, targetRotationZ, rotationSpeed * Time.deltaTime) - currentRotationZ;

    // Rotar el GameObject del volante si está asignado
    if (steeringWheel != null)
    {
        // Aplicar la rotación
        steeringWheel.transform.Rotate(0f, 0f, rotationAmount);
    }
    else
    {
        Debug.LogWarning("No se ha asignado un GameObject para el volante en el script SteeringWheelController.");
    }
}

        // COLOCAR AL TEXT LA MARCHA EN LA QUE SE VA 
       
}
