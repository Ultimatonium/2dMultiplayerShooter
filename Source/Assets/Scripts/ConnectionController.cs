using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ConnectionController : NetworkBehaviour
{
    [SerializeField]
    private Text hostIpTextfield;

    public void OnStartHostButtonPressed()
    {
        NetworkManager.singleton.StartHost();
        DisableConnectionHud();
    }

    public void OnStartClientButtonPressed()
    {
        if (hostIpTextfield.text != "")
        {
            NetworkManager.singleton.networkAddress = hostIpTextfield.text;
        }
        NetworkManager.singleton.StartClient();
        DisableConnectionHud();
    }

    private void DisableConnectionHud()
    {
        transform.root.gameObject.SetActive(false);
    }
}
