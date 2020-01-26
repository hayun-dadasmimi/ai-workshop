using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BirdAgent : Agent
{
    public Rigidbody2D rb;
    Animator animator;
    Coroutine lookUp;

    float flapForce = 5.5f;
    float turnSpeed = 3;
    public int score;

    public bool isAlive;
    public bool isArrivedGoal = false;

    // Use this for initialization
    void Awake()
    {
        //isAlive = false;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.instance.isGameStarted)
        {
            GetComponent<Rigidbody2D>().simulated = true;
            //if (Input.GetButtonDown("Jump"))
            //{
            //    Flap();
            //}
        }
    }

    IEnumerator LookUp()
    {
        animator.SetBool("up", true);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("up", false);
    }


    public void Flap()
    {
        if (isAlive)
        {
            rb.velocity = new Vector3(0, 0);
            rb.AddForce(new Vector2(0.431f, 0.107f) * flapForce, ForceMode2D.Impulse);
            if (lookUp != null)
                StopCoroutine(lookUp);
            lookUp = StartCoroutine(LookUp());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "deathTrigger")
        {
            isAlive = false;
            score = 0;
        }
        if (collision.gameObject.tag == "scoreTrigger")
        {
            isArrivedGoal = true;
            score++;
        }
    }

    public override void AgentReset()
    {
        transform.position = new Vector2(-0.25f, Random.Range(-0.8f, 0.8f));
        rb.velocity = new Vector3(0, 0);
        GetComponent<Animator>().SetBool("Start", true);
        score = 0;
        isAlive = true;
    }

    public override void CollectObservations()
    {
        // 가장 가까운 골포인트 찾기
        GameObject[] goals = GameObject.FindGameObjectsWithTag("scoreTrigger");
        float[] dists = new float[goals.Length];
        float minDist = 10;
        GameObject closetGoal = goals[0];
        for (int i = 0; i < dists.Length; i++)
        {
            dists[i] = Vector2.Distance(this.transform.position, goals[i].transform.position);
            if (minDist > dists[i] || goals[i].transform.position.x - transform.position.x >= 0)
            {
                minDist = dists[i];
                closetGoal = goals[i];
            }
        }
        //---

        AddVectorObs(Mathf.Clamp(closetGoal.transform.position.x, -1f, 1f));
        AddVectorObs(Mathf.Clamp(closetGoal.transform.position.y, -1f, 1f));

        AddVectorObs(Mathf.Clamp(this.transform.position.x, -1f, 1f));
        AddVectorObs(Mathf.Clamp(this.transform.position.y, -1f, 1f));

        AddVectorObs(Mathf.Clamp(rb.velocity.y / 10, -1f, 1f));

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(0.1f);

        int action = Mathf.FloorToInt(vectorAction[0]);
        if(action == 1)
        {
            Flap();
        }

        if(isArrivedGoal)
        {
            AddReward(score);
            isArrivedGoal = false;
        }
        if(!isAlive)
        {
            AddReward(-5.0f);
            Done();
            gameObject.SetActive(false);                        
        }

        Monitor.Log(name, GetCumulativeReward(), transform);
    }

}
