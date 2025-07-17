using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMono : MonoBehaviour
{

    [SerializeField] public PlacementData placementData;

    [SerializeField] public GameObject prefab;

    [SerializeField] public int renderID;

    public bool isDeleted = false;

    public GridLayer gridLayer;

    public int count;

    public int ObjId => placementData.ID;
    public int MatId => renderID;

    public int SoundId=>placementData.SoundID;

    public void SetObjectMono(PlacementData placementData, GameObject prefab, int renderID, GridLayer gridLayer, int count)
    {
        this.placementData = placementData;
        this.prefab = prefab;
        this.renderID = renderID;
        this.gridLayer = gridLayer;
        this.count = count;


    }

    public void AddCount()
    {
        count++;
        if(count > 0 && transform.GetChild(0).TryGetComponent<BoxCollider>(out BoxCollider collider))
        {
            collider.enabled = false;
        }
    }

    public void SubstractCount()
    {
        if(count > 0)
            count--;
        if(count == 0 && transform.GetChild(0).TryGetComponent<BoxCollider>(out BoxCollider collider))
        {
            collider.enabled = true;
        }
    }

}
