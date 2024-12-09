using UnityEngine;
using UnityEditor;

namespace CRS
{
	// This is the Editor for the Interactable MonoBehaviour.
	// However, since the Interactable contains many sub-objects, 
	// it requires many sub-editors to display them.
	// For more details, see the EditorWithSubEditors class.
	[CustomEditor(typeof(Interactable))]
	public class InteractableEditor : EditorWithSubEditors<ConditionCollectionEditor, ConditionCollection>
	{
		private Interactable _interactable; // Reference to the target.
		private SerializedProperty _interactionLocationProperty; // Represents the Transform which is where the player walks to in order to Interact with the Interactable.
		private SerializedProperty _collectionsProperty; // Represents the ConditionCollection array on the Interactable.
		private SerializedProperty _defaultReactionCollectionProperty; // Represents the ReactionCollection which is used if none of the ConditionCollections are.
		
		private const float COLLECTION_BUTTON_WIDTH = 125f; // Width in pixels of the button for adding to the ConditionCollection array.
		private const string INTERACTABLE_PROP_INTERACTION_LOCATION_NAME = "InteractionLocation"; // Name of the Transform field for where the player walks to in order to Interact with the Interactable.
		private const string INTERACTABLE_PROP_CONDITION_COLLECTIONS_NAME = "ConditionCollections"; // Name of the ConditionCollection array.
		private const string INTERACTABLE_PROP_DEFAULT_REACTION_COLLECTION_NAME = "DefaultReactionCollection"; // Name of the ReactionCollection field which is used if none of the ConditionCollections are.
		
		private void OnEnable()
		{
			// Cache the target reference.
			_interactable = (Interactable)target;

			// Cache the SerializedProperties.
			_collectionsProperty = serializedObject.FindProperty(INTERACTABLE_PROP_CONDITION_COLLECTIONS_NAME);
			_interactionLocationProperty = serializedObject.FindProperty(INTERACTABLE_PROP_INTERACTION_LOCATION_NAME);
			_defaultReactionCollectionProperty = serializedObject.FindProperty(INTERACTABLE_PROP_DEFAULT_REACTION_COLLECTION_NAME);

			// Create the necessary Editors for the ConditionCollections.
			CheckAndCreateSubEditors(_interactable.ConditionCollections);
		}

		private void OnDisable()
		{
			// When the InteractableEditor is disabled, destroy all the ConditionCollection editors.
			CleanupEditors();
		}
		
		// This is called when the ConditionCollection editors are created.
		protected override void SubEditorSetup(ConditionCollectionEditor editor)
		{
			// Give the ConditionCollection editor a reference to the array to which it belongs.
			editor.collectionsProperty = _collectionsProperty;
		}

		// Draw custom inspector
		public override void OnInspectorGUI()
		{
			// Pull information from the target into the serializedObject.
			serializedObject.Update();

			// Draw interaction handling buttons
			GUILayout.BeginHorizontal();
			if (Application.isPlaying)
			{
				if (GUILayout.Button("INTERACT"))
				{
					_interactable.Interact();
				}
			}

			if (GUILayout.Button("Focus on object!"))
			{
				EditorGUIUtility.PingObject(_interactable.gameObject.gameObject);
			}

			GUILayout.EndHorizontal();

			// If necessary, create editors for the ConditionCollections.
			CheckAndCreateSubEditors(_interactable.ConditionCollections);

			// Use the default object field GUI for the interactionLocation.
			EditorGUILayout.PropertyField(_interactionLocationProperty);

			EditorGUILayout.Space(20);

			// Display all the ConditionCollections.
			for (int i = 0; i < subEditors.Length; i++)
			{
				subEditors[i].OnInspectorGUI();
				EditorGUILayout.Space();
			}

			// Create a right-aligned button which when clicked, creates a new ConditionCollection in the ConditionCollections array.
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Collection", GUILayout.Width(COLLECTION_BUTTON_WIDTH)))
			{
				ConditionCollection newCollection = ConditionCollectionEditor.CreateConditionCollection();
				_collectionsProperty.AddToObjectArray(newCollection);
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			// Use the default object field GUI for the defaultReaction.
			EditorGUILayout.PropertyField(_defaultReactionCollectionProperty);

			// Push information back to the target from the serializedObject.
			serializedObject.ApplyModifiedProperties();
		}
	}
}