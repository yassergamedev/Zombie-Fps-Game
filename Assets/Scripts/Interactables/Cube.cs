using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this function is where we will design our interaction
    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
    public void test()
    {
        Debug.Log("test");
    }

}
