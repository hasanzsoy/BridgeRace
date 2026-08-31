using UnityEngine;

public class CharacterCollisionDetector : MonoBehaviour
{
    // =====================================================
    // IMPACT SETTINGS
    // =====================================================

    [Header("Impact Settings")]

    [SerializeField]
    private float minimumImpactSpeed = 3.5f;

    [SerializeField]
    private float horizontalKnockbackForce = 6.5f;

    [SerializeField]
    private float upwardKnockbackForce = 1.8f;


    // =====================================================
    // REFERENCES
    // =====================================================

    private CharacterStack myStack;

    private CharacterBase myCharacter;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        myStack =
            GetComponent<CharacterStack>();


        myCharacter =
            GetComponent<CharacterBase>();


        if (myStack == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterStack bulunamadı!"
            );
        }


        if (myCharacter == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterBase bulunamadı!"
            );
        }
    }


    // =====================================================
    // COLLISION
    // =====================================================

    private void OnCollisionEnter(
        Collision collision)
    {
        // =================================================
        // ÇARPTIĞIMIZ NESNE YARIŞÇI MI?
        // =================================================

        if (!collision.collider
            .TryGetComponent<CharacterBase>(
                out CharacterBase otherCharacter))
        {
            return;
        }


        // =================================================
        // DİĞER KARAKTERİN STACK'İ
        // =================================================

        if (!collision.collider
            .TryGetComponent<CharacterStack>(
                out CharacterStack otherStack))
        {
            return;
        }


        if (myStack == null ||
            myCharacter == null)
        {
            return;
        }


        // =================================================
        // HARD AI ÖZEL DURUMU
        //
        // Normal çarpışmalarda minimum hız şartımız
        // devam ediyor.
        //
        // Fakat Hard AI bilinçli olarak Player'a
        // saldırıyorsa dibine geldiğinde hız düşse bile
        // çarpışmanın çalışmasına izin veriyoruz.
        //
        // Bu sayede AI Player'a yapışıp itmez.
        // =================================================

        bool hardAIAttack =
            IsHardAIPlayerCollision(
                otherCharacter
            );


        if (!hardAIAttack &&
            collision.relativeVelocity.magnitude <
            minimumImpactSpeed)
        {
            return;
        }


        // =================================================
        // BRICK COUNTS
        // =================================================

        int myBrickCount =
            myStack.BrickCount;


        int otherBrickCount =
            otherStack.BrickCount;


        // Brick sayıları eşitse
        // kimse Knockback yemez.
        if (myBrickCount ==
            otherBrickCount)
        {
            return;
        }


        // =================================================
        // BENİM BRICK SAYIM DAHA AZ
        //
        // Bu CollisionDetector Knockback uygulamaz.
        //
        // Diğer karakterin detector'ı bu işlemi yapar.
        // =================================================

        if (myBrickCount <
            otherBrickCount)
        {
            return;
        }


        // =================================================
        // BENİM BRICK SAYIM DAHA FAZLA
        // =================================================

        if (!collision.collider
            .TryGetComponent<IKnockbackable>(
                out IKnockbackable knockbackable))
        {
            return;
        }


        // =================================================
        // KNOCKBACK DIRECTION
        // =================================================

        Vector3 direction =
            otherCharacter.transform.position -
            transform.position;


        direction.y =
            0f;


        if (direction.sqrMagnitude <
            0.001f)
        {
            direction =
                transform.forward;
        }


        direction.Normalize();


        // =================================================
        // KNOCKBACK FORCE
        // =================================================

        Vector3 knockbackForce =
            direction *
            horizontalKnockbackForce;


        knockbackForce +=
            Vector3.up *
            upwardKnockbackForce;


        // =================================================
        // KNOCKBACK
        // =================================================

        knockbackable.TakeKnockback(
            knockbackForce
        );


        Debug.Log(
            gameObject.name +
            " (" +
            myBrickCount +
            " Brick) > " +
            otherCharacter.gameObject.name +
            " (" +
            otherBrickCount +
            " Brick) → " +
            otherCharacter.gameObject.name +
            " Knockback!"
        );
    }


    // =====================================================
    // HARD AI → PLAYER COLLISION
    // =====================================================

    private bool IsHardAIPlayerCollision(
        CharacterBase otherCharacter)
    {
        // Sadece Hard difficulty.
        if (GameSettings.SelectedDifficulty !=
            AIDifficulty.Hard)
        {
            return false;
        }


        // Çarpan karakter AI olmalı.
        if (!(myCharacter is AIController))
        {
            return false;
        }


        // Çarpılan karakter Player olmalı.
        if (!(otherCharacter is PlayerController))
        {
            return false;
        }


        return true;
    }
}