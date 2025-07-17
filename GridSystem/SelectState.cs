using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectState : IBuidlingState
{


    Grid grid;

    ObjectMono selectedObject;
    GridData selectedData;

    int rotation;
    public SelectState(Grid grid,
                       ObjectMono selectedObject,
                       GridData selectedData)
    {
        this.grid = grid;
        this.selectedObject = selectedObject;
        rotation = 0;
        this.selectedData = selectedData;
    }

    public void EndState()
    {
        
    }

    public void OnAction(Vector3Int gridPosition)
    {
        
        Debug.Log(gridPosition);
        

        if (selectedObject == null)
        {
            //audio
            PlacementSystem.Instance.StopPlacement();
            return;
        }
        else
        {
            //PlacementData previousData = selectedObject.placementData;
            //if (previousData == null) Debug.Log(-1);

           // PlacementData placementData = new PlacementData(previousData.occupiedPositions, previousData.ID, previousData.PlacedObjectIndex, previousData.rotation);
            
            selectedData.RemoveObjectAt(gridPosition);

            selectedObject.gameObject.GetComponent<VFXController>()?.DisactivateVFXObject();

            PlacementSystem.Instance.StartMoving( selectedObject );
        }


    }

    


    public void UpdateState(Vector3Int gridPosition)
    {
        
    }

}
