using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // sky box
    [SerializeField] private Material morningSkyBox;
    [SerializeField] private Material affternoonSkyBox;
    [SerializeField] private Material nightSkyBox;

    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField] TextMeshProUGUI textTimeScroll; 
    [SerializeField] TextMeshProUGUI textTime;


    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime/10;
            TimeOfDay %= 24; 
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }


    private void UpdateLighting(float timePercent)
    {
        UpdateSkyBox(timePercent);
        SetTextTime();

        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 0f, 0));
        }

    }

    private void SetTextTime()
    {
        int hours = Mathf.FloorToInt(TimeOfDay);
        int minutes = Mathf.FloorToInt((TimeOfDay - hours) * 60);

        string period = hours >= 12 ? "PM" : "AM";
        int displayHours = hours % 12;
        displayHours = displayHours == 0 ? 12 : displayHours;

        textTimeScroll.SetText($"{displayHours:D2}:{minutes:D2}");
        textTime.SetText($"{displayHours:D2}:{minutes:D2} {period}");
    }

    private void UpdateSkyBox(float _time)
    {
        if (_time >= 9f/24f&&_time < 15f/24f)
        {
            RenderSettings.skybox = morningSkyBox;
        }else
        if (_time >= 15f/24f &&_time <17f/24f)
        {
            RenderSettings.skybox = affternoonSkyBox ;
        }else
        {
            RenderSettings.skybox = nightSkyBox;
        }
    }

    public void ChangeTimeScroll(float _selectedTime)
    {
        TimeOfDay= _selectedTime%24;
        //if (_isInscrease)
        //{
        //    TimeOfDay += 3f / 24f;
        //}
        //else 
        //    if(TimeOfDay >=3) TimeOfDay -= 3f / 24f;
        UpdateLighting(TimeOfDay / 24f);
    }

}