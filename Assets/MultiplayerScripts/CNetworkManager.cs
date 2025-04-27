using Mirror;
using UnityEngine;

public class CNetworkManager : NetworkManager
{
	// Ссылка на объект, который нужно включить при отключении клиента
	public GameObject objectToEnable;
	public GameObject objectToDisable;

	// Метод вызывается при старте игры
	public override void Awake()
	{
		base.Awake();
		// Подключаем клиента сразу при запуске сцены
		StartClient();
	}

	// Метод вызывается, когда клиент отключается
	public override void OnClientDisconnect()
	{
		base.OnClientDisconnect();
		Debug.Log("Client has stopped. Activating object.");
        
		// Включаем указанный объект только для отключающегося клиента
		EnableObject();
	}

	// Метод для включения объекта
	private void EnableObject()
	{
		if (objectToEnable != null)
		{
			Cursor.lockState = CursorLockMode.None; // Разблокировка курсора
			Cursor.visible = true; // Сделать курсор видимым
			objectToEnable.SetActive(true);
			if (objectToDisable != null)
			{
				objectToDisable.SetActive(false);
			}
			Debug.Log("Object activated.");
		}
		else
		{
			Debug.LogWarning("objectToEnable is not assigned!");
		}
	}
}
