using UnityEngine;

public class AgentController: MonoBehaviour
{
    [ContextMenu("Die")]
    private void Die()
    {
        GameManager.AgentDied?.Invoke(this);
    }
}