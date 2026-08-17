using UnityEngine;

public class BrickAreaActivator : MonoBehaviour
{
    [Header("Aktifleşecek Brick Spawner")]
    [SerializeField] private BrickSpawner brickSpawner;


    private void Awake()
    {
        if (brickSpawner == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde BrickSpawner atanmadı!"
            );
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (brickSpawner == null)
        {
            return;
        }


      
        if (!other.TryGetComponent<IRacer>(
                out IRacer racer))
        {
            return;
        }


        brickSpawner.ActivateColor(
            racer.RacerColor
        );
    }
}