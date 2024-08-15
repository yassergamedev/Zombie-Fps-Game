using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this is called template method design pattern

public abstract class  Interactable : MonoBehaviour
{
    //Add or remove an interactionEvent component to this gameObject
    public bool useEvents;
    //message displayed to the player for interaction
    public string promptMessage;

    public void BaseInteract()
    {
        if (useEvents)
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        Interact();
    }
    protected virtual void Interact()
    {
        //no code written in this function
        // this is just a template function to be overridden by subclasses
    }


    public virtual string OnLook()
    {
        return promptMessage;   
    }
}
