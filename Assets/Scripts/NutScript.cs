using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutScript : MonoBehaviour
{
    private bool picked;
    
    // Start is called before the first frame update
    void Start()
    {
        picked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nowPick()
    {
        picked = true;
        Destroy(GetComponent<Rigidbody>());
    }

    public bool Picked()
    {
        return picked;
    }
}
