using System;
using System.Collections;
using ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static Action<AgentController> AgentDied;
    [SerializeField] private AgentController agentPrefab;
    
    [Header("Agents Count")]
    [SerializeField] private int agentsCountAtStart = 3;
    [SerializeField] private int maxAgentsCount = 30;
    
    [Header("SpawnDuration")]
    [SerializeField] private float minSpawnDuration = 2;
    [SerializeField] private float maxSpawnDuration = 6;
    
    private GameObjectPool _agentPool;
    private int _agentsCount;//test

    void Awake()
    {
        _agentPool = new GameObjectPool(agentPrefab.gameObject, 30);
        AgentDied += ReturnAgentToPool;

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
        _agentPool.Return(agentController.gameObject);
        _agentsCount--;

        if (_agentsCount >= maxAgentsCount - 1)
        {
            Debug.Log("Start spawner (return)");
            StartCoroutine(AgentSpawner());
        }
    }

    private void AgentSpawn()
    {
        _agentsCount++;
        GameObject newAgent = _agentPool.Get();
        //Set agent position
    }

    private IEnumerator AgentSpawner()
    {
        while (_agentsCount < maxAgentsCount)
        {
            float randomDuration = Random.Range(minSpawnDuration, maxSpawnDuration);
            yield return new WaitForSeconds(randomDuration);
            AgentSpawn();
        }
        Debug.Log("Stop spawner");
    }
}
