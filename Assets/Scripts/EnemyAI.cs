using System;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// 1. Change to MonoBehaviourPun for network safety
public class EnemyAI : MonoBehaviourPun
{
    [Header("Movement Stats")]
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;

    [Header("Attack Stats")]
    [SerializeField] float damage = 40f;
    [SerializeField] AudioClip provokedSound;
    [SerializeField] AudioClip idleSound;

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    AudioSource audioSource;
    Animator animator;
    bool isProvoked = false;

    EnemyHealth health;
    Transform target;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        // Fix NavMesh spawning issues
        if (navMeshAgent.isOnNavMesh == false)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position);
            }
        }
    }

    void Update()
    {
        // 2. CRITICAL MULTIPLAYER CHECK
        // If I am NOT the host, I should not run AI logic. 
        // I should just listen to the PhotonTransformView.
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (health.IsDead())
        {
            enabled = false;
            navMeshAgent.enabled = false;
            return;
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
        }
    }

    void FindClosestTarget()
    {
        PlayerHealth[] allPlayers = FindObjectsOfType<PlayerHealth>();
        if (allPlayers.Length == 0) return;

        PlayerHealth closestPlayer = null;
        float shortestDistance = Mathf.Infinity;

        foreach (PlayerHealth player in allPlayers)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
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

    // --- MERGED FROM ENEMYATTACK.CS ---

    // 1. Triggered by Animation Event
    public void AttackHitEvent()
    {
        if (target == null) return;

        // 2. Only the Master Client (who runs the AI) sends the damage command
        if (!PhotonNetwork.IsMasterClient) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // 3. Distance check prevents getting hit from far away if you sprinted back
        // We add a small buffer (+ 1.5f) to ensure the hit registers if the animation plays
        if (distance <= navMeshAgent.stoppingDistance + 1.5f)
        {
            PhotonView targetPV = target.GetComponent<PhotonView>();
            if (targetPV != null)
            {
                // 4. Send Damage to the player over the network
                targetPV.RPC("TakeDamage", RpcTarget.All, damage);

                // Play provoked sound
                if (audioSource != null && provokedSound != null)
                    audioSource.PlayOneShot(provokedSound, 0.2f);
            }
        }
    }

    // 2. Triggered by Animation Event
    public void ZombieIdleGrowl()
    {
        if (audioSource != null && idleSound != null)
            audioSource.PlayOneShot(idleSound, 0.5f);
    }

    // ----------------------------------------

    void ChaseTarget()
    {
        animator.SetBool("Attack", false);
        animator.SetTrigger("Move");

        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(target.position);
        }
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