using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    public CarGridObject carGridObject;
    private int savedX, savedY;

    public FrostyPatternMovement carMovement;
    public SpriteRenderer spriteRenderer;
    private bool cw;
    private float delay;
    public float minDistanceToCollide;
    public Vector2 collisionOffsetNegative;
    public Vector2 collisionOffsetPositive;

    private KeyValuePair<bool, CarGridObject> handleCollision;
    private CarGridObject[] objectsInGrid;

    void Start()
    {
        savedX = carGridObject.xInGrid;
        savedY = carGridObject.yInGrid;
    }

    void Update()
    {
        Vector2 dir = carMovement.GetDirection().normalized;
        dir = new Vector2(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));
        delay -= Time.deltaTime;

        if (handleCollision.Key)
        {
            float distance = Vector2.Distance(Vector2.Scale(this.transform.position, dir)
                + Vector2.Scale(collisionOffsetNegative, new Vector2(dir.x < 0 ? -1 : 0, dir.y > 0 ? -1 : 0))
                + Vector2.Scale(collisionOffsetPositive, new Vector2(dir.x > 0 ? -1 : 0, dir.y < 0 ? -1 : 0)),
                Vector2.Scale(handleCollision.Value.transform.position, dir));

            if (distance < minDistanceToCollide)
            {
                for (int i = 0; i < objectsInGrid.Length; i++)
                {
                    CarGridObject obj = objectsInGrid[i];
                    if (obj == handleCollision.Value) continue;

                    Vector2 direction = handleCollision.Value.useCustomDirection ? handleCollision.Value.customDirection : (Vector2)(Quaternion.Euler(0, 0, 90 * (cw ? -1 : 1)) * dir);
                    if ((obj.isObstacle && obj.objectType == CarGridObject.GridObjectType.Wall && ((obj.obstacleDirection.x == -direction.x && obj.obstacleDirection.x != 0) || (obj.obstacleDirection.y == -direction.y && obj.obstacleDirection.y != 0))))
                    {
                        handleCollision = new KeyValuePair<bool, CarGridObject>(true,obj);
                        return;
                    }
                }

                if (handleCollision.Value.useCustomDirection)
                {
                    if ((dir.x == -handleCollision.Value.customDirection.x && dir.x != 0) || (dir.y == -handleCollision.Value.customDirection.y && dir.y != 0))
                    {
                        cw = !cw;
                    }
                    dir = handleCollision.Value.customDirection;
                    carMovement.SetDirection(dir);
                    spriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sign(dir.x) * -Vector2.Angle(Vector2.up, dir)));
                    handleCollision = new KeyValuePair<bool, CarGridObject>(false, null);
                    return;
                }

                if (Vector2.Scale(dir, handleCollision.Value.obstacleDirection) == Vector2.zero)
                {
                    handleCollision = new KeyValuePair<bool, CarGridObject>(false, null);
                    return;
                }


                cw = (dir.x > 0 && handleCollision.Value.obstacleDirection.y <= 0) || (dir.y > 0 && handleCollision.Value.obstacleDirection.x > 0) || (dir.y < 0 && handleCollision.Value.obstacleDirection.x <= 0)
                   || (dir.x < 0 && handleCollision.Value.obstacleDirection.y > 0);
                //cw = cw && Mathf.Sign(dir.x * handleCollision.Value.obstacleDirection.x == -1 ? -1 : 1) != ((handleCollision.Value.obstacleDirection.y*-dir.y == -1 ? -1 : 1)) ? true : cw;

                float value = cw ? -1 : 1; 
                dir = Quaternion.Euler(0, 0, 90 * value) * dir;
                carMovement.SetDirection(dir);
                spriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sign(dir.x) * -Vector2.Angle(Vector2.up, dir)));
                handleCollision = new KeyValuePair<bool, CarGridObject>(false, null);
            }
        }

        if (carGridObject.xInGrid == savedX && carGridObject.yInGrid == savedY) return;

        savedX = carGridObject.xInGrid;
        savedY = carGridObject.yInGrid;

        objectsInGrid = carGridObject.grid.GetObjectsInCell(savedX, savedY).ToArray();

        for (int i = 0; i < objectsInGrid.Length; i++)
        {
            CarGridObject obj = objectsInGrid[i];

            if (obj.alwaysTrigger || (obj.isObstacle && ((obj.obstacleDirection.x != 0 || obj.obstacleDirection.y != 0) && (obj.obstacleDirection.x == -dir.x || obj.obstacleDirection.y == -dir.y))))
            {
                if (handleCollision.Key)
                {
                    handleCollision = new KeyValuePair<bool, CarGridObject>(true, obj.objectType > handleCollision.Value.objectType ? obj : handleCollision.Value);
                    continue;
                }
                handleCollision = new KeyValuePair<bool, CarGridObject>(true, obj);
                //if (obj.useCustomDirection)
            }
        }


        //if (delay <= 0 && ((onHWallCollision.Value && dir.x != 0) || (onVWallCollision.Value && dir.y != 0)))
        //{
        //    float value = ccw ? -1 : 1;
        //    dir = Quaternion.Euler(0, 0, 90 * value) * dir;
        //    carMovement.SetDirection(dir);
        //    spriteRenderer.transform.Rotate(new Vector3(0, 0, 90 * value));
        //    delay = 0.2f;
        //    for (int i = 0; i < collisions.Length; i++)
        //    {
        //        collisions[i].overriddenRayDirection = dir;
        //    }

        //    return;
        //}

        //if (onArrowCollision.Value && delay <= 0)
        //{
        //    ArrowChange arr = arrowEventCollision.impactOnPoints[0].collider.GetComponent<ArrowChange>();
        //    if (dir.x == -arr.direction.x || dir.y == -arr.direction.y)
        //    {
        //        ccw = !ccw;
        //    }
        //    dir = arr.direction; //Quaternion.Euler(0, 0, 90) * dir;
        //    carMovement.SetDirection(dir);
        //    spriteRenderer.transform.rotation = Quaternion.AngleAxis(0, dir);
        //    delay = 0.2f;

        //    for (int i = 0; i < collisions.Length; i++)
        //    {
        //        collisions[i].overriddenRayDirection = dir;
        //    }
        //    return;
        //}
    }
}
