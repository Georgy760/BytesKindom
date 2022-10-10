using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public bool playerDetected;

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerDetected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerDetected = false;
    }
}