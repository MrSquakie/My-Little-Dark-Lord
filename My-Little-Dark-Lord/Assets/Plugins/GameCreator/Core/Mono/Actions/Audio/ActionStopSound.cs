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
	public class ActionStopSound : IAction 
	{
		public AudioClip audioClip;
		public bool fadeOut = false;
		public float fadeOutTime = 0.0f;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float time = (this.fadeOut ? this.fadeOutTime : 0.0f);
            AudioManager.Instance.StopSound2D(this.audioClip, time);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Stop Sound";
		private const string NODE_TITLE = "Stop Sound {0} {1}";

		private static readonly GUIContent GUICONTENT_AUDIOCLIP = new GUIContent("Audio Clip");
		private static readonly GUIContent GUICONTENT_FADEOUT   = new GUIContent("Fade Out?");
		private static readonly GUIContent GUICONTENT_FADETIME  = new GUIContent("Fade Time (s)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spAudioClip;
		private SerializedProperty spFadeOut;
		private SerializedProperty spFadeOutTime;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
				(this.audioClip == null ? "unknown" : this.audioClip.name),
				(this.fadeOut ? "("+this.fadeOutTime.ToString()+"s)" : "")
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spAudioClip = this.serializedObject.FindProperty("audioClip");
			this.spFadeOut = this.serializedObject.FindProperty("fadeOut");
			this.spFadeOutTime = this.serializedObject.FindProperty("fadeOutTime");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spAudioClip = null;
			this.spFadeOut = null;
			this.spFadeOutTime = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spAudioClip, GUICONTENT_AUDIOCLIP);
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