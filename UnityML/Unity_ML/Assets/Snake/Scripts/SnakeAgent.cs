using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SnakeAgent : Agent
{
    public Transform center;
    public Transform head;
    public GameObject tailOrigin;
    public GameObject point;
    public Transform monitor;
    public List<GameObject> tails = new List<GameObject>();

    public float maxTime;
    float resetPosTime = 0;

    private const int nothing = 0;
    private const int right = 1;
    private const int left = 2;
    private const int up = 3;
    private const int down = 4;

    public int inputValue = 2;
    public int lastInput = 4;
    int score = 0;

    public bool isPointEaten = false;
    public bool isDead = false;

    Vector3 newPos = Vector3.zero;

    private void Awake()
    {
        ResetSnake();
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
        point.transform.localPosition = new Vector2(Mathf.FloorToInt(Random.Range(-9, 9)) * 0.2f, Mathf.FloorToInt(Random.Range(-19, 19)) * 0.2f);
        head.localPosition = new Vector2(Mathf.FloorToInt(Random.Range(-9, 9)) * 0.2f, Mathf.FloorToInt(Random.Range(-19, 19)) * 0.2f);
        if (point.transform.position == head.transform.position)
        {
            point.transform.localPosition = new Vector2(Mathf.FloorToInt(Random.Range(-9, 9)) * 0.2f, Mathf.FloorToInt(Random.Range(-19, 19)) * 0.2f);
        }
        isPointEaten = false;
        score = 0;
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

    void DeadCheck()
    {
        if (center.position.x + 2 <= head.position.x || head.position.x <= center.position.x -2 ||
            center.position.y + 4 <= head.position.y || head.position.y <= center.position.y -4)
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
            AddReward(-1f);
            Done();
            isDead = false;
        }
    }

    public override void AgentReset()
    {
        ResetSnake();
    }

    public override void CollectObservations()
    {
        Vector3 distanceToPoint = point.transform.position - head.position;

        AddVectorObs(Mathf.Clamp(distanceToPoint.x / 2, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToPoint.y / 4, -1f, 1f));

        Vector3 relativePos = transform.position - center.position;

        AddVectorObs(Mathf.Clamp(relativePos.x / 2, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.y / 4, -1f, 1f));

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        resetPosTime++;
        AddReward(-0.001f);
        
        inputValue = Mathf.FloorToInt(vectorAction[0]);
        if (inputValue == 0)
        {
            inputValue = lastInput;
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
                    newPos = head.localPosition + new Vector3(0.2f, 0, 0);
                    break;
                case left:
                    newPos = head.localPosition + new Vector3(-0.2f, 0, 0);
                    break;
                case up:
                    newPos = head.localPosition + new Vector3(0, 0.2f, 0);
                    break;
                case down:
                    newPos = head.localPosition+ new Vector3(0, -0.2f, 0);
                    break;
            }

            if (isPointEaten)
            {
                tails.Add(Instantiate(tailOrigin));
                score++;
                AddReward(1f);
                point.transform.localPosition = ResetPoint();
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
            head.localPosition = newPos;

            if (head.position == point.transform.position)
            {
                isPointEaten = true;
            }

            lastInput = inputValue;
            resetPosTime = 0;
            DeadCheck();
        }
        Monitor.Log(name, GetCumulativeReward(), monitor);
    }




}
