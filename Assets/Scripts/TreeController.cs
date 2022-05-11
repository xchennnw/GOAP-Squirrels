using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    
    public GameObject nut;

    private float[] posX = new float[5];
    private float[] posY = new float[5];
    private GameObject[] nuts = new GameObject[5];
    private int num;
    private GameObject treeManager;
    private TreeAndCanManager treeScript;

    // Start is called before the first frame update
    void Start()
    {
        setArrays();
        InvokeRepeating("generateOneNut", 0, 4);
        InvokeRepeating("checkNutsNumber", 0, 2);
        num = 0;
        treeManager = GameObject.Find("TreeCanManager");
        treeScript = treeManager.GetComponent<TreeAndCanManager>();
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    void generateOneNut()
    {
        while (num<5)
        {
            int xx = Random.Range(0, 5);
            if (nuts[xx] == null || nuts[xx].GetComponent<NutScript>().Picked())
            {
                var o = Instantiate(nut, new Vector3(posX[xx], 2f, posY[xx]), Quaternion.identity) as GameObject;
                treeScript.addToNuts(o);
                nuts[xx] = o;
                num++;
                return;
            }
        }
     
    }

    void checkNutsNumber()
    {
        num = 0;
        for(int i=0; i<5; i++)
        {
            var o = nuts[i];
            if (o != null && !o.GetComponent<NutScript>().Picked())
            {
               num++;
            }
        }
            
    }
    void setArrays()
    {
        float x = transform.position.x;
        float z = transform.position.z;

        posX[0] = x - 4f;
        posX[1] = x;
        posX[2] = x + 4f;
        posX[3] = x - 2.5f;
        posX[4] = x + 2.5f;
      
        posY[0] = z + 3f;
        posY[1] = z + 4.5f;
        posY[2] = z + 2.5f;
        posY[3] = z - 3.5f;
        posY[4] = z - 3.5f;

    }
}
