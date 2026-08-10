using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterKnockback : MonoBehaviour, IKnockbackable
{
    [Header("Knockback Settings")]
    [SerializeField] private float controlLockDuration = 0.35f;

    [SerializeField] private float knockbackProtectionDuration = 1.1f;


    private Rigidbody rb;

    private CharacterBase character;

    private CharacterStack characterStack;

    private bool isKnockedBack;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        character = GetComponent<CharacterBase>();

        characterStack = GetComponent<CharacterStack>();


        if (character == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterBase bulunamadı!"
            );
        }


        if (characterStack == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterStack bulunamadı!"
            );
        }
    }


    public void TakeKnockback(Vector3 force)
    {
        if (isKnockedBack)
        {
            return;
        }


        StartCoroutine(
            KnockbackRoutine(force)
        );
    }


    private IEnumerator KnockbackRoutine(Vector3 force)
    {
        isKnockedBack = true;


        if (character != null)
        {
            character.SetMovementEnabled(false);
        }


        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;


        rb.AddForce(
            force,
            ForceMode.Impulse
        );


        // Taşınan bütün Brick'leri düşür.
        if (characterStack != null)
        {
            characterStack.DropBricks(
                characterStack.BrickCount
            );
        }


        if (character != null)
        {
            EventManager.CharacterKnockback(
                character
            );
        }


        // Kısa süre kontrol kapalı.
        yield return new WaitForSeconds(
            controlLockDuration
        );


        if (character != null)
        {
            character.SetMovementEnabled(true);
        }


        // Kontrol geri geldi ama karakter
        // hemen tekrar knockback yiyemez.
        float remainingProtection =
            knockbackProtectionDuration -
            controlLockDuration;


        if (remainingProtection > 0f)
        {
            yield return new WaitForSeconds(
                remainingProtection
            );
        }


        isKnockedBack = false;
    }
}