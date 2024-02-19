using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI agentInfoText;
    [SerializeField] private TextMeshProUGUI generatedText;

    [SerializeField] private Button generateTextButton;
    private void Awake()
    {
        AgentController.UpdateAgentStatus += UpdateCanvas;
        
        generateTextButton.onClick.AddListener(GenerateText);
    }

    private void OnDestroy()
    {
        AgentController.UpdateAgentStatus -= UpdateCanvas;
    }

    private void UpdateCanvas(AgentController agentController)
    {
        string agentInfo = $"{agentController.Name}<br>HP: {agentController.Health}";
        agentInfoText.text = agentController.Selected ? agentInfo : "";
    }

    private void GenerateText()
    {
        int randomNumber = Random.Range(1, 101);

        string firstPart = randomNumber % 3 == 0 ? "Marko" : "";
        string secondPart = randomNumber % 5 == 0 ? "Polo" : "";

        generatedText.text = $"{randomNumber}<br>{firstPart + secondPart}";
    }
    
}
