using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuidlingState
{
    private int gameObjectIndex = -1;

    Grid grid;
    //GridData selectedData;
    ObjectMono selectedObject;
    ObjectPlacer objectPlacer;
    int rotation;
    public RemovingState(Grid grid,

                         //GridData selectedData,
                         ObjectMono selectedObject,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        //this.selectedData = selectedData;
        this.selectedObject = selectedObject;
        this.objectPlacer = objectPlacer;


    }

    public void EndState()
    {
        //previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        /*GridData selectedData = null;
        if (furnitureData.CanPlaceObjectAt(grid, gridPosition, Vector2Int.one, rotation) == false)
        {
            selectedData = furnitureData;
        }
        else if (floorData.CanPlaceObjectAt(grid, gridPosition, Vector2Int.one, rotation) == false)
        {
            selectedData = floorData;
        }*/

        /*if (selectedData == null)
        {
            //audio
            Debug.Log("No Found selectedData");
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
            {
                Debug.Log("No Found");
                return;
            }

            //Debug.Log($"gridPosition:{gridPosition}\nIndex:{gameObjectIndex}");

            Ray ray = new Ray(grid.CellToWorld( gridPosition) + new Vector3(0, 0.1f, 0), Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 0.35f, 1 << (7 + PlacementSystem.Instance.floorIndex) | 1 << 3))
            {
                hit.collider.GetComponentInParent<ObjectMono>()?.SubstractCount();
            }

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }*/

        if(selectedObject == null)
        {
            Debug.Log("No Found SelectedObject");
        }
        else
        {
            Debug.Log("ray cast");
            Ray ray = new Ray(grid.CellToWorld(gridPosition) + new Vector3(0.05f, 0.1f, 0.05f), Vector3.down);
            Debug.Log(ray.origin);

            Debug.Log("hitcollider");
            if (Physics.Raycast(ray, out RaycastHit hit, 0.35f, 1 << (7 + PlacementSystem.Instance.floorIndex) | 1 << 3))
            {
                Debug.Log(hit.collider);
                hit.collider.GetComponentInParent<ObjectMono>()?.SubstractCount();
            }
            Debug.Log("after hitcollider");
            objectPlacer.RemoveObjectAt(selectedObject.placementData.PlacedObjectIndex);
        }

        //foreach(var data in selectedData.placedObjects)
        //Debug.Log(data.Key+" "+data.Value.PlacedObjectIndex);

        //Vector3 cellPosition = grid.CellToWorld(gridPosition);
        //previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition),Vector2Int.zero, rotation);

        PlacementSystem.Instance.StopPlacement();
    }

    
    public void UpdateState(Vector3Int gridPosition)
    {
        //bool validity = CheckIfSelectionIsValid(gridPosition);
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity, Vector2Int.zero, rotation); //update the position state when the mouse is moving
    }

    
}
