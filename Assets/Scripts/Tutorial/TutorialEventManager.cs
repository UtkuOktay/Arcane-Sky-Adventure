using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventManager : MonoBehaviour
{

    private Tutorial tutorialScript;

    private enum EventType {Trap, Hook, Enemy, Flag};

    [SerializeField]
    private EventType eventType;

    // Start is called before the first frame update
    void Start()
    {
        tutorialScript = GameObject.Find("Tutorial").GetComponent<Tutorial>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        if (eventType == EventType.Trap)
            tutorialScript.TrapWarning();

        else if (eventType == EventType.Hook)
            tutorialScript.HookWarning();
    }
}
