using Mirror;
using UnityEngine;
using TMPro;

public class PlayerNameTag : NetworkBehaviour
{
	public TMP_Text playerNameText; // Текст для отображения никнейма
	public GameObject floatingInfo; // Объект для отображения информации над головой
	public GameObject playerNamePrefab; // Префаб для отображения имени игрока на канвасе

	[SyncVar(hook = nameof(OnNameChanged))]
	public string playerName; // Имя игрока

	private TMP_Text PlayerNameCanvas; // Дополнительный текст для отображения никнейма в другом объекте

	// Метод для обновления никнейма
	void OnNameChanged(string _Old, string _New)
	{
		playerNameText.text = _New; // Обновляем текст никнейма при изменении
		if (PlayerNameCanvas != null)
		{
			PlayerNameCanvas.text = _New; // Обновляем текст в другом объекте
		}
	}

	// Метод, который вызывается для локального игрока
	public override void OnStartLocalPlayer()
	{
		floatingInfo.transform.localPosition = new Vector3(0, 1.9f, 0); // Поднимаем над головой
		floatingInfo.transform.localScale = new Vector3(1f, 1f, 1f);

		string name = "Player" + Random.Range(100, 999); // Генерация случайного имени
		CmdSetupPlayer(name); // Установка имени игрока на сервере

		playerNameText.gameObject.SetActive(false); // Скрываем текст никнейма для локального игрока

		// Создаем экземпляр префаба для отображения имени на канвасе внутри NamePanel
		if (playerNamePrefab != null)
		{
			GameObject gameObject = GameObject.Find("Canvas/Game/NamePanel");
			if (gameObject != null)
			{
				GameObject nameObject = Instantiate(playerNamePrefab, gameObject.transform);
				PlayerNameCanvas = nameObject.GetComponent<TMP_Text>();
				UpdatePlayerNameText(name); // Обновляем текст никнейма на канвасе
			}
			else
			{
				Debug.LogWarning("Не удалось найти объект NamePanel.");
			}
		}
	}

	// Метод, который вызывается при инициализации
	public override void OnStartClient()
	{
		base.OnStartClient();

		// Ищем объект с компонентом TMP_Text по имени
		GameObject textObject = GameObject.Find("PlayerName"); // Замените на имя вашего объекта
		if (textObject != null)
		{
			PlayerNameCanvas = textObject.GetComponent<TMP_Text>();
		}

		if (!isLocalPlayer)
		{
			// Обновляем текст никнейма для других игроков
			playerNameText.text = playerName;
			if (PlayerNameCanvas != null)
			{
				PlayerNameCanvas.text = playerName; // Обновляем текст в другом объекте
			}
		}
	}

	// Команда для установки имени игрока на сервере
	[Command]
	public void CmdSetupPlayer(string _name)
	{
		playerName = _name; // Устанавливаем имя игрока на сервере
		Debug.Log($"Player name set to: {playerName}");
	}

	// Метод Update для поворота информации над головой
	void Update()
	{
		if (!isLocalPlayer)
		{
			// Поворачиваем информацию над головой для не локальных игроков
			floatingInfo.transform.LookAt(Camera.main.transform);
		}
	}

	// Обновление текста никнейма на канвасе
	private void UpdatePlayerNameText(string newName)
	{
		if (PlayerNameCanvas != null)
		{
			PlayerNameCanvas.text = newName; // Обновляем текст никнейма на канвасе
		}
	}

	// Команда для отправки сообщения
	//[Command]
	//public void CmdSendMessage(string message)
	//{
	//	string formattedMessage = $"{playerName}: {message}";
	//	ChatManager.Instance.RpcReceiveMessage(formattedMessage); // Отправляем сообщение всем клиентам
	//}
}
