using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Security.Cryptography;

public class EnemyAI : MonoBehaviour
{
   
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] AudioClip walkingSound;
    

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    AudioSource audioSource;
    bool isProvoked = false;

    Animator animator;

    EnemyHealth health;
    Transform target;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
       ;
    }


    void Update()
    {
        if (health.IsDead())
        {
            enabled = false;
            navMeshAgent.enabled = false;
        }


        if (target == null)
        {
            FindClosestTarget();
            return;
        }


        distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (isProvoked)
        {
            EngageTarget();
        }
        else if (distanceToTarget <= chaseRange)
        {
            isProvoked = true;
            //You can add alarm sound or any other ways to provoke the enemy here
  
        }

    }


    void FindClosestTarget()
    {
        PlayerHealth[] allPlayers = FindObjectsOfType<PlayerHealth>();
        if (allPlayers.Length == 0)
        {
            return;
        }

        PlayerHealth closestPlayer = null;

        float shortestDistance = Mathf.Infinity;

        foreach(PlayerHealth player in allPlayers)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            target = closestPlayer.transform;
        }
    }



    public void OnDamageTaken()
    {
        isProvoked = true;

    }

    void EngageTarget()
    {
        FaceTarget();
       
        if (distanceToTarget >= navMeshAgent.stoppingDistance) 
        {
            ChaseTarget();

        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance) 
        {
            AttackTarget();
        }
    }

    void AttackTarget()
    {
        animator.SetBool("Attack", true);
       

    }

    void ChaseTarget()
    {
        animator.SetBool("Attack", false);
        animator.SetTrigger("Move");
        
        

        navMeshAgent.SetDestination(target.position);
    }
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    

}
