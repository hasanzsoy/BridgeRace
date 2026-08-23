using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;


    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            return;
        }

        transform.rotation =Quaternion.LookRotation(targetCamera.transform.forward,targetCamera.transform.up);
    }
}