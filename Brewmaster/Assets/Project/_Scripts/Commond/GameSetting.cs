#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Game
{
	public class GameSetting
	{
		public static int FPS = 60;
	}

#if UNITY_EDITOR
	public static class HelpTool
	{
		[MenuItem("Tools/OpenScene/OpenMainMenu")]
		public static void OpenMainMenu()
		{
			EditorSceneManager.OpenScene("Assets/Project/_Scenes/MainMenu.unity");
		}
		[MenuItem("Tools/OpenScene/OpenGameScene")]
		public static void OpenGameScene()
		{
			EditorSceneManager.OpenScene("Assets/Project/_Scenes/GameScene.unity");
		}
	}
#endif
}
