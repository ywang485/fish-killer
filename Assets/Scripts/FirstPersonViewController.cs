using UnityEngine;
using Rewired;

public class FirstPersonViewController : MonoBehaviour {
    private Vector3 currentRotation;
    public float leftBound = -60;
    public float rightBound = 60;
    public float upBound = 90;
    public float downBound = -90;
    void Start () {
        currentRotation = transform.rotation.eulerAngles;
    }

    void Update () {
        var mouseDelta = ReInput.players.SystemPlayer.GetAxis2D("Horizontal View", "Vertical View");
        currentRotation.x = Mathf.Clamp(currentRotation.x - mouseDelta.y, -upBound, -downBound);
        currentRotation.y = Mathf.Clamp(currentRotation.y + mouseDelta.x, leftBound, rightBound);
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);
    }
}
