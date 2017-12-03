using System.Collections;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour {


    public Transform playerPositions;
    [Tooltip("in seconds")]
    public float swapTime = 0.25f;
    [Tooltip("in pixels")]
    public int swapDistance = 100;

    float distance = 4f;
    Player player;
    Transform[] points;
    int currentPoint;
    int pointCount;

    bool canSwipe = true;
    bool isDrag = false;
    Vector2 startPoint;
    Vector2 Swapdelta;


    void Start()
    {
        GetPoints();
        transform.position = points[currentPoint].position;
        player = gameObject.GetComponent<Player>();
        distance = (points[currentPoint].position - points[currentPoint - 1].position).magnitude;
    }

    void Update()
    {
        #region StandAlone
        if (Input.GetMouseButtonDown(0) && canSwipe)
        {
            isDrag = true;
            startPoint = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDrag) player.Tap();
            isDrag = false;
        }
        #endregion
        #region Mobile
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began && canSwipe)
            {
                isDrag = true;
                startPoint = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
            {
                if (isDrag) player.Tap();
                isDrag = false;
            }
        }
        #endregion

        if (isDrag)
        {
            if (Input.touches.Length > 0)
            {
                Swapdelta = Input.touches[0].position - startPoint;
            }
            else if (Input.GetMouseButton(0))
            {
                Swapdelta = (Vector2)Input.mousePosition - startPoint;
            }

            if (Swapdelta.x > swapDistance)
            {
                Move(1);
                StartCoroutine(MoveTime());
            }
            else if (Swapdelta.x < -swapDistance)
            {
                Move(-1);
                StartCoroutine(MoveTime());
            }
        }

        if (Input.GetKeyDown("a")) Move(-1);
        if (Input.GetKeyDown("d")) Move(1);
        transform.position = Vector2.MoveTowards(transform.position, points[currentPoint].position, (distance / swapTime) * Time.deltaTime);
    }

    bool Move(int number)
    {
        currentPoint += number;
        if (currentPoint < 0)
        {
            currentPoint = 0;
            return false;
        }
        else if (currentPoint >= pointCount)
        {
            currentPoint = pointCount - 1;
            return false;
        }
        //transform.position = points[currentPoint].position;
        return true;
    }

    void GetPoints()
    {
        pointCount = playerPositions.childCount;
        points = new Transform[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = playerPositions.GetChild(i);
        }
        currentPoint = pointCount / 2;
    }

    IEnumerator MoveTime()
    {
        isDrag = false;
        canSwipe = false;
        yield return new WaitForSeconds(swapTime);
        canSwipe = true;
    }
}