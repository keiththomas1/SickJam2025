using UnityEngine;
using UnityEngine.Events;

public class Triggerable : MonoBehaviour
{
    public class TriggerEvent : UnityEvent<Collider> { }
    public TriggerEvent OnTriggered = new TriggerEvent();

    public void OnTriggerEnter(Collider other)
    {
        this.OnTriggered.Invoke(other);
    }
}
