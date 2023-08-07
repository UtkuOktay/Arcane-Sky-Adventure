using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseOverlay;

    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (paused)
            Resume();
        else
            Pause();
    }

    public void Pause(bool openPauseMenu = true)
    {
        Time.timeScale = 0f;
        paused = true;
        pauseOverlay.SetActive(openPauseMenu);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        paused = false;
        pauseOverlay.SetActive(false);
    }

    public void Restart()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}