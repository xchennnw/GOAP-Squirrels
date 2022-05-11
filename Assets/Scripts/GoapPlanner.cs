using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner
{
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{

	}
	private class Node
	{
		public HashSet<KeyValuePair<string, object>> state;
		public GoapAction action;
		public Node parent;
		public float nodeCost;
	

		public Node( HashSet<KeyValuePair<string, object>> s, GoapAction a, Node p, float c)
		{
			this.state = s;
			this.action = a;
			this.parent = p;
			this.nodeCost = c;
		}
	}

	public Queue<GoapAction> getPlan(GameObject squi, HashSet<KeyValuePair<string, object>> currWorldState,HashSet<KeyValuePair<string, object>> goal, HashSet<GoapAction> allSquiActions)
	{
		Queue<GoapAction> actionPlan = new Queue<GoapAction>();
		List<Node> GoalNodes = new List<Node>();
		Node start = new Node(currWorldState, null, null, 0);
		HashSet<GoapAction> currActions = new HashSet<GoapAction>();
	
		//Get actions currently can perform
		foreach (GoapAction act in allSquiActions)
		{
			act.inRange = false;
	        act.finished = false;
	        act.hasSetTarget = false;
			if (act.checkCondition(squi))
            {
				currActions.Add(act);
			}				
		}
						
		bool found = generateTree(start, GoalNodes, currActions, goal);

		//No available plan
		if (!found)
		{
			return null;
        }
        else
        {
			//Get the goal state node with lowest total cost
			Node leastCostGoalNode = GoalNodes[0];
			foreach (Node node in GoalNodes)
			{
				if (node.nodeCost < leastCostGoalNode.nodeCost)
				{
					leastCostGoalNode = node;
				}
			}
			//Get the corresponding action list
			List<GoapAction> temp = new List<GoapAction>();
			Node curr = leastCostGoalNode;
			while (curr != null)
			{
				if (curr.action != null)
				{
					temp.Insert(0, curr.action);
				}
				curr = curr.parent;
			}
			foreach (GoapAction a in temp)
			{
				actionPlan.Enqueue(a);
			}

			return actionPlan;
		}
	
	}

	private bool CheckState(HashSet<KeyValuePair<string, object>> goal, HashSet<KeyValuePair<string, object>> curr)
	{

		foreach (KeyValuePair<string, object> g in goal)
		{
			bool found = false;
			foreach (KeyValuePair<string, object> c in curr)
			{
				if (c.Equals(g))
				{
					found = true;
				}
			}
			if (!found)
			{
				return false;
			}
		}
		return true;
	}
	private bool generateTree(Node node, List<Node> goalNodes, HashSet<GoapAction> actions, HashSet<KeyValuePair<string, object>> goal)
	{

		bool result = false;
		foreach (GoapAction act in actions)
		{			
			if (CheckState(act.preconditions, node.state))
			{

				//Apply the action to get a new state

				HashSet<KeyValuePair<string, object>> newState = new HashSet<KeyValuePair<string, object>>();
				foreach (KeyValuePair<string, object> c in node.state)
				{
					newState.Add(new KeyValuePair<string, object>(c.Key, c.Value));
				}
				foreach (KeyValuePair<string, object> ch in act.effects)
				{
					newState.RemoveWhere((KeyValuePair<string, object> k) => { return k.Key.Equals(ch.Key); });
					newState.Add(new KeyValuePair<string, object>(ch.Key, ch.Value));
				}

				// Struct a new node for the new state
				float newCost = node.nodeCost + act.cost;
				Node newNode = new Node(newState, act, node, newCost);

				// Check if goal is reached
				if (CheckState(goal, newState))
				{ 
					goalNodes.Add(newNode);
					result =  true;
				}
				else
				{					
					HashSet<GoapAction> newSet = new HashSet<GoapAction>();
					foreach (GoapAction g in actions)
					{
						if (!g.Equals(act))
						{
							newSet.Add(g);
						}
					}
					bool found = generateTree(newNode, goalNodes, newSet, goal);
					if (found)
                    {
						result = true;
                    }						
				}
			}
		}
		return result;
	}


}
