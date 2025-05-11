using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTeamManager : NetworkBehaviour
{
    public Button addButton;
    public Button removeButton;

    private void Start()
    {
        if (!isLocalPlayer)
            return;

        GameObject addButtonObject = GameObject.Find("Button_Play");
        GameObject removeButtonObject = GameObject.Find("Button_StopSearch");

        if (addButtonObject != null)
        {
            addButton = addButtonObject.GetComponent<Button>();
            if (addButton != null)
                addButton.onClick.AddListener(OnAddToTeam);
        }

        if (removeButtonObject != null)
        {
            removeButtonObject.SetActive(false);
            removeButton = removeButtonObject.GetComponent<Button>();
            if (removeButton != null)
                removeButton.onClick.AddListener(OnRemoveFromTeam);
        }
    }

    private void OnAddToTeam()
    {
        CmdAddToTeam();
    }

    private void OnRemoveFromTeam()
    {
        CmdRemoveFromTeam();
    }

    [Command]
    private void CmdAddToTeam()
    {
        GameObject player = connectionToClient.identity.gameObject;
        Debug.Log($"Adding player to team: {player.name}"); // Отладочное сообщение
        CNetworkManager.CustomNetworkManager.TeamCheck(player);
    }

    [Command]
    private void CmdRemoveFromTeam()
    {
        GameObject player = connectionToClient.identity.gameObject;
        Debug.Log($"Removing player from team: {player.name}"); // Отладочное сообщение
        CNetworkManager.CustomNetworkManager.RemoveFromTeam(player);
    }
}
