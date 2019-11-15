namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionPlaySound3D : IAction 
	{
		public AudioClip audioClip;
		public bool fadeIn = false;
		public float fadeInTime = 0.0f;

        [Range(0.0f, 1.0f)]
        public float spatialBlend = 0.85f;
        public NumberProperty pitch = new NumberProperty(1.0f);

        public TargetPosition position = new TargetPosition(TargetPosition.Target.Player);

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float time = (this.fadeIn ? this.fadeInTime : 0.0f);
            AudioManager.Instance.PlaySound3D(
                this.audioClip, 
                time, 
                this.position.GetPosition(target),
                this.spatialBlend,
                this.pitch.GetValue(target)
            );

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Play Sound 3D";
		private const string NODE_TITLE = "Play 3D Sound {0} {1}";

		private static readonly GUIContent GUICONTENT_AUDIOCLIP = new GUIContent("Audio Clip");
		private static readonly GUIContent GUICONTENT_FADEIN    = new GUIContent("Fade In?");
		private static readonly GUIContent GUICONTENT_FADETIME  = new GUIContent("Fade Time (s)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spAudioClip;
		private SerializedProperty spFadeIn;
		private SerializedProperty spFadeInTime;

        private SerializedProperty spSpatialBlend;
        private SerializedProperty spPitch;
        private SerializedProperty spPosition;

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

            this.spSpatialBlend = this.serializedObject.FindProperty("spatialBlend");
            this.spPitch = this.serializedObject.FindProperty("pitch");
            this.spPosition = this.serializedObject.FindProperty("position");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spAudioClip = null;
			this.spFadeIn = null;
			this.spFadeInTime = null;

            this.spSpatialBlend = null;
            this.spPitch = null;
            this.spPosition = null;
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

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spSpatialBlend);
            EditorGUILayout.PropertyField(this.spPitch);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spPosition);
            EditorGUILayout.Space();

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}