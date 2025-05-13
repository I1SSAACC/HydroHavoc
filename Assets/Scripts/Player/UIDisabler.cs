using UnityEngine;
using Mirror;

public class UIDisabler : NetworkBehaviour
{

    public void UIDisable()
    {
        if (isLocalPlayer)
        {
            GameObject.Find("---UI").SetActive(false);
            GameObject.Find("MainCamera").GetComponent<AudioListener>().enabled = false;
        }
    }
}
