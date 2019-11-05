namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	// --------------------------------------------------------------------------------------------

	public class VariableAttribute : PropertyAttribute
	{
		public string controlName;
		public List<string> suggestions;
		public string suggestionsSeed;
		public GUIContent suggestedVariables;

		public VariableAttribute()
		{
			this.controlName = this.GenerateRandomString();
			this.suggestions = new List<string>();
			this.suggestionsSeed = "";

			this.suggestedVariables = new GUIContent("Suggested Variables");
		}

		private string GenerateRandomString()
		{
			string alphabet = "abcdefghijklmnopqrstuvwxyz1234567890";
			char[] characters = new char[8];
			System.Random random = new System.Random();

			for (int i = 0; i < characters.Length; i++)
			{
				characters[i] = alphabet[random.Next(alphabet.Length)];
			}

			return characters.ToString();
		}
	}

	// --------------------------------------------------------------------------------------------

	public class LocStringNoTextAttribute : PropertyAttribute
	{
		public LocStringNoTextAttribute() {}
	}

    public class LocStringNoPostProcessAttribute : PropertyAttribute
    {
        public LocStringNoPostProcessAttribute() { }
    }

	public class LocStringBigTextAttribute : PropertyAttribute
    {
        public LocStringBigTextAttribute() {}
    }

    public class LocStringBigTextNoPostProcessAttribute : PropertyAttribute
    {
        public LocStringBigTextNoPostProcessAttribute() { }
    }

	// --------------------------------------------------------------------------------------------

	public class RotationConstraintAttribute : PropertyAttribute
	{
		public RotationConstraintAttribute() {}
	}

    public class TagSelectorAttribute : PropertyAttribute
    {
        public TagSelectorAttribute() {}
    }

    public class LayerSelectorAttribute : PropertyAttribute
    {
        public LayerSelectorAttribute() {}
    }

    public class IndentAttribute : PropertyAttribute
    {
        public IndentAttribute() { }
    }
}
