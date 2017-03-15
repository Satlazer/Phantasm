﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PhantomSpawnLocation : MonoBehaviour
{

    public float maxSight = 50; //how far the ai can see, for raycasting line of sight
    public float patience = 3; //how long the ai will wait at the lastknown position before returning back to 
    public float arrivalDistance = 0.1f; //accuracy for arrival points, if within x units it considers it as arrived
    public bool chargeAttack = false; //wether the ai can do a pounce like attack or melee
    public bool alternatesVisibility = false; //wether the ai alternates between agent vision to hacker vision and back
    public Patrol patrolPath; //path the ai will idly patrol around
    public BehaviourTree.AI_TYPE type = BehaviourTree.AI_TYPE.Wallhack; //the class of ai
    public bool triggered = true; //for the inactive/whatever bonnyman/jacob wanted

    public BehaviourTree.AISettings aiSettings()
    {
        BehaviourTree.AISettings s;
        s.maxSight = maxSight;
        s.patience = patience;
        s.arrivalDistance = arrivalDistance;
        s.chargeAttack = chargeAttack;
        s.alternatesVisibility = alternatesVisibility;
        s.patrolPath = patrolPath;
        s.type = type;
        s.triggered = triggered;

        return s;
    }

    // Implement this OnDrawGizmo if you want to draw gizmos for the object
    public void OnDrawGizmosSelected()
    {

    }

    public void Start()
    {
        GameObject Phantom = Instantiate(Resources.Load("Phantom"), transform.position, transform.rotation) as GameObject;
        BehaviourTree bt = Phantom.GetComponent<BehaviourTree>();
        bt.UpdateSettings(aiSettings());
        NetworkServer.Spawn(Phantom);
    }


}
