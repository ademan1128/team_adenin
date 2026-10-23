using System;

[Serializable]
public class DialogueData
{
    public Dialogue[] dialogues;
}

[Serializable]
public class Dialogue
{
    public string name;
    public string serif;
}