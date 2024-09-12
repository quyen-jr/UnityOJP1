using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureImage : MonoBehaviour
{
    [Header("Content default")]
    [SerializeField] private Transform contentDefault;
    [SerializeField] private Button defaultBtn;
    [Header("Content custom")]
    [SerializeField] private Transform contentCustom;
    [SerializeField] private List<Button> capImgButtonsList;
    [SerializeField] private Button customBtn;
    [SerializeField] private GameObject imgCapInfoPrefab;

    private Camera mainCamera;
   
    void Start()
    {
        mainCamera = GameObject.Find("mainCamera").GetComponent<Camera>();
        // set up imgbtn for contentdefault in scrollview
        Button[] buttonsImage = contentDefault.GetComponentsInChildren<Button>();
        Debug.Log(buttonsImage.Length);
        foreach (Button button in buttonsImage)
        {
            IMGCapInfo btnCapInfo = button.GetComponent< IMGCapInfo>();
            button.onClick.AddListener(() =>
            {
                foreach (Button button in buttonsImage)
                {
                    IMGCapInfo ortherBtnCapInfo = button.GetComponent<IMGCapInfo>();
                    ortherBtnCapInfo.StopMoving();
                }
                btnCapInfo.MoveCamera();
            });
        }
        // set up btn
        customBtn.onClick.AddListener(() => EnableContentCustom());
        defaultBtn.onClick.AddListener(() => EnableContentDefault());

    }
    private void AddNewCapImgCustom(Button _newImgPos)
    {
        Debug.Log(mainCamera.transform.position + "  " + mainCamera.transform.rotation.eulerAngles);
        IMGCapInfo btnCapInfo = _newImgPos.GetComponent<IMGCapInfo>();
        btnCapInfo.SetCurrentPosAndRotateOfCamera();
        _newImgPos.onClick.AddListener(() =>
        {
            foreach (Button button in capImgButtonsList)
            {
                IMGCapInfo ortherBtnCapInfo = button.GetComponent<IMGCapInfo>();
                ortherBtnCapInfo.StopMoving();
            }
            btnCapInfo.MoveCamera();
        });
        capImgButtonsList.Add(_newImgPos);

        _newImgPos.transform.SetParent(contentCustom);

    }
    void Update()
    {
        if (contentCustom.gameObject.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                CaptureSceneImage();
            }
        }
    }
    public void CaptureSceneImage()
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height,24);
        mainCamera.targetTexture=renderTexture;
        mainCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D captureImage= new Texture2D(Screen.width,Screen.height, TextureFormat.RGB24,false);
        captureImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height),0,0);
        captureImage.Apply();
        // reset camera
        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Sprite newImg = Sprite.Create(captureImage, new Rect(0, 0, captureImage.width, captureImage.height), new Vector2(0.5f, 0.5f));

        //add ImgObject to customContent
        GameObject imgCapInfoObj = Instantiate(imgCapInfoPrefab);
        Button newImgBtn = imgCapInfoObj.GetComponent<Button>();
        AddNewCapImgCustom(newImgBtn);
        //imgCapInfoPrefab.GetComponent<IMGCapInfo>().SetCurrentPosAndRotateOfCamera();
        imgCapInfoObj.GetComponent<Image>().sprite = newImg;

    }
    public void EnableContentCustom()
    {
        customBtn.GetComponent<Image>().color = new Color(0.53f, 0.81f, 0.92f, 1f); // sky blue
        defaultBtn.GetComponent<Image>().color = Color.black;
        contentCustom.gameObject.SetActive(true);
        contentDefault.gameObject.SetActive(false);
    }
    public void EnableContentDefault()
    {
        defaultBtn.GetComponent<Image>().color = new Color(0.53f, 0.81f, 0.92f, 1f); // sky blue
        customBtn.GetComponent<Image>().color = Color.black;
        contentCustom.gameObject.SetActive(false);
        contentDefault.gameObject.SetActive(true);
    }
}
