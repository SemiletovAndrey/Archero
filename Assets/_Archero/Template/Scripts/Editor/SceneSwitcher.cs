#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Template.PlatformUtils
{
    public class SceneSwitcher : EditorWindow
    {
        private List<string> buildScenePaths = new List<string>();
        private List<string> allScenePaths = new List<string>();
        private Vector2 scrollPosition;
        private string lastSceneBeforePlay;
        private bool restoreSceneAfterPlay;
        private Texture2D sceneIcon;
        private Texture2D playIcon;

        [MenuItem("BG Games Platform/Scene Switcher")]
        public static void ShowWindow()
        {
            GetWindow<SceneSwitcher>("Scene Switcher");
        }

        private void OnEnable()
        {
            RefreshScenes();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            sceneIcon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image as Texture2D;
            playIcon = EditorGUIUtility.IconContent("d_PlayButton").image as Texture2D;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void RefreshScenes()
        {
            buildScenePaths.Clear();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    buildScenePaths.Add(scene.path);
                }
            }

            allScenePaths.Clear();
            string[] allScenes = AssetDatabase.FindAssets("t:Scene");
            foreach (string guid in allScenes)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.StartsWith("Packages/") && !buildScenePaths.Contains(path))
                {
                    allScenePaths.Add(path);
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Scene Switcher", EditorStyles.boldLabel);

            if (GUILayout.Button("Refresh Scenes"))
            {
                RefreshScenes();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("Scenes in Build Settings", EditorStyles.boldLabel);
            foreach (var scenePath in buildScenePaths)
            {
                DrawSceneRow(scenePath);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("All Other Scenes", EditorStyles.boldLabel);
            foreach (var scenePath in allScenePaths)
            {
                DrawSceneRow(scenePath);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSceneRow(string scenePath)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(scenePath), GUILayout.Width(200));

            if (GUILayout.Button(new GUIContent(sceneIcon, "Open this scene"), GUILayout.Width(30),
                    GUILayout.Height(20)))
            {
                OpenScene(scenePath);
            }

            if (GUILayout.Button(new GUIContent(playIcon, "Run this scene (returns to previous scene after stopping)"),
                    GUILayout.Width(30), GUILayout.Height(20)))
            {
                RunScene(scenePath);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }

        private void RunScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                lastSceneBeforePlay = SceneManager.GetActiveScene().path;
                restoreSceneAfterPlay = true;

                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
            }
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode && restoreSceneAfterPlay &&
                !string.IsNullOrEmpty(lastSceneBeforePlay))
            {
                EditorSceneManager.OpenScene(lastSceneBeforePlay);
                restoreSceneAfterPlay = false;
            }
        }
    }
}
#endif