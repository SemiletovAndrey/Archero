#if  UNITY_EDITOR
using DataEditorFramework.Core.Editor.Scripts.Tabs;
using UnityEditor;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor
{
    public class GameDataEditorWindow : EditorWindow
    {
        private TypeListView _typeList;
        private TabController _tabs;

        [MenuItem("Tools/Game Data Editor")]
        public static void Open()
        {
            GetWindow<GameDataEditorWindow>("Game Data Editor");
        }
        
        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            path = System.IO.Path.GetDirectoryName(path);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{path}/Styles/GameDataEditor.uss");
            
            _typeList = new TypeListView(OnTypeSelected);
            _tabs = new TabController();

            root.Add(BuildLayout());
            root.styleSheets.Add(styleSheet);
            _tabs.RestoreTabSession();
        }

        private VisualElement BuildLayout()
        {
            rootVisualElement.style.flexGrow = 1;

            var container = new TwoPaneSplitView(0, 220, TwoPaneSplitViewOrientation.Horizontal)
            {
                style = { flexGrow = 1 }
            };
            container.AddToClassList("editor-layout");

            container.Add(_typeList);
            container.Add(_tabs.Root);
            return container;
        }

        private void OnTypeSelected(System.Type type)
        {
            _tabs.OpenTab(type);
        }

        private void OnDisable()
        {
            _tabs?.SaveAllTabs();
            _tabs?.SaveTabSession();
        }
    }
}
#endif