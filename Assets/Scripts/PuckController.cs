using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

using UnityEngine;

public class PuckController : NetworkBehaviour {
	public NetworkVariableFloat speed = new NetworkVariableFloat(new NetworkVariableSettings {
		WritePermission = NetworkVariablePermission.OwnerOnly,
		ReadPermission = NetworkVariablePermission.Everyone
	});
	public NetworkVariableVector2 direction = new NetworkVariableVector2(new NetworkVariableSettings {
		WritePermission = NetworkVariablePermission.OwnerOnly,
		ReadPermission = NetworkVariablePermission.Everyone
	});
	public NetworkVariableVector3 hitLocation = new NetworkVariableVector3(new NetworkVariableSettings {
		WritePermission = NetworkVariablePermission.OwnerOnly,
		ReadPermission = NetworkVariablePermission.Everyone
	});

	private NetworkObject networkObject;
	private Rigidbody2D rigidBody;

	public float startSpeed = 10f;
	void Start() {
		speed.Value = startSpeed;

		networkObject = GetComponent<NetworkObject>();
		rigidBody = GetComponent<Rigidbody2D>();
	}

	public void RandomLaunch() {
		Vector2 newDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		newDirection.Normalize();

		direction.Value = newDirection;

		if (newDirection.x > 0) {
			PassOwnershipServerRpc(0);
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (IsOwner) {
			Vector3 hitNormal = collision.GetContact(0).normal;
			direction.Value = Vector2.Reflect(direction.Value, hitNormal);

			GameObject colliderGameObject = collision.collider.gameObject;
			if (colliderGameObject.tag == "Player") {
				ulong paddleOwner = colliderGameObject.GetComponent<NetworkObject>().OwnerClientId;
				PassOwnershipServerRpc(paddleOwner);
			}

			hitLocation.Value = transform.position;
		}
	}

	[ServerRpc]
	void PassOwnershipServerRpc(ulong paddleOwner) {
		switch (paddleOwner) {
			case 0:
				networkObject.ChangeOwnership(2);
				break;
			case 2:
				networkObject.ChangeOwnership(0);
				break;
		}
	}

	void HitLocationChanged(Vector3 previousHit, Vector3 nextHit) {
		if (!IsOwner) {
			transform.position = nextHit;
		}
	}

	void FixedUpdate() {
		Vector3 tempVect = direction.Value.normalized * speed.Value * Time.deltaTime;
		rigidBody.MovePosition(transform.position + tempVect);
	}
}
