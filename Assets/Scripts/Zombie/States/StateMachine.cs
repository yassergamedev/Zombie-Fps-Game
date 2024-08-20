using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    // property for patrolState

    public void Initialize()
    {
        // setup default state
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeState != null) {
            activeState.Perform();
        }
    }
    public void changeState(BaseState newState)
    {
        //check if activeState != null
        if (activeState != null) {
            //clean up the state
            activeState.Exit();
        }
        //change into a new State
        activeState = newState;

        //fall safe null check to make sure the state wasn't null
        if (activeState != null) {
            // setup new state
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}
