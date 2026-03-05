using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Titleplayer : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    [SerializeField] Vector3 target;
    void Start()
    {
        Time.timeScale = 1f;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.SetDestination(target);
        animator.SetBool("Moving", true);
    }
    private void Update()
    {
        if (transform.position == target)
        {
            agent.isStopped = true;
            animator.SetBool("Moving", false);
        }
    }
}
