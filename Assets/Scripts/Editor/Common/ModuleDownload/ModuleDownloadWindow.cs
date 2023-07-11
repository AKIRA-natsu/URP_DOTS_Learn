using UnityEngine;
using UnityEditor;

namespace AKIRA.Editor {
    public class ModuleDownloadWindow : EditorWindow {

        [MenuItem("Tools/AKIRA.Framework/Common/ModuleDownloadWindow")]
        private static void ShowWindow() {
            var window = GetWindow<ModuleDownloadWindow>();
            window.titleContent = new GUIContent("ModuleDownloadWindow");
            window.Show();
        }

        private void OnGUI() {
            
        }
    }
}
