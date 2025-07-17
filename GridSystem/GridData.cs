using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData
{
    public Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObejctAt(Vector3Int gridPosition, Vector2Int objectSize, int ID,int SoundId, int placedObjectIndex, int rotation)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotation);
        PlacementData data = new PlacementData(positionToOccupy,ID, SoundId, placedObjectIndex, rotation);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize, int rotation)
    {
        List<Vector3Int> returnVal = new();

        if (objectSize.x == objectSize.y) // saqure object
        { 
            for (int x = 0; x < objectSize.x; x++) // the case with rotation
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
                }
            }
        }
        else
        {
            for (int x = 0; x < objectSize.x; x++) // the case with rotation
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    if (rotation%2 == 0)
                        returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
                    else if (rotation%2 == 1)
                    {
                        returnVal.Add(gridPosition + new Vector3Int(y, 0, x));
                    }
                    /*else if (rotation == 2)
                    {
                        returnVal.Add(gridPosition + new Vector3Int(-x, 0, -y));
                    }
                    else if (rotation == 3)
                    {
                        returnVal.Add(gridPosition + new Vector3Int(-y, 0, x));
                    }*/
                }
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Grid grid, Vector3Int gridPosition, Vector2Int objectSize, int rotation)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotation);
        foreach(var pos in positionToOccupy)
        {

            Vector3 pos1 = grid.CellToWorld( pos ) + new Vector3(0.5f,1.5f,0.5f) * grid.cellSize.x;
            Ray ray = new Ray(pos1, Vector3.down);

            if (!Physics.Raycast(ray, out RaycastHit hit, 0.15f , 1<< (7 + PlacementSystem.Instance.floorIndex) | 1 << 3 ))
            {
                Debug.Log("No grid");

                return false;
            }
            if (placedObjects.ContainsKey(pos)) { 

                return false;
            } 
        }
        return true;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if(placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }

        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    public int GetObjectID(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }

        return placedObjects[gridPosition].ID;
    }

    public PlacementData GetObjectPlacementData(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return null;
        }
        return placedObjects[gridPosition];
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        //Debug.Log($"RemoveObjectAt:{gridPosition}");
        foreach(var pos in placedObjects[gridPosition].occupiedPositions)
        {
            //Debug.Log("remove: "+pos);
            placedObjects.Remove(pos); //remove the data
        }
    }

    public void MoveObjectTo(Vector3Int gridPosition, Vector2Int objectSize, PlacementData placementData, int rotation)
    {
        
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotation);

        /*foreach(var pos in positionToOccupy)
        {
            Debug.Log(pos);
        }*/

        placementData.rotation = rotation;
        placementData.occupiedPositions = positionToOccupy;

        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = placementData;
        }
    }
}

[Serializable]
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public int SoundID { get; private set; }

    public HashSet<string> tags;

    public int rotation;

    public PlacementData(List<Vector3Int> occupiedPositions, int iD,int soundId, int placedObjectIndex, int rotation)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        SoundID = soundId;            ;
        PlacedObjectIndex = placedObjectIndex;
        this.rotation = rotation;
    }
}
