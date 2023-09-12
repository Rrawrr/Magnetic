using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MagnetoMeter : MonoBehaviour
{
    [SerializeField] private Image transparentImage;
    [SerializeField] private TextMeshProUGUI sliderValueLabel;
    [SerializeField] private Slider slider;
    [SerializeField] private int numberOfValues;

    private float sensorStartValue;
    private float sensorCurrentValue;
    private float sensorDelta;

    private void Awake()
    {
        Input.location.Start();
        Input.compass.enabled = true;

        if (MagneticFieldSensor.current != null)
        {
            InputSystem.EnableDevice(MagneticFieldSensor.current);
        }
        else
        {
            Debug.LogError($"MagneticFieldSensor.current is null");
        }

        ShowSliderValue(slider.value);
        slider.onValueChanged.AddListener(ShowSliderValue);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(ShowSliderValue);
    }

    private void ShowSliderValue(float value)
    {
        sliderValueLabel.text = value.ToString();
    }

    private IEnumerator Start()
    {
        var sensorStartValueArray = new float[30];
        float sum = 0;
        for (var i = 0; i < sensorStartValueArray.Length; i++)
        {
            yield return null;
            sensorStartValueArray[i] = MagneticFieldSensor.current.magneticField.EvaluateMagnitude();
            Debug.Log($"StartValueAdded: {sensorStartValueArray[i]}");
            sum += sensorStartValueArray[i];
        }

        sensorStartValue = sum / sensorStartValueArray.Length;
        Debug.Log($"========= SensorStartValue: {sensorStartValue}");

        StartCoroutine(CalculateCurrentValueCoroutine());
    }

    private IEnumerator CalculateCurrentValueCoroutine()//TODO refactor and optimize method
    {
        while (true)
        {
            var sensorCurrentValueArray = new float[numberOfValues];
            float sumCurrent = 0;
            for (var i = 0; i < sensorCurrentValueArray.Length; i++)
            {
                sensorCurrentValueArray[i] = MagneticFieldSensor.current.magneticField.EvaluateMagnitude();
                Debug.Log($"CurrentValueAdded: {sensorCurrentValueArray[i]}");
                sumCurrent += sensorCurrentValueArray[i];
                yield return null;
            }

            sensorCurrentValue = sumCurrent / sensorCurrentValueArray.Length;
            Debug.Log($"========= SensorCurrentValue: {sensorCurrentValue}");

            SetImageAlpha();

            yield return null;
        }
    }

    private void SetImageAlpha()
    {
        sensorDelta = Mathf.Abs(sensorCurrentValue - sensorStartValue);
        Debug.Log($"========= SensorDelta: {sensorDelta}");

        if (sensorDelta > slider.value)
        {
            var alphaValue = Mathf.Clamp(sensorDelta / slider.maxValue, 0, 1);
            transparentImage.CrossFadeAlpha(1 - alphaValue, 0.25f, true);
        }
        else
        {
            transparentImage.CrossFadeAlpha(1, 0.25f, true);
        }
    }

    void OnGUI()
    {
        //GUILayout.Label("Magnetometer reading: " + Input.compass.rawVector.ToString());
        GUILayout.Label($"MagneticFieldSensor EvaluateMagnitude: {MagneticFieldSensor.current.magneticField.EvaluateMagnitude()}");
        GUILayout.Label($"StartValue: {sensorStartValue}");
        GUILayout.Label($"CurrentValue: {sensorCurrentValue}");
        GUILayout.Label($"SensorDelta: {sensorDelta}");
    }


}
