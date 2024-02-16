using UnityEngine;
using System;

public class AgentController: MonoBehaviour
{
    public static Action<AgentController> UpdateAgentStatus;
    [field: SerializeField] public int Health { get; private set; } = 3;
    [SerializeField] private GameObject outline;//test
    [SerializeField] private NameData names;
    public bool Selected { get; private set; }
    public string Name { get; private set; }

    private void OnEnable()
    {
        UpdateAgentStatus += DeselectAgent;
        Name = names.GenerateName();
    }

    private void OnDisable()
    {
        UpdateAgentStatus -= DeselectAgent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        Health--;
        if (Health <= 0) Die();
        UpdateAgentStatus?.Invoke(this);
    }
    
    private void Die()
    {
        Selected = false;
        GameManager.AgentDied?.Invoke(this);
    }

    public void OnAgentClick()
    {
        Selected = !Selected;
        outline.SetActive(Selected);
        UpdateAgentStatus?.Invoke(this);
    }

    private void DeselectAgent(AgentController agentController)
    {
        if (agentController != this)
        {
            Selected = false;
            outline.SetActive(false);
        }
    }
}