using MLAPI;
using MLAPI.Puncher.Client;
using MLAPI.Transports.UNET;
using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class TempNetworkUI : MonoBehaviour {
    static private UNetTransport transport;

	private void Start() {
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
    }

	void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            StartTextBox();
            StartButtons();
        } else {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    static private string address = "127.0.0.1";

    static void StartTextBox() {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Address:");
        address = GUILayout.TextField(address);

        GUILayout.EndHorizontal();
    }

    static string PUNCH_ADDRESS = "connieprice.co.uk";
    static ushort PUNCH_PORT = 6776;

    static void NATListen() {
        Task listenTask = Task.Factory.StartNew(() => {
            using (PuncherClient listenPeer = new PuncherClient(PUNCH_ADDRESS, PUNCH_PORT)) {
                Debug.Log("[LISTENER] Listening for single punch on our port 7777...");
                IPEndPoint endpoint = listenPeer.ListenForSinglePunch(new IPEndPoint(IPAddress.Any, 7777));
                Debug.Log("[LISTENER] Connector: " + endpoint + " punched through our NAT");
            }
        });
    }

    static bool NATPunch() {
        using (PuncherClient connector = new PuncherClient(PUNCH_ADDRESS, PUNCH_PORT)) {
            IPEndPoint remoteEndPoint;
            bool punchSuccessful = connector.TryPunch(IPAddress.Parse(address), out remoteEndPoint);
            
            if (punchSuccessful) {
                transport.ConnectAddress = remoteEndPoint.Address.ToString();
                transport.ConnectPort = remoteEndPoint.Port;

                return true;
            }
        }

        return false;
    }

    static void Host() {
        NetworkManager.Singleton.StartHost();
        NATListen();
    }

    static void Connect() {
        if (NATPunch()) {
            NetworkManager.Singleton.StartClient();
        }
    }

	static void StartButtons() {
        if (GUILayout.Button("Host")) Host();
        if (GUILayout.Button("Client")) Connect();
    }

    static void StatusLabels() {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}