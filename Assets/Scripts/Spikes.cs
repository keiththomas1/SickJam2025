using UnityEngine;

public class Spikes : MonoBehaviour
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
            float localForceMultiplier = 0.65f;
            if (collider.gameObject.name == "Player")
            {
                if (AudioController.Instance != null)
                {
                    AudioController.Instance.LoadNewSFXAndPlay("Hit_loud", null, 0.8f);
                }

                localForceMultiplier = 1f;
            }

            Vector3 finalVelocity = new Vector3(
                -7.5f * this.ForceMultiplier * localForceMultiplier,
                10f * this.ForceMultiplier * localForceMultiplier,
                0f);

            collider.gameObject.GetComponent<SickCharacterController>().AddVelocity(finalVelocity);

        }
    }
}
