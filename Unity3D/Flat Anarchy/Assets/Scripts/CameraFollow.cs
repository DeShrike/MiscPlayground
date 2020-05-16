using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float xSmooth = 8f; // How smoothly the camera catches up with it's target movement in the x axis.
    public float ySmooth = 8f; // How smoothly the camera catches up with it's target movement in the y axis.
    public Transform FollowObject; // Reference to the player's transform.

    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void Update()
    {
        this.TrackPlayer();
    }

    private void TrackPlayer()
    {
        // By default the target x and y coordinates of the camera are it's current x and y coordinates.
        float targetX = transform.position.x;
        float targetY = transform.position.y;

        targetX = Mathf.Lerp(transform.position.x, this.FollowObject.position.x, xSmooth * Time.deltaTime);
        targetY = Mathf.Lerp(transform.position.y, this.FollowObject.position.y, ySmooth * Time.deltaTime);

        // Set the camera's position to the target position with the same z component.
        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}
