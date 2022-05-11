using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGarbage : GoapAction
{
	private GameObject target;
	private TreeAndCanManager treeScript;

	// Start is called before the first frame update
	void Start()
	{
		name = "Pick Up Garbage";
		treeScript = GameObject.Find("TreeCanManager").GetComponent<TreeAndCanManager>();
		cost = 0.9f;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public PickUpGarbage()
	{
		addPrecondition("has1Nut", false);
		addPrecondition("has2Nuts", false);
		addPrecondition("has3Nuts", false);
		addPrecondition("hasGarbage", false);
		addPrecondition("playerTooClose", false);

		addEffect("hasGarbage", true);
		addEffect("collectFood", true);
	}


	public override void setTarget(GameObject agent)
	{
		int xx = Random.Range(0, agent.GetComponent<SquirrelController>().twoCans.Count);
		target = agent.GetComponent<SquirrelController>().twoCans[xx];
		targetPos = agent.GetComponent<SquirrelController>().twoCans[xx].transform.position;
		hasSetTarget = true;
	}

	public override bool checkCondition(GameObject agent)
	{
		if (agent.GetComponent<SquirrelController>().twoCans.Count > 0)
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
		if (target != null)
		{
            if (target.GetComponent<CanController>().Full)
            {				
				var c = Instantiate(target.GetComponent<CanController>().garbage, target.transform.position, Quaternion.identity) as GameObject;
				agent.GetComponent<SquirrelController>().addGarbage(c);
				c.transform.position = new Vector3(agent.transform.position.x, agent.transform.position.y + 2f, agent.transform.position.z + 1.25f);
				c.transform.parent = agent.transform;
				finished = true;
				target.GetComponent<CanController>().changeState();
				return true;
            }
            else
            {
				
				agent.GetComponent<SquirrelController>().getStuck();
				return false;
            }			
		}
		else
		{
			return false;
		}
	}
}
