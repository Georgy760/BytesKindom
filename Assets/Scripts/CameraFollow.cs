using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform currentTarget;

    [SerializeField] private Vector3 offset;

    [SerializeField] private bool fixedY;

    [SerializeField] private float fixedYVal;


    private Vector3 targetPos;


    // Update is called once per frame
    private void Update()
    {
        targetPos = currentTarget.position + offset;
        if (fixedY) targetPos.y = fixedYVal;

        transform.position = targetPos;
    }
}