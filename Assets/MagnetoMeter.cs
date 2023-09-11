using UnityEngine;

public class MagnetoMeter : MonoBehaviour
{
    void Awake()
    {
        Input.location.Start();
        Input.compass.enabled = true;
    }

    void OnGUI()
    {
        GUILayout.Label("Magnetometer reading: " + Input.compass.rawVector.ToString());
        Debug.Log(Input.compass.rawVector.ToString());

    }

}
