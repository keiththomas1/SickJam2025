using KinematicCharacterController.Examples;
using UnityEngine;

public class Meatbal : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigidbody;
    [SerializeField]
    private float ForceMultiplier = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

            collision.gameObject.GetComponent<ExampleCharacterController>().AddVelocity(collisionNormal * this.ForceMultiplier); // * velocityTowardsCollision );
        }
    }
}
