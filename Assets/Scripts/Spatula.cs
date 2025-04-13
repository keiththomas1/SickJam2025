using KinematicCharacterController.Examples;
using System.Drawing;
using UnityEngine;

public class Spatula : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigidbody;
    [SerializeField]
    private float ForceMultiplier = 50f;

    //public void OnCollisionEnter(Collision collision)
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Vector3 linearVelocity = this.Rigidbody.GetPointVelocity(collider.transform.position);
            Vector3 finalVelocity = new Vector3(linearVelocity.x * this.ForceMultiplier, 22f, linearVelocity.z * this.ForceMultiplier);

            collider.gameObject.GetComponent<SickCharacterController>().AddVelocity(finalVelocity); // * velocityTowardsCollision );

            if (collider.gameObject.name == "Player")
            {
                if (AudioController.Instance != null)
                {
                    AudioController.Instance.LoadNewSFXAndPlay("Hit_loud", null, 0.8f);
                }
            }
        }
    }
}
