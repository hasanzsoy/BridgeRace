using UnityEngine;

public class Brick : MonoBehaviour, ICollectable
{
    [Header("Brick Settings")]
    [SerializeField] private TeamColor brickColor;

    public TeamColor CollectableColor => brickColor;


    public void Collect(CharacterBase collector)
    {
        // Gün 7'de tuğla toplama ve stacking sistemi burada çalışacak.
    }
}