using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundaryController : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;
    [SerializeField]
    private List<CameraBoundaryArea> Areas = new List<CameraBoundaryArea>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (CameraBoundaryArea area in this.Areas)
        {
            area.OnTriggerEntered.AddListener(this.AreaTriggered);
        }
    }

    private void AreaTriggered(CameraBoundaryArea area)
    {
        this.Camera.transform.position = area.CameraTransform.position;
        this.Camera.transform.rotation = area.CameraTransform.rotation;
    }
}
