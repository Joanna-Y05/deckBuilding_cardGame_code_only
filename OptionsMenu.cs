using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public Dropdown resDropDown;

    Resolution[] resolutions;

    void Start()
    {
      resolutions = Screen.resolutions;
      resDropDown.ClearOptions();
      List<string> options = new List<string>();

      int currentResIndex = 0;

      for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }  
        resDropDown.AddOptions(options);
        resDropDown.value = currentResIndex;
        resDropDown.RefreshShownValue();
    }

    public void VolumeSetting(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void QualitySetting(int QualityIndex)
    {
        QualitySettings.SetQualityLevel(QualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetRes(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width,res.height,Screen.fullScreen);
    }


    public void Back()
    {
        Debug.Log("sending back to main menu");
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
