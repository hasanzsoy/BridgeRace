using UnityEngine;

public class BrickModifierGate : MonoBehaviour
{
    // =====================================================
    // MODIFIER
    // =====================================================

    [Header("Modifier")]
    [SerializeField]
    private int brickAmount;


    // =====================================================
    // RUNTIME
    // =====================================================

    private bool isUsed;


    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        // Sahne yeniden başladığında
        // Gate tekrar kullanılabilir olsun.

        isUsed = false;
    }


    // =====================================================
    // TRIGGER
    // =====================================================

    private void OnTriggerEnter(
        Collider other)
    {
        // Gate daha önce kullanıldıysa
        // hiçbir şey yapma.

        if (isUsed)
        {
            return;
        }


        // =================================================
        // TAG / LAYER KULLANMIYORUZ
        //
        // Sadece IBrickModifierTarget olan
        // karakterleri kabul ediyoruz.
        // =================================================

        if (!other.TryGetComponent<IBrickModifierTarget>(
                out IBrickModifierTarget target))
        {
            return;
        }


        // =================================================
        // GATE ARTIK KULLANILDI
        //
        // Bunu efektten ÖNCE true yapıyoruz.
        // Böylece aynı anda ikinci karakter girerse
        // tekrar çalışmaz.
        // =================================================

        isUsed = true;


        // =================================================
        // POZİTİF GATE
        // Örnek: +7
        // =================================================

        if (brickAmount > 0)
        {
            target.AddBricks(
                brickAmount
            );
        }


        // =================================================
        // NEGATİF GATE
        // Örnek: -5 / -10
        // =================================================

        else if (brickAmount < 0)
        {
            target.RemoveBricks(
                Mathf.Abs(
                    brickAmount
                )
            );
        }


        // =================================================
        // DEBUG
        // =================================================

        Debug.Log(
            other.gameObject.name +
            " Modifier Gate kullandı: " +
            brickAmount +
            " | Yeni Stack: " +
            target.CurrentBrickCount
        );


        // =================================================
        // GATE'İ TAMAMEN KAPAT
        //
        // Script Gate_Center üzerinde olduğu için:
        //
        // - Collider
        // - Post_Left
        // - Post_Right
        // - Text
        // - bütün child objeler
        //
        // birlikte kapanır.
        // =================================================

        gameObject.SetActive(
            false
        );
    }
}