using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI agentHealthText;
    void Awake()
    {
        AgentController.UpdateAgentStatus += UpdateCanvas;
    }

    private void OnDestroy()
    {
        AgentController.UpdateAgentStatus -= UpdateCanvas;
    }

    void UpdateCanvas(AgentController agentController)
    {
        agentHealthText.text = agentController.Selected ? $"HP: {agentController.Health}" : "";
    }
}
