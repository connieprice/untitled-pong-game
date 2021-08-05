using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class PuckSpawner : NetworkBehaviour {
	public GameObject puckPrefab;

	bool puckSpawned = false;
	void Update() {
		if (!puckSpawned && NetworkManager.Singleton.ConnectedClientsList.Count == 2) {
			GameObject puck = Instantiate(puckPrefab, Vector3.zero, Quaternion.identity);
			puck.GetComponent<NetworkObject>().Spawn();

			puck.GetComponent<PuckController>().RandomLaunch();

			puckSpawned = true;
		}
	}
}
