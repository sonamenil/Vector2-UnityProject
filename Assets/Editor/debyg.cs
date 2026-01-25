using UnityEditor;
using UnityEngine;
using Nekki.Vector.Core;

public class debyg : MonoBehaviour
{
    [MenuItem("Debug/Test")]
    public static void Test()
    {
        RunMainController.Location.GetUserModel().StartPhysics();
    }
}
