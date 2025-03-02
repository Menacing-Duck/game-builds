using UnityEngine;

public class CameraFol : MonoBehaviour
{
    public Transform target; 
    public Vector2 offset; 
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }
}