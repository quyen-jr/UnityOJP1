using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class IMGCapInfo : MonoBehaviour
{
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private Image image;
    private Camera mainCamera;
    private bool isMoveCamera;
    private float moveSpeed = 2f;
    private float rotateSpeed = 2f;
    private void OnDisable()
    {
        isMoveCamera = false;
    }
    void Start()
    {
        mainCamera = GameObject.Find("mainCamera").GetComponent<Camera>();
    }
    void Update()
    {
        if (isMoveCamera)
        {
            //Debug.Log(targetPos + "  " + targetRotation);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, moveSpeed * Time.deltaTime);

            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation,
            Quaternion.Euler(targetRotation), rotateSpeed * Time.deltaTime);

            if (Vector3.Distance(mainCamera.transform.position, targetPos) < 1f)
            {
                isMoveCamera = false;
                Debug.Log("false");
            }
        }
    }
    public void MoveCamera()
    {
        //Debug.Log("move camera");
        isMoveCamera = true;
    }
    public void StopMoving()
    {
        isMoveCamera = false;
    }
    public void SetCurrentPosAndRotateOfCamera()
    {
        mainCamera = GameObject.Find("mainCamera").GetComponent<Camera>();
        //Debug.Log(mainCamera.transform.position+"  "+ mainCamera.transform.rotation.eulerAngles);
        targetPos = mainCamera.transform.position;
        targetRotation = mainCamera.transform.rotation.eulerAngles;
    }
}
