using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    private int state;

    private int State {
        get { return state; }
        
        set {
            state = value;
            if (State >= 0)
                UpdateOverlay();
        }
    }

    [SerializeField]
    private TextAsset elementsTextAsset;

    [SerializeField]
    private PauseScreen pauseScreenScript;

    [SerializeField]
    private GameObject tutorialOverlay;

    [SerializeField]
    private TextMeshProUGUI tutorialText;

    [SerializeField]
    private GameObject keyImage1;

    [SerializeField]
    private GameObject keyImage2;

    [SerializeField]
    private TextMeshProUGUI keyImage1Text;

    [SerializeField]
    private TextMeshProUGUI keyImage2Text;

    private TutorialElement[] tutorialElements;

    private GameObject player;

    private GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");

        TutorialElementManager tutorialElementManager = new TutorialElementManager();
        tutorialElements = tutorialElementManager.ReadElementsFromFile(elementsTextAsset);

        State = 0;
        ShowOverlay();
    }

    // Update is called once per frame
    void Update()
    {
        //The reason I checked if State > 4 here is in fact for optimization. No need for distance calculation at the beginning of the scene.
        if (State == 4 && SquaredDistanceBetweenPlayerAndEnemy() < 100)
            EnemyWarning();
    }

    private void UpdateOverlay()
    {
        tutorialText.text = tutorialElements[state].text;

        keyImage1Text.text = tutorialElements[state].key1Value;
        keyImage2Text.text = tutorialElements[state].key2Value;

        keyImage1.SetActive(!string.IsNullOrEmpty(tutorialElements[state].key1Value));
        keyImage2.SetActive(!string.IsNullOrEmpty(tutorialElements[state].key2Value));
    }

    private void ShowOverlay()
    {
        pauseScreenScript.Pause(false);
        tutorialOverlay.SetActive(true);
    }

    private void HideOverlay()
    {
        pauseScreenScript.Resume();
        tutorialOverlay.SetActive(false);
    }

    public void OKButtonPressed()
    {
        HideOverlay();

        if (State < 2)
        {
            State++;
            Invoke("ShowOverlay", 1);
        }
    }

    public void TrapWarning()
    {
        if (State >= 3)
            return;
        
        CancelInvoke();
        State = 3;
        ShowOverlay();
        
    }

    public void HookWarning()
    {
        if (State >= 4)
            return;

        CancelInvoke();
        State = 4;
        ShowOverlay();
        
    }

    public void EnemyWarning()
    {
        if (State >= 5)
            return;

        CancelInvoke();
        State = 5;
        ShowOverlay();
    }

    private float SquaredDistanceBetweenPlayerAndEnemy()
    {
        return Mathf.Pow(player.transform.position.x - enemy.transform.position.x, 2) + Mathf.Pow(player.transform.position.y - enemy.transform.position.y, 2);
    }
}
