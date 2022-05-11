using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutGarbageHome : GoapAction
{
    // Start is called before the first frame update
    void Start()
    {
        name = "Put Garbage Home";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public PutGarbageHome()
    {
        addPrecondition("hasGarbage", true);
        addPrecondition("playerTooClose", false);

        addEffect("hasGarbage", false);
        addEffect("putFoodHome", true);

    }


    public override void setTarget(GameObject agent)
    {
        targetPos = agent.GetComponent<SquirrelController>().homeTree.transform.position;
        hasSetTarget = true;
    }

    public override bool checkCondition(GameObject agent)
    {
        return agent.GetComponent<SquirrelController>().currClosestTree == agent.GetComponent<SquirrelController>().homeTree;
    }

    public override bool perform(GameObject agent)
    {
        agent.GetComponent<SquirrelController>().dropGarbage();
        finished = true;
        return true;
    }
}
