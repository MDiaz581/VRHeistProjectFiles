using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform CameraTarget;
    public float sSpeed = 10.0f;
    public Vector3 dist;
    public Transform lookTarget;
    void FixedUpdate()
    {
        Vector3 dPos = CameraTarget.position + dist;
        Vector3 sPos = Vector3.Lerp(transform.position, dPos, sSpeed * Time.deltaTime);
        transform.position = sPos;
        transform.LookAt(lookTarget.position);
    }
}
