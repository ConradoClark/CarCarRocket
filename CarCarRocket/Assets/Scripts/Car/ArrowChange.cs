using UnityEngine;
using System.Collections;

public class ArrowChange : CarGridObject
{
    public SpriteRenderer spriteRenderer;
    public Rect bounds;
    private bool touched;
    private bool changingDirection;

    public ArrowManager manager;
    public SwipeDetector swipe;

    void Start()
    {
        base.Init();

        spriteRenderer.enabled = true;
        this.alwaysTrigger = this.useCustomDirection = true;
        customDirection = Vector2.up;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        if (Input.touchCount <= 0 && !Input.GetMouseButton(0))
        {
            if (touched && !changingDirection)
            {
                if (customDirection.x + 1 < 0.01)
                {
                    manager.RemoveArrow(this);
                    return;
                    //if (spriteRenderer.enabled)
                    //{
                    //    manager.RemoveArrow(this);
                    //    spriteRenderer.enabled = false;
                    //    this.alwaysTrigger = this.useCustomDirection = false;
                    //    touched = true;
                    //    return;
                    //}
                    //this.alwaysTrigger = this.useCustomDirection = true;
                    //spriteRenderer.enabled = true;
                }

                customDirection = Quaternion.Euler(0, 0, -90) * customDirection;
                customDirection = new Vector2(Mathf.RoundToInt(customDirection.x), Mathf.RoundToInt(customDirection.y));
                spriteRenderer.transform.Rotate(new Vector3(0, 0, -90));
            }
            touched = false;
            changingDirection = false;
            return;
        }

        if (!Input.GetMouseButtonDown(0) && (Input.touchCount == 0 || (Input.touchCount> 0 && Input.GetTouch(0).tapCount > 1))) return;
        
        Vector2 touchPosition = manager.snap.Snap(Camera.main.ScreenToWorldPoint(Input.GetMouseButtonDown(0) ? (Vector2)Input.mousePosition : Input.GetTouch(0).position));
        Vector2 gridPosition = grid.ConvertWorldToGrid(touchPosition);

        if (gridPosition.x == this.xInGrid && gridPosition.y == this.yInGrid && !touched)
        {
            this.swipe.AllowSwipe();
            touched = true;
        }
    }

    public override void ResetState()
    {
        base.ResetState();
        spriteRenderer.enabled = true;
        this.alwaysTrigger = this.useCustomDirection = true;
        customDirection = Vector2.up;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
        touched = false;
    }

    public void ChangeDirection(Vector2 direction)
    {
        customDirection = direction;
        customDirection = new Vector2(Mathf.RoundToInt(customDirection.x), Mathf.RoundToInt(customDirection.y));
        spriteRenderer.transform.rotation = Quaternion.AngleAxis(Vector2.Angle(Vector2.up, customDirection) * (customDirection.x > 0 ? -1 : 1),new Vector3(0,0,1));
        touched = false;
        changingDirection = true;
    }
}