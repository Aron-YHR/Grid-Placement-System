using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "ObjectsDatabase_SO", menuName = "Grid Building System/ObjectsDatabase_SO") ]
public class ObjectsDatabase_SO : ScriptableObject
{
    public List<ObjectData> objectsData;

}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public Category Category { get; private set; }

    [field: SerializeField]
    public GridLayer GridLayer { get; private set; }

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    [field: SerializeField]
    public Sprite Icon{ get; private set; }

    [field: SerializeField]
    public List<ObjMat> ObjectMaterials { get; private set; }

    [field:SerializeField]
    public List<string> Tags { get; private set; }

    [Header("Information")]
    public bool HasInformation=>!string.IsNullOrEmpty(InformationKey);
    [field: SerializeField]
    public string InformationKey { get; private set; }

    [Header("Audio")]
    [field: SerializeField]
    public int SoundId { get; private set; }

    [Serializable]
    public class ObjMat {
        [field: SerializeField] 
        public string Name { get; private set; }
        [field: SerializeField]
        public Material Material { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public List<string> Tags{ get; private set; }
    }

}

public enum Category {
    none=-1,
    chair=0,
    table=1,
    cabinets=2,
    decor=3
}
public enum GridLayer
{
    floor = 0,
    furniture = 1,
    decoration = 2,
    wall = 3
}