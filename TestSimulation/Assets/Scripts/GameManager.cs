using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Variables
    
    public static Action<AgentController> AgentDied;
    [SerializeField] private AgentController agentPrefab;

    [SerializeField, Min(0)] private float minSpawnDistanceToAgent = 2.5f;
    
    [Header("Agents Count")]
    [SerializeField, Min(1)] private int minAgentsCountAtStart = 3;
    [SerializeField, Min(1)] private int maxAgentsCountAtStart = 5;
    [SerializeField, Min(1)] private int maxAgentsCount = 30;
    
    [Header("Spawn Duration")]
    [SerializeField] private float minSpawnDuration = 2;
    [SerializeField] private float maxSpawnDuration = 6;

    [Header("Spawn Points")]
    [SerializeField] private Transform topLeftSpawnPoint;
    [SerializeField] private Transform bottomRightSpawnPoint;
    
    private readonly List<GameObject> _activeAgents = new List<GameObject>();
    private GameObjectPool _agentPool;

    #endregion

    #region MonoBehaviour methods

    private void Awake()
    {
        //Subscriptions
        AgentDied += ReturnAgentToPool;
        
        //pool initialization
        _agentPool = new GameObjectPool(agentPrefab.gameObject, 30);

        //spawn first agents
        int agentsCountAtStart = Random.Range(minAgentsCountAtStart, maxAgentsCountAtStart + 1);
        for (int i = 0; i < agentsCountAtStart; i++)
        {
            AgentSpawn();
        }
        
        //start spawner
        StartCoroutine(AgentSpawner());
    }

    private void OnDestroy()
    {
        //Unsubscribes
        AgentDied -= ReturnAgentToPool;
    }

    private void OnValidate()
    {
        if (minAgentsCountAtStart > maxAgentsCountAtStart)
        {
            maxAgentsCountAtStart = minAgentsCountAtStart;
        }
    }

    #endregion

    #region Private methods

    private void ReturnAgentToPool(AgentController agentController)
    {
        //return died agents to object pool
        GameObject agent = agentController.gameObject;
        _activeAgents.Remove(agent);
        _agentPool.Return(agent);
        
        //start spawner
        if (_activeAgents.Count >= maxAgentsCount - 1)
        {
            StartCoroutine(AgentSpawner());
        }
    }

    private void AgentSpawn()
    {
        GameObject newAgent = _agentPool.Get();
        newAgent.transform.position = CalculateAgentPos();
        _activeAgents.Add(newAgent);
    }

    private Vector3 CalculateAgentPos()
    {
        //search for free space on the plane in the given range
        Vector3 newPos = new Vector3();
        bool posCalculated = false;
        
        while (!posCalculated)
        {
            Vector3 topLeftPos = topLeftSpawnPoint.position;
            Vector3 bottomRightPos = bottomRightSpawnPoint.position;
        
            newPos.x = Random.Range(topLeftPos.x, bottomRightPos.x);
            newPos.y = 1.5f;
            newPos.z = Random.Range(bottomRightPos.z, topLeftPos.z);

            posCalculated = true;
            
            foreach (GameObject agent in _activeAgents)
            {
                float distanceToAgent = Vector3.Distance(agent.transform.position, newPos);
                if (distanceToAgent < minSpawnDistanceToAgent)
                {
                    posCalculated = false;
                    break;
                }
            }
        }
        return newPos;
    }

    #endregion

    #region Coroutines

    private IEnumerator AgentSpawner()
    {
        while (_activeAgents.Count < maxAgentsCount)
        {
            float randomDuration = Random.Range(minSpawnDuration, maxSpawnDuration);
            yield return new WaitForSeconds(randomDuration);
            AgentSpawn();
        }
    }

    #endregion
}
