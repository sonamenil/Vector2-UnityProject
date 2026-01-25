using System.Runtime.InteropServices;
using UnityEngine;

public class DeviceUniqueID
{
    public static string GetID
    {
        get
        {
            return _GetID();
        }
    }
    private static string _GetID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}
