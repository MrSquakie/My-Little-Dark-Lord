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
	public class ActionVolume : IAction 
	{
		public bool changeMusicVolume = true;
		public bool changeSoundVolume = true;

        public NumberProperty volume = new NumberProperty(1.0f);

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float value = this.volume.GetValue(target);

            if (this.changeMusicVolume) AudioManager.Instance.SetGlobalMusicVolume(value);
            if (this.changeSoundVolume) AudioManager.Instance.SetGlobalSoundVolume(value);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Change Volume";
		private const string NODE_TITLE = "Change {0} volume to {1}";

		private static readonly GUIContent GUICONTENT_CHANGE_MUSIC = new GUIContent("Change Music Volume?");
		private static readonly GUIContent GUICONTENT_CHANGE_SOUND = new GUIContent("Change Sound Volume?");
		private static readonly GUIContent GUICONTENT_VOLUME = new GUIContent("Volume");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spChangleMusicVolume;
		private SerializedProperty spChangleSoundVolume;
		private SerializedProperty spVolume;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			string types = "";
			if (this.changeMusicVolume && this.changeSoundVolume) types = "music and sound";
			else if (this.changeMusicVolume) types = "music";
			else if (this.changeSoundVolume) types = "sound";
			else types = "nothing";

			return string.Format(NODE_TITLE, types, this.volume);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spChangleMusicVolume = this.serializedObject.FindProperty("changeMusicVolume");
			this.spChangleSoundVolume = this.serializedObject.FindProperty("changeSoundVolume");
			this.spVolume = this.serializedObject.FindProperty("volume");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spChangleMusicVolume = null;
			this.spChangleSoundVolume = null;
			this.spVolume = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spChangleMusicVolume, GUICONTENT_CHANGE_MUSIC);
			EditorGUILayout.PropertyField(this.spChangleSoundVolume, GUICONTENT_CHANGE_SOUND);
			EditorGUILayout.PropertyField(this.spVolume, GUICONTENT_VOLUME);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}