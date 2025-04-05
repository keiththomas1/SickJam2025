using UnityEngine;

public class MeatballMachine : MonoBehaviour
{
    [SerializeField]
    private GameObject Meatball;
    [SerializeField]
    private Collider DropZone;
    [SerializeField]
    private Vector3 StartForce;
    [SerializeField]
    private float DropRateSeconds = 0.1f;

    private bool _gameStarted = true;
    private float _dropTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this._dropTimer = this.DropRateSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (this._gameStarted)
        {
            if (this._dropTimer > 0f)
            {
                this._dropTimer -= Time.deltaTime;

                if (this._dropTimer <= 0f)
                {
                    this.DropMeatball();
                    this._dropTimer = this.DropRateSeconds;
                }
            }
        }
    }

    private void DropMeatball()
    {
        var newMeatball = GameObject.Instantiate(this.Meatball);
        var randPosition = this.GetRandomPointWithinCollider(this.DropZone);
        newMeatball.transform.position = randPosition;
        newMeatball.SetActive(true);
        newMeatball.GetComponent<Rigidbody>().AddForce(this.StartForce, ForceMode.Impulse);
    }


    public Vector3 GetRandomPointWithinCollider(Collider collider)
    {
        if (collider is BoxCollider box)
        {
            Vector3 center = box.center;
            Vector3 size = box.size * 0.5f;

            float x = Random.Range(-size.x, size.x);
            float y = Random.Range(-size.y, size.y);
            float z = Random.Range(-size.z, size.z);

            return collider.transform.TransformPoint(center + new Vector3(x, y, z));
        }
        else if (collider is SphereCollider sphere)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            float randomRadius = Random.Range(0f, sphere.radius);
            return collider.transform.TransformPoint(sphere.center + randomDirection * randomRadius);
        }
        else if (collider is CapsuleCollider capsule)
        {
            Vector3 point;
            float height = Mathf.Max(0, capsule.height / 2f - capsule.radius);
            Vector3 up = collider.transform.up;

            if (Random.value > 0.5f)
            {
                point = capsule.center + up * height;
            }
            else
            {
                point = capsule.center - up * height;
            }

            Vector3 randomCircle = Random.insideUnitSphere * capsule.radius;
            randomCircle.y = 0;

            return collider.transform.TransformPoint(point + randomCircle);
        }
        else if (collider is MeshCollider meshCollider && meshCollider.convex)
        {
            Bounds bounds = meshCollider.bounds;
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            return randomPoint;
        }

        Debug.LogWarning("Collider type not supported or is non-convex.");
        return collider.transform.position;
    }
}
