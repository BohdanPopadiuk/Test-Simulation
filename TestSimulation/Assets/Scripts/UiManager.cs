using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI agentInfoText;
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
        agentInfoText.text = agentController.Selected ? $"{agentController.Name}<br>HP: {agentController.Health}" : "";
    }
}
