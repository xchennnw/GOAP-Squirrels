using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpNut3 : GoapAction
{

	private GameObject target;
	private TreeAndCanManager treeScript;

	// Start is called before the first frame update
	void Start()
	{
		name = "Pick Up 3rd Nut";
		treeScript = GameObject.Find("TreeCanManager").GetComponent<TreeAndCanManager>();
		cost = 0.3f;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public PickUpNut3()
	{
		addPrecondition("has1Nut", true);
		addPrecondition("has2Nuts", true);
		addPrecondition("has3Nuts", false);
		addPrecondition("hasGarbage", false);
		addPrecondition("playerTooClose", false);

		addEffect("has3Nuts", true);
		addEffect("collectFood", true);
	}

	
	
	public override void setTarget(GameObject agent)
	{
		int xx = Random.Range(0, agent.GetComponent<SquirrelController>().nutPositions.Count);
		if (agent.GetComponent<SquirrelController>().nutPositions[xx] != null)
		{
			target = agent.GetComponent<SquirrelController>().memoNuts[xx];
		}
		else
		{
			target = null;
		}
		targetPos = agent.GetComponent<SquirrelController>().nutPositions[xx];
		hasSetTarget = true;
		agent.GetComponent<SquirrelController>().memoNuts.Remove(target);
	}
	public override bool checkCondition(GameObject agent)
	{
		if (agent.GetComponent<SquirrelController>().nutPositions.Count > 2)
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	public override bool perform(GameObject agent)
	{
		if (target != null && !target.GetComponent<NutScript>().Picked())
		{
			target.GetComponent<NutScript>().nowPick();
			treeScript.removeFromNuts(target);
			finished = true;
			agent.GetComponent<SquirrelController>().nutPositions.Remove(targetPos);
			agent.GetComponent<SquirrelController>().memoNuts.Remove(target);
			agent.GetComponent<SquirrelController>().addNut(target);
			target.transform.position = new Vector3(agent.transform.position.x, agent.transform.position.y + 2f, agent.transform.position.z - 1.25f);
			target.transform.parent = agent.transform;
			return true;
		}
		else
		{			
			return false;
		}
	}
}
