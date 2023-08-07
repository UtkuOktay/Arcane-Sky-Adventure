using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour
{

    [SerializeField]
    private GameObject startButtons;

    [SerializeField]
    private GameObject levels;

    public void StartGameButtonClicked()
    {
        startButtons.SetActive(false);
        levels.SetActive(true);
    }

    public void StartTutorialButtonClicked()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ReturnToMainMenuButtonClicked()
    {
        levels.SetActive(false);
        startButtons.SetActive(true);
    }

    public void StartLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void StartLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
