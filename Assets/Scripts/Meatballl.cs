using KinematicCharacterController.Examples;
using UnityEngine;

public class Meatball : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigidbody;
    [SerializeField]
    private float ForceMultiplier = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ContactPoint contact = collision.contacts[0];

            // The normal points *away* from the other collider
            Vector3 collisionNormal = contact.normal.normalized;
            collisionNormal *= -1;

            // Project velocity onto the collision normal
            // float velocityTowardsCollision = Vector3.Dot(this.Rigidbody.linearVelocity.normalized, collisionNormal);

            // Optional: Clamp to positive only (i.e., only if it's actually moving toward)
            // Debug.LogWarning(velocityTowardsCollision);
            // velocityTowardsCollision = Mathf.Max(1f, velocityTowardsCollision);

            collision.gameObject.GetComponent<SickCharacterController>().AddVelocity(collisionNormal * this.ForceMultiplier); // * velocityTowardsCollision );

            if (collision.gameObject.name == "Player")
            {
                if (AudioController.Instance != null)
                {
                    AudioController.Instance.LoadNewSFXAndPlay("Hit_loud", null, 0.8f);
                }
            }
        }
    }
}
