using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace CRS
{
	// This is the Editor for the ReactionCollection MonoBehaviour.
	// However, since the ReactionCollection contains many Reactions, 
	// it requires many sub-editors to display them.
	// For more details see the EditorWithSubEditors class.
	// There are two ways of adding Reactions to the ReactionCollection:
	// a type selection popup with confirmation button and a drag and drop
	// area.  Details on these are found below.
	[CustomEditor (typeof (ReactionCollection))]
	public class ReactionCollectionEditor : EditorWithSubEditors<ReactionEditor, Reaction>
	{
		private ReactionCollection _reactionCollection;                     // Reference to the target.
		private SerializedProperty _reactionsProperty;                      // Represents the array of Reactions.

		private Type[] _reactionTypes;                                      // All the non-abstract types which inherit from Reaction.  This is used for adding new Reactions.
		private string[] _reactionTypeNames;                                // The names of all appropriate Reaction types.
		private int _selectedIndex;                                         // The index of the currently selected Reaction type.



		private ReorderableList _reordableList;                             // Sorting orders list
		private SerializedProperty _reactionsListProperty;                  // Serialized property to comunicate with editor
		private bool _isFoldoutedOrderList = false;                         // Foldout list

		private SerializedProperty _useDebugProperties;                     // Flag use additional debug options
		private SerializedProperty _pauseEditorProperty;
		private SerializedProperty _printExecutedReactionCollectionProperty;

		// Caching the vertical spacing between GUI elements.
		private float _verticalSpacing;
		private float _lineHeight;

		private const float DROP_HEIGHT = 50f;                              // Height in pixels of the area for dropping scripts.
		private const float CONTROL_SPACING = 5f;                           // Width in pixels between the popup type selection and drop area.
		private const string REACTIONS_PROP_NAME = "Reactions";             // Name of the field for the array of Reactions.
		private const string REACTIONS_LIST_PROP_NAME = "ReactionsList";    // Name of the field for the array of Reactions.
		private const string REACTIONS_DEBUG_PROP_NAME = "UseDebugOptions";
		private const string PAUSE_EDITOR_DEBUG_PROP_NAME = "PauseEditorOnReactionCollection";
		private const string PRINT_REACTION_COLLECTION_DEBUG_PROP_NAME = "PrintExecutedReactionCollection";

		private const string ITEM_PREFIX = "• ";


		private void OnEnable ()
		{
			// Cache the target.
			_reactionCollection = (ReactionCollection)target;

			// Cache the vertical spacing
			_verticalSpacing = EditorGUIUtility.standardVerticalSpacing;

			// Cache the line height
			_lineHeight = EditorGUIUtility.singleLineHeight;

			// Cache the SerializedProperties
			_reactionsProperty = serializedObject.FindProperty (REACTIONS_PROP_NAME);
			_reactionsListProperty = serializedObject.FindProperty (REACTIONS_LIST_PROP_NAME);
			_useDebugProperties = serializedObject.FindProperty (REACTIONS_DEBUG_PROP_NAME);
			_pauseEditorProperty = serializedObject.FindProperty (PAUSE_EDITOR_DEBUG_PROP_NAME);
			_printExecutedReactionCollectionProperty = serializedObject.FindProperty (PRINT_REACTION_COLLECTION_DEBUG_PROP_NAME);


			// If new editors are required for Reactions, create them.
			CheckAndCreateSubEditors (_reactionCollection.Reactions);

			// Set the array of types and type names of subtypes of Reaction.
			SetReactionNamesArray ();


			// Populate for reorder list
			PopulateReactionsList ();

			_reordableList = new ReorderableList (serializedObject, _reactionsListProperty, true, false, false, false);
			_reordableList.drawElementCallback = DrawListItems;
			_reordableList.onReorderCallback = OnReordableCallback;
		}

		private void OnDisable ()
		{
			// Destroy all the subeditors.
			CleanupEditors ();
		}

		private void PopulateReactionsList ()
		{
			_reactionCollection.ReactionsList = _reactionCollection.Reactions.ToList ();
		}

		// Reordable list
		private void DrawListItems (Rect rect, int index, bool isActive, bool isFocused)
		{
			string displayName = _reactionCollection.Reactions[index].GetType ().ToString ().Split ('.').Last ();
			EditorGUI.LabelField (new Rect (rect.x, rect.y, 200, _lineHeight), $"{displayName}");
		}

		// On Reordable list change
		private void OnReordableCallback (ReorderableList reordableList)
		{
			// Update array
			_reactionCollection.Reactions = _reactionCollection.ReactionsList.ToArray ();
			OnDisable ();
			OnEnable ();
		}


		// This is called immediately after each ReactionEditor is created.
		protected override void SubEditorSetup (ReactionEditor editor)
		{
			// Make sure the ReactionEditors have a reference to the array that contains their targets.
			editor.reactionsProperty = _reactionsProperty;
			editor.reactionsListProperty = _reactionsListProperty;
		}


		public override void OnInspectorGUI ()
		{
			// DrawDefaultInspector ();

			// Pull all the information from the target into the serializedObject.
			serializedObject.Update ();

			Rect defaultsRect = EditorGUILayout.BeginVertical ();
			GUI.Box (defaultsRect, /* No label. */"");

			// Draw tooltip buttons for handling reaction
			EditorGUILayout.HelpBox ("Reaction is multiple actions based on Conditions", MessageType.Info);

			// Draw reaction handling buttons
			GUILayout.BeginHorizontal ();
			if (Application.isPlaying)
			{
				if (GUILayout.Button ("REACT"))
				{
					_reactionCollection.React ();
				}
			}

			if (GUILayout.Button ("Focus on object!"))
			{
				EditorGUIUtility.PingObject (_reactionCollection.gameObject.gameObject);
			}

			GUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();


			// Draw description
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.LabelField ("Description:");
			_reactionCollection.Description = EditorGUILayout.TextArea (_reactionCollection.Description, GUILayout.Height (_lineHeight * 3));
			EditorGUILayout.EndVertical ();


			// If new editors for Reactions are required, create them.
			CheckAndCreateSubEditors (_reactionCollection.Reactions);

			// Display all the Reactions.
			for (int i = 0; i < subEditors.Length; i++)
			{
				subEditors[i].OnInspectorGUI ();
			}

			// Show paste button
			DisplayPasteButton ();

			// If there are Reactions, add a space.
			if (_reactionCollection.Reactions.Length > 0)
			{
				EditorGUILayout.Space (20);
			}

			// Create a Rect for the full width of the inspector with enough height for the drop area.
			Rect fullWidthRect = GUILayoutUtility.GetRect (GUIContent.none, GUIStyle.none, GUILayout.Height (DROP_HEIGHT + _verticalSpacing));

			// Create a Rect for the left GUI controls.
			Rect leftAreaRect = fullWidthRect;

			// It should be in half a space from the top.
			leftAreaRect.y += _verticalSpacing * 0.5f;

			// The width should be slightly less than half the width of the inspector.
			leftAreaRect.width *= 0.5f;
			leftAreaRect.width -= CONTROL_SPACING * 0.5f;

			// The height should be the same as the drop area.
			leftAreaRect.height = DROP_HEIGHT;

			// Create a Rect for the right GUI controls that is the same as the left Rect except...
			Rect rightAreaRect = leftAreaRect;

			// ... it should be on the right.
			rightAreaRect.x += rightAreaRect.width + CONTROL_SPACING;

			// Display the GUI for the type popup and button on the left.
			TypeSelectionGUI (leftAreaRect);

			// Display the GUI for the drag and drop area on the right.
			DragAndDropAreaGUI (rightAreaRect);

			// Manage the events for dropping on the right area.
			DraggingAndDropping (rightAreaRect, this);

			// Draw reordable list .DoLayoutList ();
			DrawReordableListGUI ();

			DisplayDebugOptions ();

			// Push the information back from the serializedObject to the target.
			serializedObject.ApplyModifiedProperties ();
		}


		private void DisplayPasteButton ()
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();

			GUIStyle centredStyle = GUI.skin.button;
			centredStyle.alignment = TextAnchor.MiddleCenter;
			centredStyle.normal.textColor = GUI.skin.button.normal.textColor;

			// Paste copied reaction from clipboard
			if (GUILayout.Button ("Paste copied\n reaction from clipboard", centredStyle, GUILayout.Height (40)))
			{
				try
				{
					string bufferValue = EditorGUIUtility.systemCopyBuffer;
					var obj = EditorUtility.InstanceIDToObject (Int32.Parse (bufferValue)) as Reaction;

					if (obj != null)
					{
						Reaction reactionCopy = Instantiate (obj);
						_reactionsProperty.AddToObjectArray (reactionCopy);
						_reactionsListProperty.AddToObjectArray (reactionCopy);
					}
					else
					{
						Debug.LogWarning ("Can't paste object! Copied object is not derived from reaction");
					}
				}
				catch (FormatException)
				{
					Debug.LogWarning ("Paste object error! Wrong Instance ID format");
				}
			}

			GUILayout.EndHorizontal ();
		}


		private void TypeSelectionGUI (Rect containingRect)
		{
			// Create Rects for the top and bottom half.
			Rect topHalf = containingRect;
			topHalf.height *= 0.5f;
			Rect bottomHalf = topHalf;
			bottomHalf.y += bottomHalf.height;

			// Display a popup in the top half showing all the reaction types.
			_selectedIndex = EditorGUI.Popup (topHalf, _selectedIndex, _reactionTypeNames);

			// Display a button in the bottom half that if clicked...
			if (GUI.Button (bottomHalf, "Add Selected Reaction"))
			{
				// ... finds the type selected by the popup, creates an appropriate reaction and adds it to the array.
				Type reactionType = _reactionTypes[_selectedIndex];
				Reaction newReaction = ReactionEditor.CreateReaction (reactionType);
				_reactionsProperty.AddToObjectArray (newReaction);
				_reactionsListProperty.AddToObjectArray (newReaction);
			}
		}


		private static void DragAndDropAreaGUI (Rect containingRect)
		{
			// Create a GUI style of a box but with middle aligned text and button text color.
			GUIStyle centredStyle = GUI.skin.box;
			centredStyle.alignment = TextAnchor.MiddleCenter;
			centredStyle.normal.textColor = GUI.skin.button.normal.textColor;

			// Draw a box over the area with the created style.
			GUI.Box (containingRect, "Drop new Reactions script here", centredStyle);
		}


		private static void DraggingAndDropping (Rect dropArea, ReactionCollectionEditor editor)
		{
			// Cache the current event.
			Event currentEvent = Event.current;

			// If the drop area doesn't contain the mouse then return.
			if (!dropArea.Contains (currentEvent.mousePosition))
				return;

			switch (currentEvent.type)
			{
				// If the mouse is dragging something...
				case EventType.DragUpdated:

					// ... change whether or not the drag *can* be performed by changing the visual mode of the cursor based on the IsDragValid function.
					DragAndDrop.visualMode = IsDragValid () ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;

					// Make sure the event isn't used by anything else.
					currentEvent.Use ();

					break;

				// If the mouse was dragging something and has released...
				case EventType.DragPerform:

					// ... accept the drag event.
					DragAndDrop.AcceptDrag ();

					// Go through all the objects that were being dragged...
					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						// ... and find the script asset that was being dragged...
						MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;

						// ... then find the type of that Reaction...
						Type reactionType = script.GetClass ();

						// ... and create a Reaction of that type and add it to the array.
						Reaction newReaction = ReactionEditor.CreateReaction (reactionType);
						editor._reactionsProperty.AddToObjectArray (newReaction);
					}

					// Make sure the event isn't used by anything else.
					currentEvent.Use ();

					break;
			}
		}


		private static bool IsDragValid ()
		{
			// Go through all the objects being dragged...
			for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
			{
				// ... and if any of them are not script assets, return that the drag is invalid.
				if (DragAndDrop.objectReferences[i].GetType () != typeof (MonoScript))
					return false;

				// Otherwise find the class contained in the script asset.
				MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;
				Type scriptType = script.GetClass ();

				// If the script does not inherit from Reaction, return that the drag is invalid.
				if (!scriptType.IsSubclassOf (typeof (Reaction)))
					return false;

				// If the script is an abstract, return that the drag is invalid.
				if (scriptType.IsAbstract)
					return false;
			}

			// If none of the dragging objects returned that the drag was invalid, return that it is valid.
			return true;
		}


		private void SetReactionNamesArray ()
		{
			// Store the Reaction type.
			Type reactionType = typeof (Reaction);

			// Get all the types that are in the same Assembly (all the runtime scripts) as the Reaction type.
			Type[] allTypes = reactionType.Assembly.GetTypes ();

			// Create an empty list to store all the types that are subtypes of Reaction.
			List<Type> reactionSubTypeList = new List<Type> ();

			// Go through all the types in the Assembly...
			for (int i = 0; i < allTypes.Length; i++)
			{
				// ... and if they are a non-abstract subclass of Reaction then add them to the list.
				if (allTypes[i].IsSubclassOf (reactionType) && !allTypes[i].IsAbstract)
				{
					reactionSubTypeList.Add (allTypes[i]);
				}
			}

			// Convert the list to an array and store it.
			_reactionTypes = reactionSubTypeList.ToArray ();

			// Create an empty list of strings to store the names of the Reaction types.
			List<string> reactionTypeNameList = new List<string> ();

			// Go through all the Reaction types and add their names to the list.
			for (int i = 0; i < _reactionTypes.Length; i++)
			{
				reactionTypeNameList.Add (_reactionTypes[i].Name);
			}

			// Convert the list to an array and store it.
			_reactionTypeNames = reactionTypeNameList.ToArray ();
		}


		private void DrawReordableListGUI ()
		{
			EditorGUILayout.Space ();
			_isFoldoutedOrderList = EditorGUILayout.Foldout (_isFoldoutedOrderList, "Reactions order");
			if (_isFoldoutedOrderList)
			{
				_reordableList.DoLayoutList ();
			}
		}


		private void DisplayDebugOptions ()
		{
			EditorGUILayout.Space ();

			// Show/Hide Additions options
			EditorGUILayout.PropertyField (_useDebugProperties);

			if (!_useDebugProperties.boolValue)
			{
				GUI.enabled = false;
			}

			//EditorGUI.indentLevel++;

			Rect optionsRect = EditorGUILayout.BeginVertical ();
			optionsRect = EditorGUI.IndentedRect (optionsRect);
			GUI.Box (optionsRect, "");
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (_pauseEditorProperty, new GUIContent (ITEM_PREFIX + "Pause Editor", "Pause editor on reache this reaction collection"));
			EditorGUILayout.PropertyField (_printExecutedReactionCollectionProperty, new GUIContent (ITEM_PREFIX + "Print reaction", "Print reaction collection"));

			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical ();

			//EditorGUI.indentLevel--;

			if (!_useDebugProperties.boolValue)
			{
				GUI.enabled = true;
			}
		}
	}
}