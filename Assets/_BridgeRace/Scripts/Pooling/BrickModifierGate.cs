using UnityEngine;

public class BrickModifierGate : MonoBehaviour
{
    [Header("Modifier")]
    [SerializeField]
    private int brickAmount;
    private bool isUsed;

    private void OnEnable()
    {
        isUsed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed)
        {
            return;
        }

        if (!other.TryGetComponent<IBrickModifierTarget>(out IBrickModifierTarget target))
        {
            return;
        }

        isUsed = true;

        if (brickAmount > 0)
        {
            target.AddBricks(brickAmount);
        }

        else if (brickAmount < 0)
        {
            target.RemoveBricks(Mathf.Abs(brickAmount));
        }

        Debug.Log(other.gameObject.name +" Modifier Gate kullandı: " +brickAmount +" | Yeni Stack: " +target.CurrentBrickCount);

        gameObject.SetActive(false);
    }
}