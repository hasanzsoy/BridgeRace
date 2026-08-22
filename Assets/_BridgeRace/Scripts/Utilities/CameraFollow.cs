using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 18f, -12f);

    [SerializeField]
    private float followSpeed = 5f;

    private bool followEnabled = true;


    private void LateUpdate()
    {
        if (!followEnabled)
        {
            return;
        }


        if (target == null)
        {
            return;
        }


        Vector3 targetPosition =
            target.position + offset;


        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed *
                Time.deltaTime
            );
    }


    public void SetFollowEnabled(
        bool enabled)
    {
        followEnabled = enabled;
    }
}