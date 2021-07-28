using UnityEngine;
using UnityEditor;

/*===============================================================
Project:	Screen Handler
Developer:	Marci San Diego
Company:	Personal - marcisandiego@gmail.com
Date:       25/09/2019 19:08
===============================================================*/

namespace MSD.Modules.ScreenHandler.Editor
{
    [CustomEditor(typeof(ScreenKey))]
    public class ScreenKeyInspector : UnityEditor.Editor
    {
        private ScreenKey Target => target as ScreenKey;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			using (new EditorGUI.DisabledScope(!Target.IsRegistered)) {

				using (new EditorGUI.DisabledScope(Target.IsShown)) {
					if (GUILayout.Button("Show Screen")) {
						Target.ShowScreen();
					}
				}

				using (new EditorGUI.DisabledScope(!Target.IsShown)) {
					if (GUILayout.Button("Hide Screen")) {
						Target.HideScreen();
					}
				}
			}
		}
	}
}
