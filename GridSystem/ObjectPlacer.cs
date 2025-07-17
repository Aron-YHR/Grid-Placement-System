using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour // spawn the object on the grid cell
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new(); // the objects are placed on the grid

    [SerializeField]
    private float duration = 0.3f, durationScale = 0.3f;
    [SerializeField]
    private Vector3 strength = new Vector3(0, 0.3f, 0);
    [SerializeField]
    private Ease ease = Ease.OutBounce, easeScale = Ease.OutBounce;


    public int PlaceObject(GameObject prefab, Vector3 position, int rotation)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.GetChild(0).gameObject.layer = 12 + PlacementSystem.Instance.floorIndex;
        newObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.layer = 12 + PlacementSystem.Instance.floorIndex;

        newObject.transform.position = position;

        Vector3 childPosition = newObject.transform.Find("GameObject").localPosition;

        if (rotation == 0)
            newObject.transform.Find("GameObject").localPosition = new Vector3(childPosition.x, 0, childPosition.z);
        else if (rotation == 1)
            newObject.transform.Find("GameObject").localPosition = new Vector3(childPosition.z, 0, childPosition.x);

        newObject.transform.Find("GameObject").localRotation = Quaternion.Euler(new Vector3(0, rotation * 90, 0));

        

        Transform surface = newObject.transform.GetChild(0).Find("Surface");
        if(surface !=null)
            surface.GetComponent<BoxCollider>().enabled = true;

        float objectSize = newObject.GetComponentInChildren<Renderer>().bounds.size.magnitude * 0.5f;
        //Debug.Log(objectSize);

        newObject.transform.DOShakePosition(duration, strength * PlacementSystem.Instance.GetSelectedGrid().cellSize.x).SetEase(ease).OnStart(() =>
        {
            DustSpawner.Instance.SpawnDustEffect(newObject.transform.GetChild(0).GetChild(0).position, objectSize);
        });

        

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    public void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            Debug.Log("No Found");
            return;
        }


        placedGameObjects[gameObjectIndex].transform.GetChild(0).GetChild(0).DOScale( new Vector3(0, 0, 0), durationScale)
                                                    .SetEase(easeScale)
                                                    .OnComplete(() => { Destroy(placedGameObjects[gameObjectIndex]);placedGameObjects[gameObjectIndex] = null; });


    }

    public void MoveObject(GameObject prefab, Vector3 position)
    {
        prefab.transform.position = position;
        //Debug.Log($"{position}");

        Transform surface = prefab.transform.GetChild(0).Find("Surface");
        if (surface != null)
            surface.GetComponent<BoxCollider>().enabled = true;

        float objectSize = prefab.GetComponentInChildren<Renderer>().bounds.size.magnitude * 0.5f;
        //Debug.Log(objectSize);

        prefab.transform.DOShakePosition(duration, strength * PlacementSystem.Instance.GetSelectedGrid().cellSize.x).SetEase(ease).OnStart(() =>
        {
            if(!prefab.GetComponent<ObjectMono>().isDeleted)
            DustSpawner.Instance.SpawnDustEffect(prefab.transform.GetChild(0).GetChild(0).position, objectSize);
        });

        prefab.GetComponent<VFXController>()?.ActivateVFXObject();

        //if (tweener==null || tweener.IsComplete()) {
        //    tweener=;
        //}

    }

    public GameObject GetPlacedGameObject(int gameObjectIndex)
    {
        return placedGameObjects[gameObjectIndex];
    }
}
