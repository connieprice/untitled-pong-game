using MLAPI;
using MLAPI.Transports.UNET;
using System;
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
    static private string port = "7777";

    static void StartTextBox() {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Address:");
        address = GUILayout.TextField(address);

        GUILayout.Label("Port:");
        port = GUILayout.TextField(port);

        GUILayout.EndHorizontal();
    }

    static bool HandleAddressPort() {
        int portInt;
        bool worked = Int32.TryParse(port, out portInt);

        if (worked) {
            transport.ConnectAddress = address;

            transport.ConnectPort = portInt;
            transport.ServerListenPort = portInt;
        }

        return worked;
    }

    static void Host() {
        if (HandleAddressPort()) {
            NetworkManager.Singleton.StartHost();
        }
    }

    static void Connect() {
        if (HandleAddressPort()) {
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