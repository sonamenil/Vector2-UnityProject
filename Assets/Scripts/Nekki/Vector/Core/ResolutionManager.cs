using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nekki.Vector.Core.User;
using UnityEngine;

public static class ResolutionManager
{
    private static Resolution[] resolutions;
    private static List<Resolution> filteredResolutions;

    private static int currentResolutionIndex;

    private static double currentRefreshRate;

    public static List<Resolution> UsedResolutions
    {
        get { return filteredResolutions; }
    }

    public static int CurrentResolutionIndex
    {
        get { return currentResolutionIndex; }
        set { currentResolutionIndex = value; }
    }

    public static Resolution DefaultResolution
    {
        get
        {
            return Screen.currentResolution;
        }
    }

    public static Resolution CurrentResolution
    {
        get
        {
            return filteredResolutions[currentResolutionIndex];
        }
        set
        {
            if (currentResolutionIndex == filteredResolutions.IndexOf(value))
            {
                return;
            }
            if (!filteredResolutions.Contains(value))
            {
                return;
            }
            SetResolution(value);

        }
    }

    public static event System.Action<Resolution> OnResolutionChanged;

    public static void Init()
    {
        Resolution dataresolution = DataLocal.Current.Settings.CurrentResolution;
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate || resolutions[i].refreshRateRatio.value + 1 == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == dataresolution.width && filteredResolutions[i].height == dataresolution.height && filteredResolutions[i].refreshRateRatio.value == dataresolution.refreshRateRatio.value || filteredResolutions[i].refreshRateRatio.value + 1 == dataresolution.refreshRateRatio.value)
            {
                CurrentResolution = filteredResolutions[i];
            }
        }
    }

    private static void SetResolution(Resolution resolution)
    {
        DebugUtils.Log("SwitchResolution: " + resolution.ToString());
        currentResolutionIndex = filteredResolutions.IndexOf(resolution);
        FullScreenMode fullscreen = DataLocal.Current.Settings.Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(resolution.width, resolution.height, fullscreen, resolution.refreshRateRatio);
        if (ResolutionManager.OnResolutionChanged != null)
        {
            ResolutionManager.OnResolutionChanged(filteredResolutions[currentResolutionIndex]);
        }
    }


}
