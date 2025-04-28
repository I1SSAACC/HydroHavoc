using Mirror;
using UnityEngine;
using TMPro;

public class PlayerNameTag : NetworkBehaviour
{
    public TMP_Text PlayerNameText;
    public GameObject FloatingInfo;
    public GameObject PlayerNamePrefab;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string PlayerName;

    private TMP_Text _playerNameCanvas;

    void OnNameChanged(string OldName, string NewName)
    {
        if (_playerNameCanvas == null)
            return;

        _playerNameCanvas.text = NewName;
    }

    public override void OnStartLocalPlayer()
    {
        FloatingInfo.transform.localPosition = new Vector3(0, 1.9f, 0);
        FloatingInfo.transform.localScale = new Vector3(1f, 1f, 1f);

        string name = "Player" + Random.Range(100, 999);
        CmdSetupPlayer(name);

        PlayerNameText.gameObject.SetActive(false);

        if (PlayerNamePrefab == null)
        {
            Debug.LogWarning("PlayerNamePrefab равен null.");
            return;
        }

        GameObject gameObject = GameObject.Find("Canvas/Game/NamePanel");
        if (gameObject == null)
        {
            Debug.LogWarning("Не удалось найти объект NamePanel.");
            return;
        }

        GameObject nameObject = Instantiate(PlayerNamePrefab, gameObject.transform);
        _playerNameCanvas = nameObject.GetComponent<TMP_Text>();

        if (_playerNameCanvas == null)
        {
            Debug.LogWarning("Не удалось получить компонент TMP_Text.");
            return;
        }

        UpdatePlayerNameText(name);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        GameObject textObject = GameObject.Find("PlayerName");
        if (textObject != null)
        {
            _playerNameCanvas = textObject.GetComponent<TMP_Text>();
        }

        if (!isLocalPlayer)
        {
            PlayerNameText.text = PlayerName;
            if (_playerNameCanvas != null)
            {
                _playerNameCanvas.text = PlayerName;
            }
        }
    }

    [Command]
    public void CmdSetupPlayer(string _name)
    {
        PlayerName = _name;
        Debug.Log($"Player name set to: {PlayerName}");
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        FloatingInfo.transform.LookAt(Camera.main.transform);
    }

    private void UpdatePlayerNameText(string newName)
    {
        if (_playerNameCanvas == null)
            return;

        _playerNameCanvas.text = newName;
    }
}