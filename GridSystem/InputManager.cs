using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    [Inject]
    private CameraController cameraController;


    private void Awake()
    {
        cameraController.onFloorChanged.AddListener(changeFloor);
    }


    public Vector3 GetSelectedMapPosition(Vector3 mousePos)
    {
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, 1<<(7+ PlacementSystem.Instance.floorIndex) | 1 << 3))
        {
            lastPosition = hit.point;
            //Debug.Log($"Hit at:{hit.point}");
            PlacementSystem.Instance.floorHeight = hit.point.y;
        }

        return lastPosition;
    }

    private void changeFloor(int index)
    {
        PlacementSystem.Instance.floorIndex = index;
    }
}
