using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summery>
/// This class just provides a nice easy way to select your scene from the top menu bar. If you add, remove, or rename scenes, make sure to update the paths in each MenuItem function.
/// </summery>

public class ScenesMenuItem : MonoBehaviour
{
	private static bool loadScene(string scenePath)
	{
		Scene s = EditorSceneManager.OpenScene(scenePath);
		if (s != null)
			return s.isLoaded;
		else
			return false;
	}

	private static void showEditorWindow<T>(string title) where T : EditorWindow
	{
		EditorWindow window = EditorWindow.GetWindow<T>(false, title, true);
		if (window != null)
			window.Show(true);
	}

	////////////////////////////////////////////////////////////////////////////////////////

	[MenuItem("Scenes/Open Startup Scene")]
	static void OpenStartupScene()
	{
		loadScene("Assets/Scenes/Startup/StartupScene.unity");
	}

	[MenuItem("Scenes/Open Static Scene")]
	static void OpenStaticScene()
	{
		loadScene("Assets/Scenes/StaticScene/StaticScene.unity");
	}

	[MenuItem("Scenes/Open Visual Flow Scene")]
	static void OpenVisualFlow()
	{
		loadScene("Assets/Scenes/VisualFlow/VisualFlow.unity");
	}

    [MenuItem("Scenes/Open Closed Room Scene")]
    static void OpenClosedRoom()
    {
        loadScene("Assets/Scenes/ClosedRoom/ClosedRoom.unity");
    }

}

