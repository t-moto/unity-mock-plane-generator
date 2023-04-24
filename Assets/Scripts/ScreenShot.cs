using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ScreenShot
{
#if UNITY_EDITOR
    [MenuItem("Edit/CaptureGameView #3")]
#endif
    public static void CaptureGameView()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var fileName = string.Format("{0}_{1}.png", Application.productName, DateTime.Now.ToString("yyyyMMddHHmmss"));
        var path = Path.Combine(desktop, fileName);
        ScreenCapture.CaptureScreenshot(path);
    }
}
