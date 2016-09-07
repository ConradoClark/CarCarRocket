using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CarGridObject : FrostyPoolableObject
{
    public CarGrid grid;
    public bool isObstacle;
    public bool useCustomDirection;
    public Vector2 customDirection;
    public bool alwaysTrigger;
    public Vector2 obstacleDirection;
    public SpriteRenderer gizmo;
    public GridObjectType objectType;

    public enum GridObjectType
    {
        Wall = 0,
        Arrow = 1,
        Car = 2,
        FinishLine = 3
    }

    [Header("Grid Position")]
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public int xInGrid;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public int yInGrid;

    void Start()
    {
        Init();

        if (Application.isPlaying && gizmo!=null)
        {
            gizmo.enabled = false;
        }
    }

    public virtual void Init()
    {
        if (grid != null)
        {
            grid.AddObject(this);
        }
    }

    void Update()
    {
        if (Application.isPlaying) return;

        if (gizmo == null) return;

        Vector2 direction = useCustomDirection ? customDirection.normalized : obstacleDirection.normalized;
        gizmo.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sign(direction.x * (direction.y == -1 ? 1 : -1)) * Vector2.Angle(Vector2.up, direction));
    }
}
