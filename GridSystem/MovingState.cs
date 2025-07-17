using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : IBuidlingState
{
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase_SO objectsDatabase;

    ObjectPlacer objectPlacer;
    GridData selectedData;
    ObjectMono selectedObject;
    int rotation;
    float placeOffset = 0;

    Collider lastDetectedCollider;

    public MovingState(Grid grid,
                         PreviewSystem previewSystem,
                         ObjectsDatabase_SO objectsDatabase,
                         GridData selectedData,
                         ObjectPlacer objectPlacer,
                         ObjectMono selectedObject)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.objectsDatabase = objectsDatabase;
        this.selectedData = selectedData;
        this.objectPlacer = objectPlacer;
        this.selectedObject = selectedObject;

        

        rotation = selectedObject.placementData.rotation;

        


            //selectedData = placementData.ID == 0 ? floorData : furnitureData;

        previewSystem.StartShowingMovingPreview(objectPlacer.GetPlacedGameObject(selectedObject.placementData.PlacedObjectIndex),
        objectsDatabase.objectsData[selectedObject.placementData.ID].Size);

        Ray ray = new Ray(previewSystem.previewPos + new Vector3(0,0.1f,0), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.35f, 1 << (7 + PlacementSystem.Instance.floorIndex) | 1 << 3))
        {
            lastDetectedCollider = hit.collider;
            //Debug.DrawRay(ray.origin,ray.direction,Color.red,.95f);
            //Debug.LogError(lastDetectedCollider.name);
            PlacementSystem.Instance.floorHeight = hit.point.y;
        }
        else
        {
            lastDetectedCollider = null;
        }


    }

    public void EndState()
    {
        
        //previewSystem.StopShowingMovingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        //Debug.Log(gridPosition);

        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (placementValidity == false)
        {
            Debug.Log(-1);
            return;
        }
        
        Ray ray = new Ray(previewSystem.previewPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.15f , 1 << (7 + PlacementSystem.Instance.floorIndex) | 1 << 3))
        {
            placeOffset = hit.point.y;
        }

        Vector3 objectPosition = grid.CellToWorld(gridPosition);
        objectPosition.y = placeOffset;
        //mouseIndicator.transform.position = mousePosition;

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false, objectsDatabase.objectsData[selectedObject.placementData.ID].Size, rotation, grid.cellSize.x);
        //Debug.Log("Moving");

        selectedData.MoveObjectTo(gridPosition,
            objectsDatabase.objectsData[selectedObject.placementData.ID].Size, selectedObject.placementData, rotation);
        //Debug.Log("Moving1");

        

        objectPlacer.MoveObject(objectPlacer.GetPlacedGameObject(selectedObject.placementData.PlacedObjectIndex), objectPosition);
        //Debug.Log("Moving2");


        //PlacementSystem.Instance.StopPlacement();


    }
    
    public void UpdateState(Vector3Int gridPosition)
    {
        placeOffset = PlacementSystem.Instance.placeOffset;
        rotation = PlacementSystem.Instance.rotation;
        //Debug.Log(rotation);


        bool placementValidity = CheckPlacementValidity(gridPosition);
        PlacementSystem.Instance.SetPosValidity(placementValidity);

        Vector3 objectPosition = grid.CellToWorld(gridPosition);
        objectPosition.y = PlacementSystem.Instance.floorHeight; // Adjust hight issue

        previewSystem.UpdatePosition(objectPosition, placementValidity, objectsDatabase.objectsData[selectedObject.placementData.ID].Size, rotation, grid.cellSize.x);

        Ray ray = new Ray(previewSystem.previewPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.35f, 1 << (7 + PlacementSystem.Instance.floorIndex) | 1 << 3))
        {

            if (hit.collider != lastDetectedCollider)
            {

                //Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.35f);
                //Debug.LogError(lastDetectedCollider.name + " " + hit.collider.name);
                if (lastDetectedCollider != null)
                    lastDetectedCollider.GetComponentInParent<ObjectMono>()?.SubstractCount();
                lastDetectedCollider = hit.collider;
                lastDetectedCollider.GetComponentInParent<ObjectMono>()?.AddCount();
            }
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        //GridData selectedData = objectsDatabase.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(grid, gridPosition, objectsDatabase.objectsData[selectedObject.placementData.ID].Size, rotation);
    }

    
}
