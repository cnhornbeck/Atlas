using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 5.0f;
    private Vector2 rotation = Vector2.zero;
    private Camera mainCamera;
    private bool isCursorLocked = true;

    private void Start()
    {
        // Initialize camera and lock cursor
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        HandleMovement();
        HandleRotation();
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
        }
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLocked;
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Jump");
        float z = Input.GetAxis("Vertical");

        // Get value of scroll wheel
        float scroll = 0;
        scroll += Input.GetAxis("Mouse ScrollWheel");
        speed = Mathf.Pow(2, scroll * 2) * speed;

        Vector3 moveDirection = transform.right * x + transform.up * y + mainCamera.transform.forward * z;
        transform.position += speed * Time.deltaTime * moveDirection;
    }

    private void HandleRotation()
    {
        rotation.y += Input.GetAxis("Mouse X") * lookSensitivity;
        rotation.x -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotation.x = Mathf.Clamp(rotation.x, -90, 90);

        // Apply rotation
        transform.localEulerAngles = new Vector3(0f, rotation.y, 0f);
        mainCamera.transform.localEulerAngles = new Vector3(rotation.x, 0f, 0f);
    }
}
