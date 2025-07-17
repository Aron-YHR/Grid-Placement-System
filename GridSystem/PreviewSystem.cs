using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f, cellIndicatorOffset = 0.055f;


    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    [SerializeField] private MeshRenderer gridMeshRenderer;

    [SerializeField] private float outRadiusMin = 1.5f, outRadiusMax = 3.0f;

    int lastRotation;


    public Vector3 previewPos=>previewObject.transform.GetChild(0).GetChild(0).position;

    private void Start()
    {
        lastRotation = 0;
        previewMaterialInstance = new Material(previewMaterialsPrefab);

    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {     
        previewObject = Instantiate(prefab); //a prebiew object
        Transform surface = previewObject.transform.GetChild(0).Find("Surface");
        if (surface != null)
        {
            BoxCollider[] colliders = surface.GetComponents<BoxCollider>();
            foreach(BoxCollider boxCollider in colliders)
            {
                boxCollider.enabled = false;
            }
        }

        PreparePreview(previewObject); // change the materials into transparent
        
    }

    public void StartShowingMovingPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = prefab;
        Transform surface = previewObject.transform.GetChild(0).Find("Surface");
        if (surface != null)
        {
            BoxCollider[] colliders = surface.GetComponents<BoxCollider>();
            foreach (BoxCollider boxCollider in colliders)
            {
                boxCollider.enabled = false;
            }
        }
    }

    private void PreparePreview(GameObject previewObject) // change the material into transparent
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for(int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }



    public void StopShowingPreview()
    {
        
        if(previewObject != null) 
            Destroy(previewObject);
    }

   /* public void StopShowingMovingPreview()
    {
        
    }*/

    public void UpdatePosition(Vector3 position, bool validaity, Vector2Int size, int rotation, float scale)
    {
        if (previewObject != null)
        {
            MovePreview(position, rotation);
            ApplyFeedbackToPreview(validaity);


            cellIndicator.transform.position = new Vector3(0, position.y + cellIndicatorOffset, 0);

            gridMeshRenderer.material.SetVector("_Center", previewObject.transform.GetChild(0).GetChild(0).position);
            //gridMeshRenderer.material.SetVector("_Center", position + new Vector3(size.x / 2.0f * scale , 0, size.y / 2.0f * scale));
            gridMeshRenderer.material.SetFloat("_OuterRadius", Mathf.Clamp(size.x * scale, outRadiusMin, outRadiusMax));

            ApplyFeedbackToGrid(validaity);

        }

    }

    private void MovePreview(Vector3 position, int rotation)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
        Debug.Log(rotation * 90);

        Vector3 childPosition = previewObject.transform.GetChild(0).localPosition;
        lastRotation = (int)previewObject.transform.GetChild(0).localRotation.eulerAngles.y / 90;
        //Debug.Log(previewObject.transform.GetChild(0).localRotation.eulerAngles.y);

        if (rotation != lastRotation)
        {
            previewObject.transform.GetChild(0).localPosition = new Vector3(childPosition.z, 0, childPosition.x);

            previewObject.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0, rotation * 90, 0));
        }
    }


    
    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        
        c.a = 0.5f;
        
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToGrid(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        gridMeshRenderer.material.SetColor("_GridColor", c);

    }

    public void StartShowingRemovePreview()
    {
        
    }
}
