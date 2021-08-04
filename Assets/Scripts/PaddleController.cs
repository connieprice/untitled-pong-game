using MLAPI;
using UnityEngine;

public class PaddleController : NetworkBehaviour {
    public float speed = 3.0f;

	private float sideEdge = 50f;
    private float topEdge = 27f;

    private void Update() {
        float x = OwnerClientId == 0 ? -sideEdge : sideEdge;
        float y;

        if (IsLocalPlayer) {
            float input = Input.GetAxis("Vertical");

            y = Mathf.Clamp(transform.position.y + (speed * input * Time.deltaTime), -topEdge, topEdge);
            transform.position = new Vector3(x, y, 0);
        }
    }
}
