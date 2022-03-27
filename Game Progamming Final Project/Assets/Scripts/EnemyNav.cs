using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    //private Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        //anim = GetComponent<Animator>();
        //anim.SetInteger("AnimState", 1);
    }

    void Update()
    {
        agent.SetDestination(player.position);
    }
}
