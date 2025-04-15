using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ConveyorBeltDummy;
    [SerializeField]
    private Vector3 SpawnPositionLocal;
    [SerializeField]
    private float DistanceBeforeRespawn = 50f;
    [SerializeField]
    private float ConveyorLength = 10f;

    private Queue<GameObject> _conveyorBelts = new Queue<GameObject>();

    private void Start()
    {
        this.ConveyorBeltDummy.SetActive(false);

        int conveyorsToSpawn = (int)(this.DistanceBeforeRespawn / this.ConveyorLength);
        for (int i = 0; i < conveyorsToSpawn; i++)
        {
            this.SpawnNewConveyorBelt(this.DistanceBeforeRespawn - (this.ConveyorLength * i));
        }
    }

    private void Update()
    {
        if (this._conveyorBelts.Count > 0)
        {
            if (this._conveyorBelts.Peek().transform.localPosition.z > this.SpawnPositionLocal.z + this.DistanceBeforeRespawn)
            {
                this.SpawnNewConveyorBelt(0f);

                var belt = this._conveyorBelts.Dequeue();
                Destroy(belt.gameObject);
            }
        }
    }

    private void SpawnNewConveyorBelt(float zOffset)
    {
        var newBelt = GameObject.Instantiate(this.ConveyorBeltDummy);
        newBelt.transform.SetParent(this.transform);
        newBelt.transform.localPosition = new Vector3(this.SpawnPositionLocal.x, this.SpawnPositionLocal.y, this.SpawnPositionLocal.z + zOffset);
        newBelt.SetActive(true);

        this._conveyorBelts.Enqueue(newBelt);
    }
}
