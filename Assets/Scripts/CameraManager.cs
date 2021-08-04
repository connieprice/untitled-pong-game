using UnityEngine;

[ExecuteInEditMode]
public class CameraManager : MonoBehaviour {
    private void OnGUI() {
        float aspectRatioDesign = (16f / 9f);
        float orthographicStartSize = 30f;

        float inverseAspectRatio = 1 / aspectRatioDesign;
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        if (currentAspectRatio > aspectRatioDesign) {
            currentAspectRatio -= (currentAspectRatio - aspectRatioDesign);
        } else if (currentAspectRatio < inverseAspectRatio) {
            currentAspectRatio += (currentAspectRatio - inverseAspectRatio);
        }

        Camera.main.orthographicSize = aspectRatioDesign * (orthographicStartSize / currentAspectRatio);
    }
}
