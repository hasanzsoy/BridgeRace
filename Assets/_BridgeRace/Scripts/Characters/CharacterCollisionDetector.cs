using UnityEngine;

public class CharacterCollisionDetector : MonoBehaviour
{
    [Header("Impact Settings")]
    [SerializeField] private float minimumImpactSpeed = 2f;

    [SerializeField] private float horizontalKnockbackForce = 7.5f;

    [SerializeField] private float upwardKnockbackForce = 2.2f;


    private void OnCollisionEnter(
        Collision collision)
    {
        if (collision.relativeVelocity.magnitude <
            minimumImpactSpeed)
        {
            return;
        }


        if (!collision.collider.TryGetComponent<IKnockbackable>(
                out IKnockbackable knockbackable))
        {
            return;
        }


        Vector3 direction =
            collision.transform.position -
            transform.position;


        direction.y = 0f;


        if (direction.sqrMagnitude < 0.001f)
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
    }
}