namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionAudioPause : IAction 
	{
        public BoolProperty pause = new BoolProperty(true);

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            AudioListener.pause = this.pause.GetValue(target);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Pause Audio";
		private const string NODE_TITLE = "Play/Pause audio";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spPause;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
			this.spPause = this.serializedObject.FindProperty("pause");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spPause = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spPause);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}