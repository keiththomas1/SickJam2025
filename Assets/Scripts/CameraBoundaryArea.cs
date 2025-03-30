using UnityEngine;
using UnityEngine.Events;

public class CameraBoundaryArea : MonoBehaviour
{
    public Transform CameraTransform;

    public class TriggerEvent : UnityEvent<CameraBoundaryArea> { }
    public TriggerEvent OnTriggerEntered = new TriggerEvent();

    private void Awake()
    {
        this.CameraTransform.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.OnTriggerEntered.Invoke(this);
        }
    }
}
