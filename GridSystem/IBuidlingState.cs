using UnityEngine;

public interface IBuidlingState
{
    void EndState();
    void OnAction(Vector3Int gridPosition);

    //void OnAction(ObjectMono objectMono);

    //PlacementData OnSelectAction(Vector3Int gridPosition);

    void UpdateState(Vector3Int gridPosition);
}
