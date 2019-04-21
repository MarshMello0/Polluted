#if UNITY_EDITOR
      using UnityEditor;
      using UnityEditor.SceneManagement;
           
      [InitializeOnLoad]
      public class ScenePlayer
      {
          static ScenePlayer()
          {
              if (EditorBuildSettings.scenes.Length == 0)
                  return;
           
              EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
          }
      }
      #endif