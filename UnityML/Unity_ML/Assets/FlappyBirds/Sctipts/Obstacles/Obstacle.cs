using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] float speed;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Death());
        StartCoroutine(DeathCheck());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameStarted == true)
            transform.position -= Vector3.right * speed * Time.deltaTime;//new Vector3(0.1f * speed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z);
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(5);
        if (GameManager.instance.isGameStarted == true)
            Destroy(gameObject);
    }
    IEnumerator DeathCheck()
    {
        while(GameManager.instance.isGameStarted)
        {
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
}
