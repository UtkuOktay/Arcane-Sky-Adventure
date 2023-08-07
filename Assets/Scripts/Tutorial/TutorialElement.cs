using System;

[Serializable]
public class TutorialElement
{
    public string text;
    public string key1Value;
    public string key2Value;

    public TutorialElement(string text, string key1Value, string key2Value)
    {
        this.text = text;
        this.key1Value = key1Value;
        this.key2Value = key2Value;
    }
}
