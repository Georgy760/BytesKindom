using UnityEngine;

public class Turntable : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;


    public bool defaultAxis = true;
    public float rotationSpeed;
    public bool alwaysOn;
    private bool _activated;

    // Update is called once per frame
    private void Update()
    {
        if (alwaysOn || _activated)
        {
            if (defaultAxis)
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            else
                transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void Activate()
    {
        _activated = true;
    }
}