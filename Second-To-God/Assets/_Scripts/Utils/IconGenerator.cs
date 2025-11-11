#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Reflection;
using System.IO;

/// <summary>
/// Generates icon images for items in URP.
/// Updated for newer Unity versions: safer RenderTexture creation, MSAA handling,
/// TextureImporter.SaveAndReimport usage, ReadPixels Apply(), and robust path fallback.
/// </summary>
public class ItemIconGenerator : EditorWindow
{
	private const int Resolution = 500;

	private Camera _cam;
	private string _filename;
	private RenderTexture _rt;

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

		// Show preview (RenderTexture is a Texture)
		Rect rect = EditorGUILayout.GetControlRect(false, Resolution);
		if (_cam != null && _cam.targetTexture != null)
			EditorGUI.DrawTextureTransparent(rect, _cam.targetTexture, ScaleMode.ScaleToFit);
		else
			EditorGUI.HelpBox(rect, "Waiting for camera...", MessageType.Info);

		// Try to get project window path via internal API; fallback to "Assets"
		string pathToCurrentFolder = "Assets";
		try
		{
			Type projectWindowUtilType = typeof(ProjectWindowUtil);
			MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
			if (getActiveFolderPath != null)
			{
				object obj = getActiveFolderPath.Invoke(null, new object[0]);
				if (obj != null)
					pathToCurrentFolder = obj.ToString();
			}
		}
		catch
		{
			pathToCurrentFolder = "Assets";
		}

		_filename = EditorGUILayout.TextField("Filename", _filename);

		if (string.IsNullOrWhiteSpace(_filename))
		{
			EditorGUILayout.HelpBox("Enter filename to continue", MessageType.Info);
			return;
		}

		// Sanitize filename a little
		foreach (char invalid in Path.GetInvalidFileNameChars())
			_filename = _filename.Replace(invalid, '_');

		if (!GUILayout.Button($"Save: {pathToCurrentFolder}/{_filename}.png"))
			return;

		// Ensure camera rendered and RT active
		if (_cam == null || _cam.targetTexture == null)
		{
			EditorUtility.DisplayDialog("Error", "Render camera or render texture missing.", "OK");
			return;
		}

		_cam.Render();

		RenderTexture prevRt = RenderTexture.active;
		RenderTexture.active = _cam.targetTexture;

		Texture2D captured = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false);
		Rect rectReadPicture = new Rect(0, 0, Resolution, Resolution);
		captured.ReadPixels(rectReadPicture, 0, 0);
		captured.Apply();

		Texture2D savedAsset = SaveIcon(captured, pathToCurrentFolder, _filename);

		if (savedAsset != null)
			EditorGUIUtility.PingObject(savedAsset);
		else
			Debug.LogWarning("Failed to save icon asset.");

		RenderTexture.active = prevRt;
	}

	private void UpdateCamera()
	{
		if (_cam == null)
		{
			FindOrCreateCam();
			if (_cam != null)
				EditorGUIUtility.PingObject(_cam.gameObject);
		}

		// Synchronize the custom camera with the scene view camera (if available)
		if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null && _cam != null)
		{
			Camera sceneCamera = SceneView.lastActiveSceneView.camera;
			_cam.transform.position = sceneCamera.transform.position;
			_cam.transform.rotation = sceneCamera.transform.rotation;
		}

		// Make sure RT exists and is current
		if (_cam != null)
		{
			if (_cam.targetTexture == null)
				CreateRenderTexture();
			_cam.Render();
		}
	}

	public Texture2D SaveIcon(Texture2D virtual2D, string assetPath, string filename)
	{
		if (virtual2D == null) return null;

		// Ensure PNG data is up-to-date
		byte[] encoded = virtual2D.EncodeToPNG();

		// Asset DB expects forward slashes
		string assetRelativePath = assetPath.TrimEnd('/') + "/" + filename + ".png";

		// Build system path to write file
		string projectRoot = Application.dataPath;
		projectRoot = projectRoot.Remove(projectRoot.LastIndexOf('/') + 1); // ends with '/'
		string systemPath = (projectRoot + assetRelativePath).Replace('/', Path.DirectorySeparatorChar);

		// Ensure directory exists
		string dir = Path.GetDirectoryName(systemPath);
		if (!Directory.Exists(dir))
			Directory.CreateDirectory(dir);

		if (File.Exists(systemPath))
			File.Delete(systemPath);

		File.WriteAllBytes(systemPath, encoded);

		// Refresh and configure importer
		AssetDatabase.Refresh();

		TextureImporter importer = AssetImporter.GetAtPath(assetRelativePath) as TextureImporter;
		if (importer != null)
		{
			importer.textureType = TextureImporterType.Sprite;
			importer.alphaIsTransparency = true;
			importer.mipmapEnabled = false;
#if UNITY_2021_2_OR_NEWER
			// Ensure readable if needed (optional)
			importer.isReadable = false;
#endif
			importer.SaveAndReimport();
		}
		else
		{
			Debug.LogWarning($"TextureImporter not found for path: {assetRelativePath}");
		}

		return AssetDatabase.LoadAssetAtPath<Texture2D>(assetRelativePath);
	}

	private void FindOrCreateCam()
	{
		const string camName = "Icon Render Camera";

		GameObject prevCam = GameObject.Find(camName);

		if (prevCam != null && prevCam.TryGetComponent(out _cam))
			return;

		GameObject camGo = new GameObject(camName);
		camGo.hideFlags = HideFlags.HideAndDontSave;

		_cam = camGo.AddComponent<Camera>();
		_cam.nearClipPlane = 0.01f;
		_cam.farClipPlane = 100f;
		_cam.clearFlags = CameraClearFlags.Color;
		_cam.backgroundColor = new Color(1f, 1f, 1f, 0f); // transparent background (white when composited)
		_cam.cameraType = CameraType.Preview;
		_cam.hideFlags = HideFlags.HideAndDontSave;

		CreateRenderTexture();

		// Configure anti-aliasing on RT; if QualitySettings.antiAliasing is 0, use 1 (no MSAA)
		if (_rt != null)
		{
			_rt.antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing);
		}

		EditorUtility.SetDirty(_cam);
	}

	private void CreateRenderTexture()
	{
		// Dispose previous RT if exists
		if (_rt != null)
		{
			if (_cam != null && _cam.targetTexture == _rt)
				_cam.targetTexture = null;
			_rt.Release();
			UnityEngine.Object.DestroyImmediate(_rt);
			_rt = null;
		}

		// Create a depth-enabled RT for correct rendering
		_rt = new RenderTexture(Resolution, Resolution, 24)
		{
			name = "IconRenderRT",
			useMipMap = false,
			autoGenerateMips = false,
			hideFlags = HideFlags.HideAndDontSave
		};

		// Respect project MSAA setting
		_rt.antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing);

		_rt.Create();

		if (_cam != null)
			_cam.targetTexture = _rt;
	}
}

#endif
