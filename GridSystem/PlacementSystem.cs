using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementSystem : Singleton<PlacementSystem>
{
    [SerializeField] public InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private Grid gridForSmall;
    [SerializeField] private Grid selectedGrid;


    [SerializeField] 
    private ObjectsDatabase_SO objectsDatabase;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData, furnitureData, decorationData;
    public GridData selectedData;

    //private Renderer previewRenderer;

    [SerializeField]
    private PreviewSystem previewSystem;
    public Vector3 PreviewPos=>previewSystem.previewPos;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    private GameObject buttons;

    ObjectMono selectedObject;

    IBuidlingState buildingState;

    public int rotation;

    public float placeOffset = 0f;

    bool posValidity;

    private List<ObjectMono> objs=new List<ObjectMono>();

    [HideInInspector] public UnityEvent<ObjectMono> onObjPlaced = new UnityEvent<ObjectMono>();
    [HideInInspector] public UnityEvent<ObjectMono> onMaterialOff = new UnityEvent<ObjectMono>();
    [HideInInspector] public UnityEvent<ObjectMono> onMaterialOn = new UnityEvent<ObjectMono>();
    [HideInInspector] public UnityEvent<ObjectMono> onObjRemoved = new UnityEvent<ObjectMono>();
    [HideInInspector] public UnityEvent<ObjectMono> onObjRotated = new UnityEvent<ObjectMono>();

    public int floorIndex;
    public float floorHeight;

    public static bool previewing=false;
    private void Start()
    {
        StopPlacement();
        floorData = new GridData();
        furnitureData = new GridData();
        decorationData = new GridData();
        //previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        //buttons.SetActive(false);
        rotation = 0;
        floorIndex = 0;
        floorHeight = 0;
        //inputManager.OnHold += StartMoving;
    }

#if !UNITY_IOS
    /*private void Update()
    {


        if (buildingState == null) return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition); //Change mousePos into the latest cellPos

        if (lastDetectedPosition != gridPosition)
        {

            lastDetectedPosition = gridPosition;
            buildingState.UpdateState(lastDetectedPosition);
        }

        if (Input.GetKeyDown(KeyCode.R)) // TODO: make this into a function
        {
            rotation = (rotation + 1) % 4;
            buildingState.UpdateState(lastDetectedPosition);

            //lastDetectedPosition = gridPosition;
        }

    }*/
#endif

    public void RotateObject()
    {
        //Vector3 mousePosition = inputManager.GetSelectedMapPosition(mousepos);
        //Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        rotation = (rotation + 1) % 4;
        buildingState.UpdateState(lastDetectedPosition);
        onObjRotated?.Invoke(selectedObject);
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        rotation = 0;
        gridVisualization.SetActive(true);

        selectedData = GetGridData(objectsDatabase.objectsData[ID].GridLayer);

        buildingState = new PlacementState(ID, selectedGrid , previewSystem, objectsDatabase, selectedData, objectPlacer, 0);

        Debug.Log("Place " + lastDetectedPosition);
        buildingState.UpdateState(lastDetectedPosition);

    }

    public void StartRemoving()
    {
        RemoveObject(selectedObject);
        
        gridVisualization.SetActive(true);
        rotation = 0;
        buildingState = new RemovingState(selectedGrid, selectedObject, objectPlacer);

        //inputManager.OnClicked += PlaceStructure;
        buildingState.OnAction(lastDetectedPosition);
        //inputManager.OnExit += StopPlacement;
        StopPlacement();
    }

    public void StartMoving(ObjectMono selectedObject)
    {

        StopPlacement();
        gridVisualization.SetActive(true);

        //Debug.Log(selectedObject);
        this.selectedObject = selectedObject;
        rotation = selectedObject.placementData.rotation;
        //Debug.Log(rotation);

        //Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        //Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        //lastDetectedPosition = placementData.occupiedPositions[0];

        buildingState = new MovingState(selectedGrid, previewSystem, objectsDatabase, selectedData, objectPlacer, selectedObject);

        Debug.Log(lastDetectedPosition);

        buildingState.UpdateState(lastDetectedPosition);
        

    }

    public void StartChangeMaterial(int renderID)
    {
        onMaterialOff?.Invoke(selectedObject);
        ChangeMaterial.SetMaterial(selectedObject.placementData.ID, selectedObject.gameObject, objectsDatabase, renderID);
        selectedObject.renderID = renderID;
        onMaterialOn?.Invoke(selectedObject);
    }

    public void StartSelect()
    {
        //buttons.SetActive(true);
        
        StopPlacement();

        selectedObject = SelectObject();
        //Debug.Log(selectedObject);

        if (selectedObject == null) return ;


        gridVisualization.SetActive(true);

        //Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        //Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        lastDetectedPosition = selectedObject.placementData.occupiedPositions[0];

        //Debug.Log(lastDetectedPosition);
        //Debug.Log(mousePosition);

        selectedData = GetGridData(selectedObject.gridLayer);

        buildingState = new SelectState(selectedGrid, selectedObject,selectedData);

        buildingState.OnAction(lastDetectedPosition);

        //PlacementData data = buidlingState.OnSelectAction(gridPosition);
        //if (data != null)
        //Debug.Log(objectsDatabase.objectsData[data.ID].Name+" "+data.PlacedObjectIndex);

        //buidlingState = null;


    }

    public void PlaceStructure()
    {
        //if (inputManager.IsPointerOverUI())
        //{
        //    return;
        //}
        //Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        //Vector3Int gridPosition = grid.WorldToCell(mousePosition); //Change mousePos into the latest cellPos
        

        buildingState.OnAction(lastDetectedPosition);

        
    }


    /*private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = objectsDatabase.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, objectsDatabase.objectsData[selectedObjectIndex].Size);
    }*/

    public void StopPlacement()
    {

        gridVisualization.SetActive(false);
        //cellIndicator.SetActive(false);

        if (buildingState == null)
        {
            Debug.Log("buildingState is null");
            return;
        }
        buildingState.EndState();

        //placeOffset = 0;
 

        selectedObject = null;

        //inputManager.OnHold -= PlaceStructure;
        //inputManager.OnUp -= StopPlacement;

        //lastDetectedPosition = Vector3Int.zero;

        buildingState = null;

    }


    public void PosInput(Vector2 mousepos)
    {
        

        Vector3 mousePosition = inputManager.GetSelectedMapPosition(mousepos);

        try {
            Vector3Int gridPosition = selectedGrid.WorldToCell(mousePosition); //Change mousePos into the latest cellPos

            //Debug.Log(gridPosition);

            if (buildingState != null )
            {

                buildingState.UpdateState(gridPosition);
                //Debug.Log(mousepos);
            }

            lastDetectedPosition = gridPosition;

            //Debug.Log(placeOffset);

            /*if (lastDetectedPosition != gridPosition)
            {
                
            }*/
        } catch { }
        
    }

    public bool IfBuildingStateIsNull()
    {
        return buildingState == null;
    }

    public bool GetPosValidity()
    {
        return posValidity;
    }

    public void SetPosValidity(bool b)
    {
        posValidity = b;
    }

    public ObjectMono SelectObject()
    {

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, 1 << (12 + floorIndex))) // only can select the object on the floor player's on 
        {
            
            return hit.transform.GetComponentInParent<ObjectMono>();
        }


        return null;
    }

    public GridData GetGridData(GridLayer gridLayer)
    {
        switch (gridLayer)
        {
            case GridLayer.floor: selectedGrid = grid; return floorData;
            case GridLayer.furniture: selectedGrid = grid; return furnitureData;
            case GridLayer.decoration: selectedGrid = gridForSmall; return decorationData;
            case GridLayer.wall: return null;
            default: return null;
        }
    }


    public Vector3 GetSelectedObjectPos(out bool flag)
    {
        if (selectedObject != null)
        {
            flag = true;
            return selectedObject.transform.GetChild(0).position;
        }
        else {
            flag = false;
            return Vector3.zero;
        }
            
    }

    public void SetObjectDatabse(ObjectsDatabase_SO target)
    {
        objectsDatabase = target;
    }

    public ObjectsDatabase_SO getObjectDatabase()
    {
        //Debug.Log(objectsDatabase);
        return objectsDatabase;
    }

    public ObjectMono GetSelectedObj()
    {
        return selectedObject;
    }

    public void AddObject(ObjectMono obj)
    {
        if (!previewing) {
            onObjPlaced?.Invoke(obj);
        }
        
        objs.Add(obj);
    }

    private void RemoveObject(ObjectMono obj)
    {
        onObjRemoved?.Invoke(obj);
        objs.Remove(obj);
    }

    public Grid GetSelectedGrid()
    {
        return selectedGrid;
    }

    public void SetObjectDeleted()
    {
        if(selectedObject != null)
        selectedObject.isDeleted = true;
    }
}
