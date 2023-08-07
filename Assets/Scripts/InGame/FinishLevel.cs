using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLevel : MonoBehaviour
{

    private GameObject finishScreen;

    private PauseScreen pauseScreenScript;

    private float completionTime;

    // Start is called before the first frame update
    void Start()
    {
        finishScreen = GameObject.Find("GameUICanvas").transform.Find("FinishScreen").gameObject;
        pauseScreenScript = GameObject.FindGameObjectWithTag("PauseScreen").GetComponent<PauseScreen>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseScreenScript.paused)
            completionTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            EnableFinishScreen();
    }

    private void EnableFinishScreen()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
            pauseScreenScript.ReturnToMainMenu();
        else
        {
            completionTime = Mathf.Round(completionTime * 100) / 100;
            finishScreen.GetComponent<FinishScreen>().completionTime = completionTime;
            finishScreen.SetActive(true);
        }
    }
}