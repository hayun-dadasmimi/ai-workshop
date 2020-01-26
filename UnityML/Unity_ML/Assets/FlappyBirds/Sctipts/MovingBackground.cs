using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{

    [SerializeField] float speed = 1;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameStarted == true)
        {
            Vector2 offset = new Vector2(Time.time * speed, 0);
            GetComponent<Renderer>().material.mainTextureOffset = offset;
        }
    }
}
