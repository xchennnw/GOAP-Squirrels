using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInATree : GoapAction
{
    // Start is called before the first frame update
    void Start()
    {
        name = "Hide in a Tree";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public HideInATree()
    {
        addPrecondition("playerTooClose", true);
        addEffect("playerTooClose", false);
    }


    public override void setTarget(GameObject agent)
    {
        targetPos = agent.GetComponent<SquirrelController>().currClosestTree.transform.position;
        hasSetTarget = true;
    }

    public override bool checkCondition(GameObject agent)
    {
        return true;
    }

    public override bool perform(GameObject agent)
    {
        agent.GetComponent<SquirrelController>().goToHide();
        finished = true;
        return true;
    }
}
