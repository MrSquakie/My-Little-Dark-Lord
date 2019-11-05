namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ConditionTag : ICondition
	{
        [TagSelector]
        public string conditionTag = "";
        public bool matchChildren = false;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
		{
            if (target == null) return false;
            return target.CompareTag(this.conditionTag);
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Tag";
		private const string NODE_TITLE = "Target has tag {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spConditionTag;
        private SerializedProperty spMatchChildren;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                (string.IsNullOrEmpty(this.conditionTag) ? "none" : this.conditionTag)
            );
		}

		protected override void OnEnableEditorChild ()
		{
			this.spConditionTag = this.serializedObject.FindProperty("conditionTag");
            this.spMatchChildren = this.serializedObject.FindProperty("matchChildren");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spConditionTag);
            EditorGUILayout.PropertyField(this.spMatchChildren);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
