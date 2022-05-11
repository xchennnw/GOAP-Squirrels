using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour
{

    public HashSet<KeyValuePair<string, object>> preconditions;
    public HashSet<KeyValuePair<string, object>> effects;

    public bool inRange = false;
    public bool finished = false;
    public bool hasSetTarget = false;
    public float cost = 1f;
    public Vector3 targetPos;
    public string name;

    public GoapAction()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

  
    public abstract void setTarget(GameObject agent);
    public abstract bool checkCondition(GameObject agent);
    public abstract bool perform(GameObject agent);
  

    public void addPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }
    public void addEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }


   
}
