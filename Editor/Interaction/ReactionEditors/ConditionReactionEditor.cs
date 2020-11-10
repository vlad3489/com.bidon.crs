using UnityEditor;

namespace CRS
{
	[CustomEditor (typeof (ConditionReaction))]
	public class ConditionReactionEditor : ReactionEditor
	{
		private SerializedProperty _conditionProperty;       // Represents the Condition that will be changed.
		private SerializedProperty _satisfiedProperty;       // Represents the value that the Condition's satifised flag will be set to.


		// Name of the field which is the Condition that will be changed.
		private const string CONDITION_REACTION_PROP_CONDITION_NAME = "Condition";

		// Name of the bool field which is the value the Condition will get.
		private const string CONDITION_REACTION_PROP_SATISFIED_NAME = "IsSatisfied";


		protected override void Init ()
		{
			// Cache the SerializedProperties.
			_conditionProperty = serializedObject.FindProperty (CONDITION_REACTION_PROP_CONDITION_NAME);
			_satisfiedProperty = serializedObject.FindProperty (CONDITION_REACTION_PROP_SATISFIED_NAME);
		}


		protected override void DrawReaction ()
		{
			// If there's isn't a Condition yet, set it to the first Condition from the AllConditions array.
			if (_conditionProperty.objectReferenceValue == null)
				_conditionProperty.objectReferenceValue = AllConditionsEditor.TryGetConditionAt (0);

			// Get the index of the Condition in the AllConditions array.
			int index = AllConditionsEditor.TryGetConditionIndex ((Condition)_conditionProperty.objectReferenceValue);

			// Use and set that index based on a popup of all the descriptions of the Conditions.
			index = EditorGUILayout.Popup (index, AllConditionsEditor.AllConditionDescriptions);

			// Set the Condition based on the new index from the AllConditions array.
			_conditionProperty.objectReferenceValue = AllConditionsEditor.TryGetConditionAt (index);

			// Use default toggle GUI for the satisfied field.
			EditorGUILayout.PropertyField (_satisfiedProperty);
		}


		protected override string GetFoldoutLabel ()
		{
			return "Condition Reaction";
		}
	}
}