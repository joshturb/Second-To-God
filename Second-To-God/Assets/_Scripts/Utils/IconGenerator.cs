#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;

/// <summary>
/// Generates icon images for items in URP.
/// </summary>
public class ItemIconGenerator : EditorWindow
{
    private const int Resolution = 500;

    private Camera _cam;
    private string _filename;

    [MenuItem("Build Tools/Icon Generator")]
    private static void Init()
    {
        ItemIconGenerator window = GetWindow<ItemIconGenerator>("Icon Generator");
        window.minSize = window.maxSize = new Vector2(520, 650);
        window.Show();
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        UpdateCamera();

        Rect rect = EditorGUILayout.GetControlRect(false, Resolution);
        EditorGUI.DrawTextureTransparent(rect, _cam.targetTexture, ScaleMode.ScaleToFit);

        // Hack to get unity's project window path
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        string pathToCurrentFolder = obj.ToString();

        _filename = EditorGUILayout.TextField("Filename", _filename);

        if (string.IsNullOrWhiteSpace(_filename))
        {
            EditorGUILayout.HelpBox("Enter filename to continue", MessageType.Info);
            return;
        }

        if (!GUILayout.Button($"Save: {pathToCurrentFolder}/{_filename}.png"))
            return;

        RenderTexture prevRt = RenderTexture.active;
        RenderTexture.active = _cam.targetTexture;

        Texture2D captured = new(Resolution, Resolution, TextureFormat.RGBA32, false);
        Rect rectReadPicture = new(0, 0, Resolution, Resolution);
        captured.ReadPixels(rectReadPicture, 0, 0);

        Texture2D savedAsset = SaveIcon(captured, pathToCurrentFolder, _filename);

        EditorGUIUtility.PingObject(savedAsset);
        RenderTexture.active = prevRt;
    }

    private void UpdateCamera()
    {
        if (_cam == null)
        {
            FindOrCreateCam();
            EditorGUIUtility.PingObject(_cam.gameObject);
        }

        // Synchronize the custom camera with the scene view camera
        if (SceneView.lastActiveSceneView != null)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            _cam.transform.position = sceneCamera.transform.position;
            _cam.transform.rotation = sceneCamera.transform.rotation;
        }

        _cam.Render();
    }

    public Texture2D SaveIcon(Texture2D virtual2D, string assetPath, string filename)
    {
        byte[] encoded = virtual2D.EncodeToPNG();
        assetPath += "/" + filename + ".png";

        string systemPath = Application.dataPath;
        systemPath = systemPath.Remove(systemPath.LastIndexOf('/') + 1);
        systemPath += assetPath;
        systemPath = systemPath.Replace('/', Path.DirectorySeparatorChar);

        if (File.Exists(systemPath))
            File.Delete(systemPath);

        File.WriteAllBytes(systemPath, encoded);

        AssetDatabase.Refresh();

        TextureImporter typeChanger = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        typeChanger.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset(assetPath);

        return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
    }

    private void FindOrCreateCam()
    {
        const string camName = "Icon Render Camera";

        GameObject prevCam = GameObject.Find(camName);

        if (prevCam != null && prevCam.TryGetComponent(out _cam))
            return;

        GameObject camGo = new(camName);

        _cam = camGo.AddComponent<Camera>();
        _cam.nearClipPlane = 0.01f;
        _cam.farClipPlane = 100;
        _cam.targetTexture = new RenderTexture(Resolution, Resolution, 0);

        // Set the camera's background color and other properties for URP
        _cam.clearFlags = CameraClearFlags.Color;
        _cam.backgroundColor = new Color(1f, 1f, 1f, 0f); // White background with 0 alpha

        // Configure anti-aliasing via URP (in URP settings, set Anti-aliasing to MSAA)
        QualitySettings.antiAliasing = 4; // Assuming 4x MSAA (modify this depending on your project settings)

        EditorUtility.SetDirty(_cam);
    }
}

#endif
