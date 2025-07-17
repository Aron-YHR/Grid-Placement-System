using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuidlingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase_SO objectsDatabase;
    GridData selectedData;
    ObjectPlacer objectPlacer;
    int rotation;
    float placeOffset = 0;

    public static ObjectMono NewestObj;



    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabase_SO objectsDatabase,
                          GridData selectedData,
                          ObjectPlacer objectPlacer,
                          int rotation
                          )
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.objectsDatabase = objectsDatabase;
        this.selectedData = selectedData;
        this.objectPlacer = objectPlacer;
        this.rotation = rotation;




        selectedObjectIndex = objectsDatabase.objectsData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
            objectsDatabase.objectsData[selectedObjectIndex].Prefab,
            objectsDatabase.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }

        
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition) // check if the grid cell can be placed, if yes, go to spawn the object, and add the data
    {
        Ray ray = new Ray(previewSystem.previewPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.15f , 1 << (7 + PlacementSystem.Instance.floorIndex) | 1 << 3))
        {

            placeOffset = hit.point.y;
            Debug.Log(placeOffset);

            hit.collider.GetComponentInParent<ObjectMono>()?.AddCount();

            
        }

        Vector3 objectPosition = grid.CellToWorld(gridPosition);
        objectPosition.y = placeOffset;

        Debug.Log("Place at: " + objectPosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex, rotation);
        if (placementValidity == false)
        {
            return;
        }

        int index = objectPlacer.PlaceObject(objectsDatabase.objectsData[selectedObjectIndex].Prefab, objectPosition, rotation);

        //mouseIndicator.transform.position = mousePosition;


        selectedData.AddObejctAt(gridPosition,
            objectsDatabase.objectsData[selectedObjectIndex].Size,
            objectsDatabase.objectsData[selectedObjectIndex].ID,
            objectsDatabase.objectsData[selectedObjectIndex].SoundId,
            index,rotation);

        GameObject newObject = objectPlacer.GetPlacedGameObject(index);

        NewestObj = newObject.AddComponent<ObjectMono>();
        NewestObj.SetObjectMono(selectedData.GetObjectPlacementData(gridPosition), newObject, 0, objectsDatabase.objectsData[selectedObjectIndex].GridLayer,0); // add ObjectMono script to the new placed object;
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false, objectsDatabase.objectsData[selectedObjectIndex].Size,rotation, grid.cellSize.x);
        ChangeMaterial.SetMaterial(NewestObj.placementData.ID, NewestObj.gameObject, objectsDatabase, 0);

        newObject.GetComponent<VFXController>()?.ActivateVFXObject();

        PlacementSystem.Instance.AddObject(NewestObj);

        PlacementSystem.Instance.rotation = 0;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex, int rotation)
    {

        return selectedData.CanPlaceObjectAt(grid,gridPosition, objectsDatabase.objectsData[selectedObjectIndex].Size,rotation);
    }

    public void UpdateState(Vector3Int gridPosition) //keep checking the validity of the grid cell
    {
        placeOffset = PlacementSystem.Instance.placeOffset;
        rotation = PlacementSystem.Instance.rotation;
        //Debug.Log(rotation);

        


        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex,rotation);
        PlacementSystem.Instance.SetPosValidity(placementValidity);
       // Debug.Log(grid.CellToWorld(gridPosition));

        Vector3 objectPosition = grid.CellToWorld(gridPosition);
        objectPosition.y = PlacementSystem.Instance.floorHeight;

        previewSystem.UpdatePosition(objectPosition, placementValidity, objectsDatabase.objectsData[selectedObjectIndex].Size,rotation, grid.cellSize.x);
    }

}
