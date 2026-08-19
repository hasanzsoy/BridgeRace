using System.Collections.Generic;
using UnityEngine;

public class BrickModifierGate : MonoBehaviour
{
    [Header("Modifier")]
    [SerializeField] private int brickAmount;

    [Header("Gate Settings")]
    [SerializeField] private bool oneUsePerCharacter = true;


    private HashSet<IBrickModifierTarget>
        usedCharacters =
            new HashSet<IBrickModifierTarget>();


    private void OnEnable()
    {
        usedCharacters.Clear();
    }


    private void OnTriggerEnter(
        Collider other)
    {
        // Tag / Layer kontrolü yok.
        // Interface üzerinden algılıyoruz.
        if (!other.TryGetComponent<IBrickModifierTarget>(
                out IBrickModifierTarget target))
        {
            return;
        }


        // Aynı karakter aynı Gate'i daha önce
        // kullandıysa tekrar çalışmasın.
        if (oneUsePerCharacter &&
            usedCharacters.Contains(target))
        {
            return;
        }


        // Pozitif değer:
        // +7 gibi Brick ekler.
        if (brickAmount > 0)
        {
            target.AddBricks(
                brickAmount
            );
        }


        // Negatif değer:
        // -5 / -10 gibi Brick azaltır.
        else if (brickAmount < 0)
        {
            target.RemoveBricks(
                Mathf.Abs(brickAmount)
            );
        }


        if (oneUsePerCharacter)
        {
            usedCharacters.Add(
                target
            );
        }


        Debug.Log(
            other.gameObject.name +
            " Modifier Gate: " +
            brickAmount +
            " | Yeni Stack: " +
            target.CurrentBrickCount
        );
    }
}