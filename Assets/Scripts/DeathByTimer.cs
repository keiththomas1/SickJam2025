using UnityEngine;

public class DeathByTimer : MonoBehaviour
{
    public float deathTimeInSeconds = 30f;
    float currentTimerValue;

    // Use this for initialization
    void Start()
    {
        currentTimerValue = deathTimeInSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimerValue -= Time.deltaTime;
        if (currentTimerValue <= 0.0f)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    public void SetDeathTimeInSeconds(float time)
    {
        this.deathTimeInSeconds = time;
        this.currentTimerValue = time;
    }
}