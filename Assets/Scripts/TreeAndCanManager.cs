using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeAndCanManager : MonoBehaviour
{
    public Text t1;
    public Text t2;
    public Text t3;
    public Text t4;
    public Text t5;

    public Text t6;
    public bool ghostMode;

    public GameObject tree;
    public GameObject can;
    public GameObject squirrel;
    public GameObject sign;
    public GameObject nut;

    int layerMask = 1 << 3;
    private int[,] panel = new int[10, 10];
    private float[] posX = new float[10];
    private float[] posY = new float[10];
    public List<GameObject> nutsList = new List<GameObject>();
    public List<GameObject> canList = new List<GameObject>();
    public List<GameObject> treeList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        GenerateTreesAndCans();
        ghostMode = false;
        updateModeText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            switchMode();
        }
        if (ghostMode)
        {
            checkMouseClick();
        }
        
    }

    void updateModeText()
    {
        string s = "Mode:";
        if (ghostMode)
        {
            s = s + "Ghost";
        }
        else
        {
            s = s + "Normal";
        }
        t6.text = s;
    }

    void switchMode()
    {
        if (ghostMode)
        {
            ghostMode = false;
        }
        else
        {
            ghostMode = true;
        }
        updateModeText();
    }

    void checkMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, -1.25F);
            float distance;
            Vector3 worldPosition;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                if (hit.transform != null)
                {
                    GameObject obj = hit.transform.gameObject;                  
                    obj.GetComponent<Renderer>().material.color = Color.red;
                    if (nutsList.Contains(obj))
                    {
                        Debug.Log("Destroyed Nut.");
                        nutsList.Remove(obj);
                        Destroy(obj);
                    }
                    else 
                    {
                        GameObject par = obj.transform.parent.gameObject;                       
                        if (canList.Contains(par))
                        {
                            par.GetComponent<CanController>().changeState();
                            Debug.Log("Switched Can Mode.");
                        }
                        else if (canList.Contains(par.transform.parent.gameObject))
                        {
                            par.transform.parent.gameObject.GetComponent<CanController>().changeState();
                            Debug.Log("Switched Can Mode.");
                        }                     
                    }
                }
             
            }
            else if (plane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);
                var o = Instantiate(nut, worldPosition, Quaternion.identity) as GameObject;
                nutsList.Add(o);
                Debug.Log("New nut created.");
            }
           
        }
    }
    public void addToNuts(GameObject o)
    {
        nutsList.Add(o);
    }
    public void removeFromNuts(GameObject o)
    {
        nutsList.Remove(o);
    }
    public void GenerateTreesAndCans()
    {
        int[] aaa = new int[5];
        int[] bbb = new int[5];

        float x1 = -45;
        for (int i = 0; i < 10; i++)
        {
            posX[i] = x1;
            x1 += 10;
        }
        float y1 = -45;
        for (int i = 0; i < 10; i++)
        {
            posY[i] = y1;
            y1 += 10;
        }
        // Instantiate trees
        int number = 0;
        while (number < 5)
        {
            int xx = Random.Range(0, 10);
            int yy = Random.Range(0, 10);
            if (panel[xx, yy] != 1)
            {
                aaa[number] = yy;
                bbb[number] = xx;
                number++;
                panel[xx, yy] = 1;     
            }
        }

        // 5 tree and squirrel pairs
        // A colored cube is generated for each pair to distinguish

        var t = Instantiate(tree, new Vector3(posY[aaa[0]], 0, posX[bbb[0]]), Quaternion.identity) as GameObject;
        treeList.Add(t);
        var o = Instantiate(squirrel, new Vector3(posY[aaa[0]] + 2, 0.82f, posX[bbb[0]] + 2), Quaternion.identity) as GameObject;
        o.GetComponent<SquirrelController>().homeTree = t;
        o.GetComponent<SquirrelController>().text1 = t1;
        var s1 = Instantiate(sign, new Vector3(posY[aaa[0]]+2, 2.5f, posX[bbb[0]]+2.5f), Quaternion.identity) as GameObject;
        s1.transform.parent = o.transform;
        var s2 = Instantiate(sign, new Vector3(posY[aaa[0]], 15f, posX[bbb[0]]), Quaternion.identity) as GameObject;
        s1.GetComponent<Renderer>().material.color = Color.red;
        s2.GetComponent<Renderer>().material.color = Color.red;

        /*
        t = Instantiate(tree, new Vector3(posY[aaa[1]], 0, posX[bbb[1]]), Quaternion.identity) as GameObject;
        t = Instantiate(tree, new Vector3(posY[aaa[2]], 0, posX[bbb[2]]), Quaternion.identity) as GameObject;
        t = Instantiate(tree, new Vector3(posY[aaa[3]], 0, posX[bbb[3]]), Quaternion.identity) as GameObject;
        t = Instantiate(tree, new Vector3(posY[aaa[4]], 0, posX[bbb[4]]), Quaternion.identity) as GameObject;
        */
        
        t = Instantiate(tree, new Vector3(posY[aaa[1]], 0, posX[bbb[1]]), Quaternion.identity) as GameObject;
        treeList.Add(t);
        o = Instantiate(squirrel, new Vector3(posY[aaa[1]] + 2, 0.82f, posX[bbb[1]] + 2), Quaternion.identity) as GameObject;
        o.GetComponent<SquirrelController>().homeTree = t;
        o.GetComponent<SquirrelController>().text1 = t2;
        s1 = Instantiate(sign, new Vector3(posY[aaa[1]] + 2, 2.5f, posX[bbb[1]] + 2.5f), Quaternion.identity) as GameObject;
        s1.transform.parent = o.transform;
        s2 = Instantiate(sign, new Vector3(posY[aaa[1]], 15f, posX[bbb[1]]), Quaternion.identity) as GameObject;
        s1.GetComponent<Renderer>().material.color = Color.blue;
        s2.GetComponent<Renderer>().material.color = Color.blue;
    

        t = Instantiate(tree, new Vector3(posY[aaa[2]], 0, posX[bbb[2]]), Quaternion.identity) as GameObject;
        treeList.Add(t);
        o = Instantiate(squirrel, new Vector3(posY[aaa[2]] + 2, 0.82f, posX[bbb[2]] + 2), Quaternion.identity) as GameObject;
        o.GetComponent<SquirrelController>().homeTree = t;
        o.GetComponent<SquirrelController>().text1 = t3;
        s1 = Instantiate(sign, new Vector3(posY[aaa[2]] + 2, 2.5f, posX[bbb[2]] + 2.5f), Quaternion.identity) as GameObject;
        s1.transform.parent = o.transform;
        s2 = Instantiate(sign, new Vector3(posY[aaa[2]], 15f, posX[bbb[2]]), Quaternion.identity) as GameObject;
        s1.GetComponent<Renderer>().material.color = Color.yellow;
        s2.GetComponent<Renderer>().material.color = Color.yellow;
      

        t = Instantiate(tree, new Vector3(posY[aaa[3]], 0, posX[bbb[3]]), Quaternion.identity) as GameObject;
        treeList.Add(t);
        o = Instantiate(squirrel, new Vector3(posY[aaa[3]] + 2, 0.82f, posX[bbb[3]] + 2), Quaternion.identity) as GameObject;
        o.GetComponent<SquirrelController>().homeTree = t;
        o.GetComponent<SquirrelController>().text1 = t4;
        s1 = Instantiate(sign, new Vector3(posY[aaa[3]] + 2, 2.5f, posX[bbb[3]] + 2.5f), Quaternion.identity) as GameObject;
        s1.transform.parent = o.transform;
        s2 = Instantiate(sign, new Vector3(posY[aaa[3]], 15f, posX[bbb[3]]), Quaternion.identity) as GameObject;
        s1.GetComponent<Renderer>().material.color = Color.white;
        s2.GetComponent<Renderer>().material.color = Color.white;



        t = Instantiate(tree, new Vector3(posY[aaa[4]], 0, posX[bbb[4]]), Quaternion.identity) as GameObject;
        treeList.Add(t);
        o = Instantiate(squirrel, new Vector3(posY[aaa[4]] + 2, 0.82f, posX[bbb[4]] + 2), Quaternion.identity) as GameObject;
        o.GetComponent<SquirrelController>().homeTree = t;
        o.GetComponent<SquirrelController>().text1 = t5;
        s1 = Instantiate(sign, new Vector3(posY[aaa[4]] + 2, 2.5f, posX[bbb[4]] + 2.5f), Quaternion.identity) as GameObject;
        s1.transform.parent = o.transform;
        s2 = Instantiate(sign, new Vector3(posY[aaa[4]], 15f, posX[bbb[4]]), Quaternion.identity) as GameObject;
        s1.GetComponent<Renderer>().material.color = Color.black;
        s2.GetComponent<Renderer>().material.color = Color.black;
        


        //Other trees
        while (number < 10)
        {
            int xx = Random.Range(0, 10);
            int yy = Random.Range(0, 10);

            if (panel[xx, yy] != 1)
            {
                t = Instantiate(tree, new Vector3(posY[yy], 0, posX[xx]), Quaternion.identity) as GameObject;
                treeList.Add(t);
                number++;
                panel[xx, yy] = 1;              
            }

        }
        // Instantiate cans
        number = 0;
        while (number < 5)
        {
            int xx = Random.Range(0, 10);
            int yy = Random.Range(0, 10);

            if (panel[xx, yy] != 1)
            {
                var c = Instantiate(can, new Vector3(posY[yy], 1.5f, posX[xx]), Quaternion.identity) as GameObject;
                number++;
                panel[xx, yy] = 1;
                canList.Add(c);
            }

        }

    }


}
