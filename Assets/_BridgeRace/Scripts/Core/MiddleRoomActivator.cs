using UnityEngine;

public class MiddleRoomActivator : MonoBehaviour
{
    [Header("Middle Brick Spawner")]
    [SerializeField] private BrickSpawner middleSpawner;


    private void Awake()
    {
        if (middleSpawner == null)
        {
            Debug.LogError(
                gameObject.name +
                " için Middle Spawner atanmadı!"
            );
        }
    }


    private void OnTriggerEnter(
        Collider other)
    {
        if (middleSpawner == null)
        {
            return;
        }


        if (!other.TryGetComponent<IRacer>(
                out IRacer racer))
        {
            return;
        }


        middleSpawner.ActivateColor(
            racer.RacerColor
        );
    }
}