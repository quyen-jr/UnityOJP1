using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICrossSection : MonoBehaviour
{
    public Slider slider;
    public Transform crossPlane;
    public Toggle toggleX;
    public Toggle toggleY;
    public Toggle toggleZ;
    void Start()
    {
        toggleX.onValueChanged.AddListener(delegate { OnToggleChanged(toggleX); });
        toggleY.onValueChanged.AddListener(delegate { OnToggleChanged(toggleY); });
        toggleZ.onValueChanged.AddListener(delegate { OnToggleChanged(toggleZ); });


        slider.onValueChanged.AddListener(OnSliderValueChanged);



    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        }
    }
    void OnToggleChanged(Toggle changedToggle)
    {


        if (changedToggle.isOn)
        {
            if (changedToggle == toggleX)
            {
                crossPlane.transform.localPosition = new Vector3(-10, -8.5f, 9);
                crossPlane.transform.localEulerAngles = new Vector3(0, 0, 90);
                toggleY.isOn = false;
                toggleZ.isOn = false;
                slider.minValue = -10;
                slider.maxValue = 30;

                slider.value = slider.minValue;
            }
            else if (changedToggle == toggleY)
            {
                crossPlane.transform.localPosition = new Vector3(8, 6, 6);
                crossPlane.transform.localEulerAngles = new Vector3(0, 0, 0);
                toggleX.isOn = false;
                toggleZ.isOn = false;

                slider.minValue = -18;
                slider.maxValue = 6;

                slider.value = slider.maxValue;
            }
            else if (changedToggle == toggleZ)
            {
                crossPlane.transform.localPosition = new Vector3(9, -8.5f, -10);
                crossPlane.transform.localEulerAngles = new Vector3(-90, 0, 0);
                toggleX.isOn = false;
                toggleY.isOn = false;
                slider.minValue = -10;
                slider.maxValue = 26;

                slider.value = slider.minValue;
            }
        }
    }
    void OnSliderValueChanged(float value)
    {
        Vector3 currentPosition = crossPlane.transform.localPosition;

        if (toggleX.isOn)
        {

            currentPosition.x = value;
        }
        else if (toggleY.isOn)
        {
            currentPosition.y = value;
        }
        else if (toggleZ.isOn)
        {
            currentPosition.z = value;
        }
        crossPlane.transform.localPosition = currentPosition;
        Debug.Log($"Plane Position: {crossPlane.transform.position}");
    }

}
