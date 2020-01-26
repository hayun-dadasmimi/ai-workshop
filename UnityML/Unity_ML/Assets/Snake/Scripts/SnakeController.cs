using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public Transform center;
    public Transform head;
    public GameObject tailOrigin;
    public GameObject point;
    public List<GameObject> tails = new List<GameObject>();

    public float maxTime;
    float resetPosTime = 0;

    private const int right = 1;
    private const int left = 2;
    private const int up = 3;
    private const int down = 4;

    public int inputValue = 2;
    public int lastInput = 4;
    int score = 0;

    public bool isPointEaten = false;
    public bool isDead = false;

    public enum state { right, left, up, down }

    Vector3 newPos;

    private void Awake()
    {
        ResetSnake();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SnakeMove();
    }

    void ResetSnake()
    {
        lastInput = Random.Range(1, 5);
        inputValue = Random.Range(1, 5);
        for (int i = 0; i < tails.Count; i++)
        {
            Destroy(tails[i]);
        }
        tails = new List<GameObject>();
        point.transform.position = new Vector2(Mathf.FloorToInt(Random.Range(-9, 9)) * 0.2f, Mathf.FloorToInt(Random.Range(-19, 19)) * 0.2f);
        head.position = new Vector2(Mathf.FloorToInt(Random.Range(-9, 9)) * 0.2f, Mathf.FloorToInt(Random.Range(-19, 19)) * 0.2f);
        if (point.transform.position == head.transform.position)
        {
            point.transform.position = new Vector2(Mathf.FloorToInt(Random.Range(-9, 9)) * 0.2f, Mathf.FloorToInt(Random.Range(-19, 19)) * 0.2f);
        }
        isPointEaten = false;

    }

    Vector3 ResetPoint()
    {
        Vector3 newPoint = new Vector2(Mathf.FloorToInt(Random.Range(-9, 9)) * 0.2f, Mathf.FloorToInt(Random.Range(-19, 19)) * 0.2f);

        List<bool> list = new List<bool>();
        for (int i = 0; i < tails.Count; i++)
        {
            if (newPoint == tails[i].transform.position)
            {
                list.Add(true);
            }
        }
        if (newPoint == head.position)
        {
            list.Add(true);
        }

        if (list.Contains(true))
        {
            return ResetPoint();
        }
        else
        {
            return newPoint;
        }
    }

    void SnakeMove()
    {
        resetPosTime++;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputValue = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputValue = 2;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            inputValue = 3;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            inputValue = 4;
        }

        if ((inputValue == 1 || inputValue == 2) && (lastInput == 1 || lastInput == 2))
        {
            inputValue = lastInput;
        }
        if ((inputValue == 3 || inputValue == 4) && (lastInput == 3 || lastInput == 4))
        {
            inputValue = lastInput;
        }


        if (resetPosTime >= maxTime)
        {
            switch (inputValue)
            {
                case right:
                    newPos = head.position + new Vector3(0.2f, 0, 0);
                    break;
                case left:
                    newPos = head.position + new Vector3(-0.2f, 0, 0);
                    break;
                case up:
                    newPos = head.position + new Vector3(0, 0.2f, 0);
                    break;
                case down:
                    newPos = head.position + new Vector3(0, -0.2f, 0);
                    break;
            }

            if (isPointEaten)
            {
                tails.Add(Instantiate(tailOrigin));
                score++;
                point.transform.position = ResetPoint();
                isPointEaten = false;
            }

            if (tails.Count != 0)
            {
                for (int i = tails.Count - 1; i > 0; i--)
                {
                    tails[i].transform.position = tails[i - 1].transform.position;
                }
                tails[0].transform.position = head.position;
            }
            head.position = newPos;

            if (head.position == point.transform.position)
            {
                isPointEaten = true;
            }

            lastInput = inputValue;
            resetPosTime = 0;
        }

        DeadCheck();
    }

    void DeadCheck()
    {
        if (2 <= head.position.x || head.position.x <= -2 || 4 <= head.position.y || head.position.y <= -4)
        {
            isDead = true;
        }

        for (int i = 0; i < tails.Count; i++)
        {
            if (tails[i].transform.position == head.position)
            {
                isDead = true;
            }
        }

        if (isDead)
        {
            ResetSnake();
            isDead = false;
        }
    }
}
