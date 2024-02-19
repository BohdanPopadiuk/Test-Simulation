using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static Action<AgentController> AgentDied;
    [SerializeField] private AgentController agentPrefab;

    [SerializeField] private float minSpawnDistanceToAgent = 2.5f;
    
    [Header("Agents Count")]
    [SerializeField, Min(1)] private int minAgentsCountAtStart = 3;
    [SerializeField, Min(1)] private int maxAgentsCountAtStart = 5;
    [SerializeField, Min(1)] private int maxAgentsCount = 30;
    
    [Header("SpawnDuration")]
    [SerializeField] private float minSpawnDuration = 2;
    [SerializeField] private float maxSpawnDuration = 6;

    [Header("SpawnPoints")]
    [SerializeField] private Transform topLeftSpawnPoint;
    [SerializeField] private Transform bottomRightSpawnPoint;
    
    private GameObjectPool _agentPool;
    private readonly List<GameObject> _activeAgents = new List<GameObject>();

    void Awake()
    {
        _agentPool = new GameObjectPool(agentPrefab.gameObject, 30);
        AgentDied += ReturnAgentToPool;

        int agentsCountAtStart = Random.Range(minAgentsCountAtStart, maxAgentsCountAtStart + 1);
        for (int i = 0; i < agentsCountAtStart; i++)
        {
            AgentSpawn();
        }
        
        Debug.Log("Start spawner");
        StartCoroutine(AgentSpawner());
    }

    private void OnDestroy()
    {
        AgentDied -= ReturnAgentToPool;
    }

    private void ReturnAgentToPool(AgentController agentController)
    {
        GameObject agent = agentController.gameObject;
        _activeAgents.Remove(agent);
        _agentPool.Return(agent);
        
        if (_activeAgents.Count >= maxAgentsCount - 1)
        {
            Debug.Log("Start spawner (return)");
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

    private IEnumerator AgentSpawner()
    {
        while (_activeAgents.Count < maxAgentsCount)
        {
            float randomDuration = Random.Range(minSpawnDuration, maxSpawnDuration);
            yield return new WaitForSeconds(randomDuration);
            AgentSpawn();
        }
        Debug.Log("Stop spawner");
    }
}
