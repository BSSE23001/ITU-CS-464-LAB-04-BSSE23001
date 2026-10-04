using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class QuickPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 7.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -19.6f;

    [Header("Look Settings")]
    [Tooltip("New input system uses pixel deltas, so keep this between 0.05 and 0.2")]
    public float mouseSensitivity = 0.12f;

    private CharacterController controller;
    private Transform cam;
    private float verticalVelocity;
    private float pitch = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        Camera childCam = GetComponentInChildren<Camera>();
        if (childCam != null)
        {
            cam = childCam.transform;
        }
        else
        {
            Debug.LogError("No Camera found as a child of the Player! Drag your Main Camera under the Player Capsule.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        // Safety check if devices aren't detected
        if (keyboard == null || mouse == null || cam == null) return;

        // 1. Mouse Look
        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseDelta.x);

        pitch = Mathf.Clamp(pitch - mouseDelta.y, -85f, 85f);
        cam.localEulerAngles = Vector3.right * pitch;

        // 2. Ground Check & Gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small grounding force to keep contact on slopes
        }

        // 3. Jump (Spacebar)
        if (keyboard.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        // 4. WASD Movement
        float moveX = 0f;
        float moveZ = 0f;

        if (keyboard.wKey.isPressed) moveZ += 1f;
        if (keyboard.sKey.isPressed) moveZ -= 1f;
        if (keyboard.dKey.isPressed) moveX += 1f;
        if (keyboard.aKey.isPressed) moveX -= 1f;

        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;
        Vector3 finalVelocity = (moveDirection * speed) + (Vector3.up * verticalVelocity);

        controller.Move(finalVelocity * Time.deltaTime);
    }
}