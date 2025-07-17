using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial
{

    public static void SetMaterial(int ID, GameObject selectedObject, ObjectsDatabase_SO objectsDatabase, int renderID)
    {
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = objectsDatabase.objectsData[ID].ObjectMaterials[renderID].Material;
            }
            renderer.materials = materials;
        }
    }

}
