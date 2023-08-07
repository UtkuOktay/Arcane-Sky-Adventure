using System;
using UnityEngine;

public class TutorialElementManager
{

    public TutorialElement[] ReadElementsFromFile(TextAsset textAsset)
    {
        return JsonUtility.FromJson<TutorialElements>(textAsset.text).tutorialElements;
    }


    //Since JsonUtility in Unity does not support reading arrays directly, I needed to create a class to hold the array.
    [Serializable]
    private class TutorialElements
    {
        public TutorialElement[] tutorialElements;
    }
}
