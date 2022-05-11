using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutNutHome : GoapAction
{
    
    // Start is called before the first frame update
    void Start()
    {
        name = "Put Nut Home";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PutNutHome()
    {
        addPrecondition("has3Nuts", true);
        addPrecondition("playerTooClose", false);

        addEffect("has1Nut", false);
        addEffect("has2Nuts", false);
        addEffect("has3Nuts", false);
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
        agent.GetComponent<SquirrelController>().dropNuts();
        finished = true;
        return true;
    }
}
