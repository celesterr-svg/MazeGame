using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 2f;
    public float smoothTime = 0.05f;
    public float cameraFollowSpeed = 10f;

    [Header("References")]
    public Transform playerCamera;
    public Rigidbody player;

    private float xRotation = 0f;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private Vector3 cameraVelocity = Vector3.zero;
    private Transform cameraTarget;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player.freezeRotation = true;

        GameObject cameraPivot = new GameObject("Camera Pivot");
        cameraTarget = cameraPivot.transform;
        cameraTarget.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        pMovement();
        mouseLook();
        smoothCameraFollow();
    }

    void pMovement() 
    { 
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= speed;
        move.y = player.velocity.y;

        player.velocity = move;
    }

    void mouseLook() 
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        Vector2 targetMouseDelta = new Vector2(mouseX, mouseY);
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, smoothTime);

        xRotation -= currentMouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * currentMouseDelta.x);
    }

    void smoothCameraFollow() 
    {
        cameraTarget.position = Vector3.SmoothDamp(cameraTarget.position, transform.position, ref cameraVelocity, 1f / cameraFollowSpeed);
        playerCamera.position = cameraTarget.position;
    }
}
