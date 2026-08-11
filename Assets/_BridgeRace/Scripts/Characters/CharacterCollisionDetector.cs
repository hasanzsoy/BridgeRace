using UnityEngine;

public class CharacterCollisionDetector : MonoBehaviour
{
    [Header("Impact Settings")]
    [SerializeField] private float minimumImpactSpeed = 3.5f;

    [SerializeField] private float horizontalKnockbackForce = 6.5f;

    [SerializeField] private float upwardKnockbackForce = 1.8f;


    private CharacterStack myStack;


    private void Awake()
    {
        myStack =
            GetComponent<CharacterStack>();


        if (myStack == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde CharacterStack bulunamadı!"
            );
        }
    }


    private void OnCollisionEnter(
        Collision collision)
    {
        // Çok hafif temaslarda knockback olmasın.
        if (collision.relativeVelocity.magnitude <
            minimumImpactSpeed)
        {
            return;
        }


        // Çarptığımız nesne bir yarışçı mı?
        if (!collision.collider.TryGetComponent<CharacterBase>(
                out CharacterBase otherCharacter))
        {
            return;
        }


        // Diğer karakterin stack sistemini bul.
        if (!collision.collider.TryGetComponent<CharacterStack>(
                out CharacterStack otherStack))
        {
            return;
        }


        if (myStack == null)
        {
            return;
        }


        int myBrickCount =
            myStack.BrickCount;


        int otherBrickCount =
            otherStack.BrickCount;


        // Brick sayıları eşitse kimse kaybetmez.
        if (myBrickCount ==
            otherBrickCount)
        {
            return;
        }


        // Benim Brick sayım daha azsa
        // diğer karakterin CollisionDetector'ı
        // beni düşürecek.
        //
        // Bu sayede aynı çarpışmada
        // iki defa knockback çağrılmıyor.
        if (myBrickCount <
            otherBrickCount)
        {
            return;
        }


        // Buraya geldiysek:
        //
        // Benim Brick sayım daha fazla.
        // Diğer karakter kaybetti.


        if (!collision.collider.TryGetComponent<IKnockbackable>(
                out IKnockbackable knockbackable))
        {
            return;
        }


        Vector3 direction =
            otherCharacter.transform.position -
            transform.position;


        direction.y = 0f;


        if (direction.sqrMagnitude <
            0.001f)
        {
            direction =
                transform.forward;
        }


        direction.Normalize();


        Vector3 knockbackForce =
            direction *
            horizontalKnockbackForce;


        knockbackForce +=
            Vector3.up *
            upwardKnockbackForce;


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
}