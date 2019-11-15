namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionStopMusic : IAction 
	{
		public bool fadeOut = false;
		public float fadeOutTime = 0.0f;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float time = (this.fadeOut ? this.fadeOutTime : 0.0f);
            AudioManager.Instance.StopMusic(time);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Stop Music";
		private const string NODE_TITLE = "Stop Music {0}";

		private static readonly GUIContent GUICONTENT_FADEOUT    = new GUIContent("Fade Out?");
		private static readonly GUIContent GUICONTENT_FADETIME  = new GUIContent("Fade Time (s)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spFadeOut;
		private SerializedProperty spFadeOutTime;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
				(this.fadeOut ? "("+this.fadeOutTime.ToString()+"s)" : "")
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spFadeOut = this.serializedObject.FindProperty("fadeOut");
			this.spFadeOutTime = this.serializedObject.FindProperty("fadeOutTime");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spFadeOut = null;
			this.spFadeOutTime = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spFadeOut, GUICONTENT_FADEOUT);
			if (this.spFadeOut.boolValue)
			{
				EditorGUILayout.PropertyField(this.spFadeOutTime, GUICONTENT_FADETIME);
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}