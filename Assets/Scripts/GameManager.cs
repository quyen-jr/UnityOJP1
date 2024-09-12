using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Cross mode")]
    public Transform HouseObject;
    public Transform HouseToCross;
    public Transform CrossUI;

    [Header("Third and first camera mode ")]
    public Transform UI;
    public Camera mainCamera;
    public CinemachineFreeLook freeLookCamera;
    public Transform player;
    public Transform firstCamera;

    private bool isViewMode = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            isViewMode = !isViewMode;

            if (isViewMode)
            {
                UI.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                mainCamera.gameObject.SetActive(true);
                mainCamera.GetComponent<ControlCamera>().enabled = true;
                freeLookCamera.gameObject.SetActive(false);
                firstCamera.gameObject.SetActive(false);
                player.gameObject.SetActive(false);
            }
            else
            {
                UI.gameObject.SetActive(false);
                player.gameObject.SetActive(true);
                mainCamera.gameObject.SetActive(true);
                mainCamera.GetComponent<ControlCamera>().enabled = false;
                freeLookCamera.gameObject.SetActive(true);
                firstCamera.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;

                HouseObject.gameObject.SetActive(true);
                HouseToCross.gameObject.SetActive(false);
                CrossUI.gameObject.SetActive(false);
            }

        }
        Debug.Log(isViewMode);
        // cross mode
        if (isViewMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CrossUI.gameObject.activeSelf)
                {
                    HouseObject.gameObject.SetActive(true);
                    HouseToCross.gameObject.SetActive(false);
                    CrossUI.gameObject.SetActive(false);
                    UI.gameObject.SetActive(true);
                }
                else
                {
                    HouseObject.gameObject.SetActive(false);
                    HouseToCross.gameObject.SetActive(true);
                    CrossUI.gameObject.SetActive(true);
                    UI.gameObject.SetActive(false);
                }

            }
        }
    }
}
