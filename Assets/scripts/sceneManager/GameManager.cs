using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public Text ChaosScore;
    public GameObject NotBoss;//按钮
    public GameObject ToBoss;//按钮
    public int chaos = 0;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        if (null == instance)
        {
            Debug.Log("GameManager:null == instance");
        }
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ChaosScore.text = chaos.ToString();
        if (chaos >= 100)
        {
            instance.NotBoss.SetActive(false);
            instance.ToBoss.SetActive(true);
        }
    }
    public static void addScore(int added)
    {
        instance.chaos += added;
    }
    public static void NextScene()//给按钮用来切换到boss战
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
