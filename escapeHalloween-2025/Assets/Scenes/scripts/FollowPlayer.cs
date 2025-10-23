using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;

public class FollowPlayer : MonoBehaviour
{
    public NavMeshAgent nMA;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;
    public float viewAngle = 90;
    public float viewRadius = 15;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIteration = 4;
    public float edgeDistance = .5f;
    public Transform[] wayPoints;
    int m_currentWaypointIndex;

    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_playerPosition;

    float m_waitTime;
    float m_timeToRotate;
    bool m_playerInRange;
    bool m_playerNear;
    bool m_isPatrol;
    bool m_caughtPlayer;
    // Start is called before the first frame update
    void Start()
    {
        m_playerPosition = Vector3.zero;
        m_isPatrol = true;
        m_caughtPlayer = false;
        m_playerInRange = false;
        m_waitTime = startWaitTime;
        m_timeToRotate = timeToRotate;

        m_currentWaypointIndex = 0;
        nMA = GetComponent<NavMeshAgent>();

        nMA.isStopped = false;
        nMA.speed = speedWalk;
        nMA.SetDestination(wayPoints[m_currentWaypointIndex].position);

    }

    // Update is called once per frame
    void Update()
    {
        EnvironmentView();

        if (!m_isPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }

    private void Chasing()
    {
        m_playerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_caughtPlayer)
        {
            Move(speedRun);
            nMA.SetDestination(m_playerPosition);

        }
        if (nMA.remainingDistance <= nMA.stoppingDistance)
        {
            if (m_waitTime <= 0 && !m_caughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                m_isPatrol = true;
                m_playerNear = false;
                Move(speedWalk);
                m_timeToRotate = timeToRotate;
                m_waitTime = startWaitTime;
                nMA.SetDestination(wayPoints[m_currentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    m_waitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Patroling()
    {
        if (m_playerNear)
        {
            if (m_timeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                m_timeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_playerNear = false;
            playerLastPosition = Vector3.zero;
            nMA.SetDestination(wayPoints[m_currentWaypointIndex].position);
            if (nMA.remainingDistance <= nMA.stoppingDistance)
            {
                if (m_waitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_waitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_waitTime -= Time.deltaTime;
                }
            }
        }

    }
    void Move(float speed)
    {
        nMA.isStopped = false;
        nMA.speed = speed;
    }

    void Stop()
    {
        nMA.isStopped = true;
        nMA.speed = 0;
    }

    public void NextPoint()
    {
        m_currentWaypointIndex = (m_currentWaypointIndex + 1) % wayPoints.Length;
        nMA.SetDestination(wayPoints[m_currentWaypointIndex].position);
    }

    void CaughtPlayer()
    {
        m_caughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        nMA.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= .3)
        {
            if (m_waitTime <= 0)
            {
                m_playerNear = false;
                Move(speedWalk);
                nMA.SetDestination(wayPoints[m_currentWaypointIndex].position);
                m_waitTime = startWaitTime;
                m_timeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_waitTime -= Time.deltaTime;
            }
        }
    }

    void EnvironmentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float distToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleMask))
                {
                    m_playerInRange = true;
                    m_isPatrol = false;
                }
                else
                {
                    m_playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                m_playerInRange = false;
            }
            if (m_playerInRange)
            {
                m_playerPosition = player.transform.position;
            }
        }
    }
}
