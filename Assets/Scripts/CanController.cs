using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanController : MonoBehaviour
{

    float last_switch;
    public bool Full;
    public GameObject fullCan;
    public GameObject emptyCan;
    public GameObject garbage;

    GameObject curr;
    // Start is called before the first frame update
    void Start()
    {
        int xx = Random.Range(0, 10);
        if (xx < 5)
        {
            Full = true;
            curr = Instantiate(fullCan, transform.position, Quaternion.identity) as GameObject;
            curr.transform.parent = transform;
        }
        else
        {
            Full = false;
            curr = Instantiate(emptyCan, transform.position, Quaternion.identity) as GameObject;
            curr.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - last_switch>= 10)
        {
            changeState();
        }
    }

    public void changeState()
    {
        if (Full)
        {
            Destroy(curr);
            Full = false;
            curr = Instantiate(emptyCan, transform.position, Quaternion.identity) as GameObject;
            curr.transform.parent = transform;
        }
        else
        {
            Destroy(curr);
            Full = true;
            curr = Instantiate(fullCan, transform.position, Quaternion.identity) as GameObject;
            curr.transform.parent = transform;
        }
        last_switch = Time.time;
    }
}
