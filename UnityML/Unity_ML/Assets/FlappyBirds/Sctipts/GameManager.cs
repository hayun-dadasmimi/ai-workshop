using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public int score;
    public Text scoreText;
    [SerializeField] GameObject[] birds;
    [SerializeField] GameObject obstacle;

    public bool isGameStarted = false;

    // Use this for initialization
    void Start()
    {
        instance = this;
        score = 0;
        StartGame();        
    }

    public void StartGame()
    {
        for(int i = 0; i < birds.Length; i++)
        {
            birds[i].SetActive(true);
            birds[i].GetComponent<BirdAgent>().AgentReset();
        }
        isGameStarted = true;
        Instantiate(obstacle, new Vector3(1, Random.Range(-0.26f, 0.54f), -1), Quaternion.identity);
        StartCoroutine(SpawnObstacles());
        StartCoroutine(BirdsCheck());
    }

    public void RestartGame()
    {
        score = 0;
        isGameStarted = false;
        StopAllCoroutines();
        Invoke("StartGame", 0.1f);
    }

    private void Update()
    {
        score = MaxBirdsScore();
        scoreText.text = score.ToString();

        if(Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < birds.Length; i++)
            {
                birds[i].SetActive(false);
            }

            RestartGame();
        }
    }

    IEnumerator SpawnObstacles()
    {
        while (GameManager.instance.isGameStarted == true)
        {            
            yield return new WaitForSeconds(1.5f);
            Instantiate(obstacle, new Vector3(1, Random.Range(-0.26f, 0.54f), -1), Quaternion.identity);
        }

    }

    IEnumerator BirdsCheck()
    {
        while(IsBirdExist())
        {
            yield return new WaitForSeconds(0.01f);
        }
        RestartGame();
    }

    bool IsBirdExist()
    {
        List<bool> list = new List<bool>();

        for(int i = 0; i < birds.Length; i++)
        {
            list.Add(birds[i].activeInHierarchy);
        }

        if(list.Contains(true))
        {
            return true;
        }else
        {
            return false;
        }
    }

    int MaxBirdsScore()
    {
        int[] list = new int[birds.Length];
        for (int i = 0; i < birds.Length; i++)
        {
            list[i] = birds[i].GetComponent<BirdAgent>().score;
        }

        return Mathf.Max(list);
    }
}
