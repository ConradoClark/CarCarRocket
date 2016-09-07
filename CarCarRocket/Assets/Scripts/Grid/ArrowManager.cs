using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ArrowManager : MonoBehaviour
{
    public FrostySnapToGrid snap;
    public CarGrid grid;
    private bool touched;
    public int numberOfArrows;
    public FrostyPoolInstance arrowInstance;

    private int arrowsUsed;
    private List<GameObject> arrows;
    private bool removedArrow;

    void Awake()
    {
        arrows = new List<GameObject>();
    }

    void Update()
    {
        if (removedArrow || (Input.touchCount <= 0 && !Input.GetMouseButtonDown(0)))
        {
            removedArrow = false;
            touched = false;
            return;
        }

        var touchPosition = snap.Snap(Camera.main.ScreenToWorldPoint(Input.GetMouseButtonDown(0) ? (Vector2)Input.mousePosition : Input.GetTouch(0).position));
        Vector2 gridPosition = grid.ConvertWorldToGrid(touchPosition);

        if (!touched && grid.IsInBounds(gridPosition))
        {
            touched = true;
            if (grid.GetObjectsInCell((int)gridPosition.x, (int)gridPosition.y).Any(obj => obj.objectType == CarGridObject.GridObjectType.Arrow)) return;

            GameObject arrowObj = Toolbox.Instance.pool.Retrieve(arrowInstance);
            ArrowChange component = arrowObj.GetComponent<ArrowChange>();
            component.manager = this;
            component.grid = grid;
            component.swipe.AllowSwipe();

            grid.AddObject(component);

            arrowObj.transform.position = new Vector3(touchPosition.x,touchPosition.y,arrowObj.transform.position.z);
            snap.Snap(arrowObj.transform);

            if (arrowsUsed == numberOfArrows && arrows.Count > 0)
            {
                var arrow = arrows.Last();
                arrows.RemoveAt(arrows.Count - 1);
                Toolbox.Instance.pool.Release(arrowInstance, arrow);
                arrowsUsed--;
            }

            arrows.Insert(0,arrowObj);
            arrowsUsed++;
        }
    }

    public void RemoveArrow(ArrowChange arrow)
    {
        arrowsUsed--;
        removedArrow = true;
        Toolbox.Instance.pool.Release(arrowInstance, arrow.gameObject);
        if (arrows.Count > 0)
        {
            arrows.Remove(arrow.gameObject);
        }
    }
}
