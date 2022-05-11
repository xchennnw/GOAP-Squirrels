using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SquirrelController : MonoBehaviour
{
    //Related GameObjects
    public Text text1;
    private TreeAndCanManager treeScript;
    NavMeshAgent agent;
    public GameObject homeTree;
    private GameObject player;

    //World state values
    bool closeToPlayer;
    bool hasGarbage;
    int numOfNuts;
    private List<GameObject> nuts = new List<GameObject>();
    private GameObject garbage;

    // Memories
    public List<GameObject> memoNuts = new List<GameObject>();
    public List<Vector3> nutPositions = new List<Vector3>();
    public GameObject currClosestTree;
    public List<GameObject> twoCans = new List<GameObject>();

    //States
    private bool ifGhostMode;
    private int idleState = 1;
    private int moveToState = 2;
    private int actionState = 3;
    private int currState;
    private bool stuck;
    private float stuckTime;
    private bool hiding;
    private float hideTime;
    private bool WaitingorRoaming;
 
    //GOAP Planner
    private GoapPlanner planner;
    private HashSet<GoapAction> squirrelActions = new HashSet<GoapAction>();
    private Queue<GoapAction> currPlan = new Queue<GoapAction>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerCapsule");
        treeScript = GameObject.Find("TreeCanManager").GetComponent<TreeAndCanManager>();
        ifGhostMode = treeScript.ghostMode;

        numOfNuts = 0;
        hasGarbage = false;
        closeToPlayer = false;

        agent = GetComponent<NavMeshAgent>();
        agent.destination = transform.position;

        planner = new GoapPlanner();
       
        currState = idleState;
        WaitingorRoaming = true;
        stuck = false;
        hiding = false;

        //Actions
        GoapAction[] actionComponents = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actionComponents)
        {
            squirrelActions.Add(a);
        }

        InvokeRepeating("getRoamingDest", 0, 6f);
    }

    // Update is called once per frame
    void Update()
    {
        //Check Ghost Mode
        ifGhostMode = treeScript.ghostMode;
        if (!ifGhostMode && !closeToPlayer)
        {
            detectPlayer();
        }
        // Behave according to current state
        if (stuck)
        {
            if (Time.time-stuckTime > 4f)
            {
                stuck = false;              
            }
        }
        else if (hiding)
        {
            Vector3 treeTop = new Vector3(currClosestTree.transform.position.x, currClosestTree.transform.position.y + 6f, currClosestTree.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, treeTop, 0.3f);
            if (Time.time - hideTime > 8f)
            {
                hiding = false;
                agent.updatePosition = true;
            }
        }
        else if(currState == idleState)
        {
            IdleState();           
        }
        else if(currState == moveToState)
        {
            MoveToState();
        }
        else if(currState == actionState)
        {
            ActionState();
        }
        updateText();
    }

    void updateText()
    {
        string s = "CurrState:";
        if (stuck)
        {
            s = s + "Stuck";
        }
        else if (hiding)
        {
            s = s + "Hiding";
        }
        else if (WaitingorRoaming)
        {
            s = s + "Wait/Roam";
        }
        else if(currState == 1)
        {
            s = s + "Idle";
        }
        else if (currState == 2)
        {
            s = s + "MovingTo";
        }
        else if (currState == 3)
        {
            s = s + "Action";
        }

        foreach (GoapAction act in currPlan)
        {
            s = s + "\n" + act.name;
        }
        text1.text = s;
    }

    //--------------------Set world states and Goal-----------------------------------
    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("has1Nut", (numOfNuts > 0)));
        worldData.Add(new KeyValuePair<string, object>("has2Nuts", (numOfNuts > 1)));
        worldData.Add(new KeyValuePair<string, object>("has3Nuts", (numOfNuts > 2)));
        worldData.Add(new KeyValuePair<string, object>("hasGarbage", hasGarbage));
        worldData.Add(new KeyValuePair<string, object>("playerTooClose", closeToPlayer));
        return worldData;
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>("playerTooClose", false));       
        if(currClosestTree == homeTree)
        {
            goal.Add(new KeyValuePair<string, object>("putFoodHome", true));
        }
        else if(nutPositions.Count>2 || twoCans.Count>0)
        {
            goal.Add(new KeyValuePair<string, object>("collectFood", true));
        }
       
        return goal;
    }

    //-------------------- Some behaviors for actions-----------------------------------
    public void addNut(GameObject n)
    {
        nuts.Add(n);
        numOfNuts++;
    }
    public void dropNuts()
    {
        foreach(GameObject n in nuts)
        {
            Destroy(n);
        }
        nuts.Clear();
        numOfNuts=0;
    }
    public void addGarbage(GameObject n)
    {
        garbage = n;
        hasGarbage = true;
    }

    public void getStuck()
    { 
        stuck = true;
        stuckTime = Time.time;
    }
    public void dropGarbage()
    {
        Destroy(garbage);
        garbage = null;
        hasGarbage = false;
    }
    void getRoamingDest()
    {
        if (WaitingorRoaming)
        {
            float a = Random.Range(-49f, 49f);
            float b = Random.Range(-49f, 49f);
            int r = Random.Range(0, 2);
            if (r == 0)
            {
                agent.destination = new Vector3(a, transform.position.y, b);
            }
            else
            {
                agent.destination = transform.position;
            }
           
        }
    }
    void detectPlayer()
    {
        if (!stuck && distance(gameObject, player) < 10f)
        {
            Debug.Log("Too close!");
            closeToPlayer = true;
            currState = idleState;
        }
    }

    public void goToHide()
    {      
        hiding = true;
        closeToPlayer = false;
        hideTime = Time.time;
        agent.updatePosition = false;
    }
//--------------------------------------Idle state-----------------------------------
    private void IdleState()
    {
        Observe();
        HashSet<KeyValuePair<string, object>> worldState = getWorldState();
        HashSet<KeyValuePair<string, object>> goal = createGoalState();
     
        // Plan
        Queue<GoapAction> plan = planner.getPlan(gameObject, worldState, goal, squirrelActions);
        if (plan != null)
        {
            WaitingorRoaming = false;
            currPlan = plan;          
            currState = actionState;
        }
        else
        {
            //Idle behaviors: roaming and waiting           
            WaitingorRoaming = true;
            agent.speed = 5f;
        }

    }
    //-----------------------------Move to target state-----------------------------------
    private void MoveToState()
    {
        
        GoapAction action = currPlan.Peek();
        agent.speed = 5f;
        float d;
        if(action.name == "Hide in a Tree")
        {
            d = 4f;
            agent.speed = 15f;
        }
        else if (action.name == "Put Nut Home" || action.name == "Put Garbage Home" ) 
        {
            d = 2f;
        }
        else if (action.name == "Pick Up Garbage")
        {
            d = 2.8f;
        }
        else
        {
            d = 1f;
        }
        float dx = transform.position.x - action.targetPos.x;
        float dz = transform.position.z - action.targetPos.z;
        float dist = Mathf.Sqrt(dx * dx + dz * dz);
       
        if (dist < d)
        {
            action.inRange =true;
            currState = actionState;
        }
    }

    //-----------------------------Run actions state-----------------------------------
    private void ActionState()
    {

        if (currPlan.Count <= 0)
        {
            currState = idleState;
            return;
        }
        GoapAction action = currPlan.Peek();
        if (action.finished)
        {
            currPlan.Dequeue();
        }
        if (currPlan.Count <= 0)
        {
            currState = idleState;
            return;
        }
        action = currPlan.Peek();

        if (!action.hasSetTarget)
        {
            action.setTarget(gameObject);
        }

        if (!action.inRange)
        {
            agent.destination = new Vector3(action.targetPos.x, agent.destination.y, action.targetPos.z);
            currState = moveToState;
        }
        else
        {
            bool success = action.perform(gameObject);
            if (!success)
            {
                currState = idleState;
            }
        }      
        
    }

    //-----------------------------Observe environment around----------------------------------
    private void Observe()
    {

        nutPositions.Clear();
        twoCans.Clear();

        //Find closest Tree
        if (distance(gameObject, homeTree) < 10)
        {
            currClosestTree = homeTree;    //Make it easier to find home
        }
        else
        {
            GameObject closest = treeScript.treeList[0];
            float currDist = distance(gameObject, closest);
            float newDist;
            foreach (GameObject tree in treeScript.treeList)
            {
                newDist = distance(gameObject, tree);
                if (newDist < currDist)
                {
                    currDist = newDist;
                    closest = tree;
                }
            }
            currClosestTree = closest;
        }     

        //Update observed nuts
        int n = 0;
        int count = 0;
        while(count<5 && n< treeScript.nutsList.Count)
        {
            if (distance(gameObject, treeScript.nutsList[n]) < 15)
            {
                memoNuts.Add(treeScript.nutsList[n]);
                nutPositions.Add(treeScript.nutsList[n].transform.position);
                count++;
            }
            n++;
        }

        //Update observed Cans
        n = 0;
        count = 0;
        while (count < 2 && n < treeScript.canList.Count)
        {
            if (distance(gameObject, treeScript.canList[n]) < 15)
            {
                twoCans.Add(treeScript.canList[n]);
                count++;
            }
            n++;
        }

    }

    private float distance(GameObject a, GameObject b)
    {
        float dx = a.transform.position.x - b.transform.position.x;
        float dz = a.transform.position.z - b.transform.position.z;
        float dist = Mathf.Sqrt(dx * dx + dz * dz);
        return dist;
    }







}