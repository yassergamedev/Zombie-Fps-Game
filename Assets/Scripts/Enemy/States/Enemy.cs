using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }
    //just for debugging purposes
    [SerializeField]
    private string currentState;
    private GameObject player;
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;
    public float Health = 100f;
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();
    }
    public bool CanSeePlayer()
    {
        if (player != null)
        {
            //is the player close enough to be seen
            if (Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    //check if the enemy line of sight is blocked by an object
                    Ray ray = new(transform.position + (Vector3.up * eyeHeight), targetDirection);
                   
                    RaycastHit hitInfo = new RaycastHit();
                    
                    if (Physics.Raycast(ray, out hitInfo, sightDistance)) {
                       
                        if(hitInfo.transform.gameObject == player)
                        {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                            Debug.Log("Found Player");
                            return true;
                        }
                    }

                }
            
            }
        }
        return false;
    }
}
