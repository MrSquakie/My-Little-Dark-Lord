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
	public class ActionPlaySound : IAction 
	{
		public AudioClip audioClip;
		public bool fadeIn = false;
		public float fadeInTime = 0.0f;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float time = (this.fadeIn ? this.fadeInTime : 0.0f);
            AudioManager.Instance.PlaySound2D(this.audioClip, time);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Play Sound";
		private const string NODE_TITLE = "Play Sound {0} {1}";

		private static readonly GUIContent GUICONTENT_AUDIOCLIP = new GUIContent("Audio Clip");
		private static readonly GUIContent GUICONTENT_FADEIN    = new GUIContent("Fade In?");
		private static readonly GUIContent GUICONTENT_FADETIME  = new GUIContent("Fade Time (s)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spAudioClip;
		private SerializedProperty spFadeIn;
		private SerializedProperty spFadeInTime;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
				(this.audioClip == null ? "none" : this.audioClip.name),
				(this.fadeIn ? "("+this.fadeInTime.ToString()+"s)" : "")
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spAudioClip = this.serializedObject.FindProperty("audioClip");
			this.spFadeIn = this.serializedObject.FindProperty("fadeIn");
			this.spFadeInTime = this.serializedObject.FindProperty("fadeInTime");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spAudioClip = null;
			this.spFadeIn = null;
			this.spFadeInTime = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spAudioClip, GUICONTENT_AUDIOCLIP);
			EditorGUILayout.PropertyField(this.spFadeIn, GUICONTENT_FADEIN);

			if (this.spFadeIn.boolValue)
			{
				EditorGUILayout.PropertyField(this.spFadeInTime, GUICONTENT_FADETIME);
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}