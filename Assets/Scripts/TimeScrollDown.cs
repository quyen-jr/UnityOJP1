using UnityEngine;

public class TimeScrollDown : MonoBehaviour
{
    public RectTransform content;
    public Transform scrollview;
    public LightingManager lightManager;
    private Vector2 previousPosition;

    public float selectedHour;
    void Start()
    {
        previousPosition = content.anchoredPosition;
    }

    void Update()
    {
        if (content.anchoredPosition.y < 0)
        {
            content.anchoredPosition.Set(content.anchoredPosition.x, 0);
        }
        selectedHour = (content.anchoredPosition.y / 40f);

        Vector2 currentPosition = content.anchoredPosition;
        if (currentPosition.y != previousPosition.y)
        {
            Debug.Log("Cuộn");
            lightManager.ChangeTimeScroll(selectedHour);
        }
        previousPosition = currentPosition;
    }
    public void EnableScrollview()
    {
        if (scrollview.gameObject.activeSelf == true)
            scrollview.gameObject.SetActive(false);
        else scrollview.gameObject.SetActive(true);
    }

}
