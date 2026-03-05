using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Player player;
    public LayerMask playerMask, groundMask, wallMask;
    public float sightRange;
    public bool canSeePlayer;
    public Health health;
    Transform startLocation;
    //Rigidbody rb;
    public float jumpPower = 10f;
    Animator animator;
    [SerializeField] int damage = 2;
    [SerializeField] float attackRange = 5f;
    bool onCoolDown = false;
    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        startLocation = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        canSeePlayer = Physics.CheckSphere(transform.position, sightRange, playerMask);
        if (canSeePlayer)
        {
            navMeshAgent.isStopped = false;
            if (animator) animator.SetBool("CanSeePlayer", true);
            
            chasePlayer();
        }
        else
        {
            navMeshAgent.SetDestination(startLocation.position);
            if (animator) animator.SetBool("CanSeePlayer", false);
            //bool nearbyAlly = false;
            //MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>();
            //foreach (var enemy in enemies)
            //{
            //    if (enemy.canSeePlayer) nearbyAlly = true;
            //}
            //if (!nearbyAlly)player.seen = false;
        }
    }
    void chasePlayer()
    {
        //player.seen = true;
        NavMeshHit hit;
        Vector3 target = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
        NavMesh.Raycast(transform.position, target, out hit, groundMask);
        if (!Physics.CheckSphere(transform.position, 1f, playerMask) && !NavMesh.Raycast(transform.position, target, out hit, groundMask)/*Physics.Raycast(transform.position, player.transform.position, Mathf.Infinity, groundMask)*/) navMeshAgent.SetDestination(player.gameObject.transform.position);
        else if (hit.mask == NavMesh.GetAreaFromName("Jump"))
        {
            //rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            if (animator) animator.SetTrigger("Jump");
            navMeshAgent.SetDestination(player.gameObject.transform.position);
        }
        else
        {
            navMeshAgent.isStopped = true;
            if (animator) animator.SetBool("CanSeePlayer", false);
            TryAttackPlayer();
        }
    }
    void TryAttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= attackRange && !onCoolDown) StartCoroutine(Attack(distance));
    }
    void DamagePlayer()
    {
        player.UpdateHealth(damage);
        StartCoroutine(CoolDown());
        onCoolDown = true;
    }
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(5);
        onCoolDown = false;
    }
    IEnumerator Attack(float distance)
    {
        yield return new WaitForSeconds(0.5f);
        if (distance <= attackRange && !onCoolDown) DamagePlayer();
    }
    //private void OnDestroy()
    //{
    //    bool nearbyAlly = false;
    //    MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>();
    //    foreach (var enemy in enemies)
    //    {
    //        if (enemy.canSeePlayer) nearbyAlly = true;
    //    }
    //    if (player.seen && !nearbyAlly) player.seen = false;
    //}
}
