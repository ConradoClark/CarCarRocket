using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CarGrid : MonoBehaviour
{

    private List<CarGridObject> gridObjects;
    public int gridXSize;
    public int gridYSize;
    public int cellSize;

    void Awake()
    {
        gridObjects = new List<CarGridObject>();
    }    

    void Update()
    {
        for (int i = 0; i < gridObjects.Count; i++)
        {
            CarGridObject gridObject = gridObjects[i];
            if (gridObject == null || !gridObject.isActiveAndEnabled) continue;
            gridObject.xInGrid = (int)(Mathf.Round((gridObject.transform.position.x - this.transform.position.x + (cellSize * gridXSize - 2 * cellSize) / 2) / cellSize)) - 1;
            gridObject.yInGrid = (int)(Mathf.Round((-gridObject.transform.position.y - this.transform.position.y + (cellSize * gridYSize - 2 * cellSize) / 2) / cellSize)) + 1;
        }
    }

    public void AddObject(CarGridObject obj)
    {
        if (!gridObjects.Contains(obj))
        {
            this.gridObjects.Add(obj);
        }
    }

    public IEnumerable<CarGridObject> GetObjectsInCell(int x, int y)
    {
        for (int i = 0; i < gridObjects.Count; i++)
        {
            if (!gridObjects[i].isActiveAndEnabled) continue;
            if (gridObjects[i].xInGrid == x && gridObjects[i].yInGrid == y)
            {
                yield return gridObjects[i];
            }
        }
    }

    public Vector2 ConvertWorldToGrid(Vector2 worldPosition)
    {
        return new Vector2(
            (int)(Mathf.Round((worldPosition.x - this.transform.position.x + (cellSize * gridXSize - 2 * cellSize) / 2) / cellSize)) - 1,
            (int)(Mathf.Round((-worldPosition.y - this.transform.position.y + (cellSize * gridYSize - 2 * cellSize) / 2) / cellSize)) + 1);
    }

    public bool IsInBounds(Vector2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridXSize && gridPosition.y >= 0 && gridPosition.y < gridYSize;
    }
}
