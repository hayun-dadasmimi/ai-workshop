using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Rigidbody2D rb;
    Animator animator;
    Coroutine lookUp;

    public float flapForce;
    public float turnSpeed;

    public bool isAlive;

    // Use this for initialization
    void Awake()
    {
        isAlive = false;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.instance.isGameStarted)
        {
            GetComponent<Rigidbody2D>().simulated = true;
            if (Input.GetMouseButtonDown(0))
            {
                Flap();
            }
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

        if (!isAlive)
        {
            GetComponent<Animator>().SetBool("Start", true);
            isAlive = true;
            Flap();
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "deathTrigger")
        {
            isAlive = false;
            gameObject.SetActive(false);
        }
    }
}
