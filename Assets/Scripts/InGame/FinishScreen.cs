using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishScreen : MonoBehaviour
{

    [SerializeField]
    private GameObject window;

    private PauseScreen pauseScreenScript;

    private GameObject player;

    Player playerScript;

    [SerializeField]
    private RawImage darkOverlayImage;

    private Color32 color;

    private float colorAlpha;

    public float completionTime;

    // Start is called before the first frame update
    void Start()
    {
        color = darkOverlayImage.color;

        pauseScreenScript = GameObject.FindGameObjectWithTag("PauseScreen").GetComponent<PauseScreen>();

        player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        player.GetComponent<Animator>().SetFloat("velocity", 0);

        playerScript = player.GetComponent<Player>();
        pauseScreenScript.paused = true;
        Invoke("OpenWindow", 0.5f);

        WriteFinishStats();
    }

    // Update is called once per frame
    void Update()
    {
        Fade();
    }

    private void Fade()
    {
        if (colorAlpha >= 200)
            return;

        colorAlpha += 400 * Time.deltaTime;
        color.a = (byte)colorAlpha;
        darkOverlayImage.color = color;
    }

    private void OpenWindow()
    {
        window.SetActive(true);
    }

    private void WriteFinishStats()
    {
        int coinsEarned = playerScript.numberOfCoins;
        float HP = playerScript.Health;
        int respawns = playerScript.respawns;
        int defeatedEnemies = playerScript.defeatedEnemies;

        window.transform.Find("Values").GetComponent<TextMeshProUGUI>().text = $"{completionTime} s\n{coinsEarned}\n{HP}\n{respawns}\n{defeatedEnemies}";

        //1. 20 seconds worths 2500 points, halving completion time doubles the score.
        int timePoints = Mathf.RoundToInt(50000 / completionTime);

        //2. Each coin worths 200 points.
        int coinPoints = 200 * coinsEarned;

        //3. An HP worths 2 points.
        int hpPoints = Mathf.RoundToInt(HP * 2);

        //4. Until 5, each respawn costs 200 points, but after that the increase rate of the respawn penalty decreases as it uses a logarithmic scale.
        int respawnPenalty = 200 * respawns;

        if (respawns > 5)
            respawnPenalty = Mathf.RoundToInt(1500f * Mathf.Log10(respawns));

        //5. Each defeated enemy worths 1000 points.
        int defeatedEnemyPoints = 1000 * defeatedEnemies;

        float points = timePoints + coinPoints + hpPoints - respawnPenalty + defeatedEnemyPoints;
        if (points < 0)
            points = 0;

        window.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = "Score: " + points;

        window.transform.Find("Points").GetComponent<TextMeshProUGUI>().text = $"{timePoints}\n{coinPoints}\n{hpPoints}\n-{respawnPenalty}\n{defeatedEnemyPoints}";
    }

    public void ReturnToMainMenu()
    {
        pauseScreenScript.ReturnToMainMenu();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
