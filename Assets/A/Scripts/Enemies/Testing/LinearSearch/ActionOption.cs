using System;
using UnityEngine;

public class ActionOption
{
    public string name; 
    public float score;
    public Action action;

    public ActionOption(string name, float score, Action action)
    {
        this.name = name;
        this.score = score;
        this.action = action;
    }
}
