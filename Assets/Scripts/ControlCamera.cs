using UnityEngine;
using UnityEngine.EventSystems;

public class ControlCamera : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private Transform target;
    private Vector2 preMousePos;
    private bool isRotate = false;
    private float speedRotation = 5f;
    private float distanceToTarget;

    [Header("Zoom camera")]
    [SerializeField] private float minFov = 10;
    [SerializeField] private float maxFov = 70;
    [SerializeField] private float sensivity = 15f;
    private float fov = 65;

    [Header("Pan camera")]
    [SerializeField] private float panSpeed;
    private Vector3 prePanMousePos;

    private bool isInteracWithUI;
    private bool disableControl;
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        preMousePos = new Vector2(0.5f, 0.5f);
        distanceToTarget = Vector3.Distance(transform.position, target.position);
    }

    void Update()
    {


        if (disableControl)
        {
            return;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                isInteracWithUI = true;
            }
            else
            {
                isInteracWithUI = false;
            }
        }


        RotateAroundObjWithLeftMouse();
        PanCameraWithRightMouse();
        if (isInteracWithUI) return;
        ZoomWithMouseWheel();
    }

    private void RotateAroundObjWithLeftMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotate = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotate = false;
        }

        if (isRotate)
        {
            float deltaX = Input.GetAxis("Mouse X") * speedRotation;
            float deltaY = Input.GetAxis("Mouse Y") * speedRotation;

            transform.RotateAround(target.position, Vector3.up, deltaX);

            transform.RotateAround(target.position, transform.right, -deltaY);
        }

        //

        transform.LookAt(target);
    }
    private void ZoomWithMouseWheel()
    {
        fov = mainCamera.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        mainCamera.fieldOfView = fov;
    }
    public void PanCameraWithRightMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prePanMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {

            Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition - prePanMousePos);
            //Debug.Log(pos);
            Vector3 distanceToMove = transform.forward * pos.y + transform.right * pos.x;
            transform.Translate(distanceToMove * panSpeed * Time.deltaTime, Space.World);
        }
    }
    public void DisableControl()
    {
        disableControl = true;
    }
}
