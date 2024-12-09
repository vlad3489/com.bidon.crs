using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace CRS
{
	[CustomEditor(typeof(AllConditions))]
	public class AllConditionsEditor : Editor
	{
		private const string DEFAULT_CONDITION_NAME = "New condition name";
		private ConditionEditor[] _conditionEditors; // All the subEditors to display the Conditions.
		private AllConditions _allConditions; // Reference to the target.
		private string _newConditionName; // String to start off the naming of new Conditions.
		private bool _isFoldoutedOriginalList = false;

		private ReorderableList _reordableList;
		private SerializedProperty _conditionsProperty;
		private SerializedProperty _levelsConditionList;
		private bool _isInRenamingState;

		private static string[] _allConditionDescriptions; // Field to store the descriptions of all the Conditions.

		private const string CREATION_PATH = "Assets/Resources/AllConditions.asset"; // The path that the AllConditions asset is created at.
		private const float BUTTON_WIDTH = 30f; // Width in pixels of the button to create Conditions.

		// Property for accessing the descriptions for all the Conditions.
		// This is used for the Popups on the ConditionEditor.
		public static string[] AllConditionDescriptions
		{
			get
			{
				// If the description array doesn't exist yet, set it.
				if (_allConditionDescriptions == null)
				{
					SetAllConditionDescriptions();
				}

				return _allConditionDescriptions;
			}
			private set { _allConditionDescriptions = value; }
		}

		private void OnEnable()
		{
			// Cache the reference to the target.
			_allConditions = (AllConditions)target;

			// If there aren't any Conditions on the target, create an empty array of Conditions.
			if (_allConditions.Conditions == null)
			{
				_allConditions.Conditions = new Condition[0];
			}

			// If there aren't any editors, create them.
			if (_conditionEditors == null)
			{
				CreateEditors();
			}

			_conditionsProperty = serializedObject.FindProperty("Conditions");
			_levelsConditionList = serializedObject.FindProperty("LevelsCondition");

			_reordableList = new ReorderableList(serializedObject, _conditionsProperty, true, true, false, false);
			_reordableList.drawElementCallback = DrawConditionItem;
			_reordableList.drawHeaderCallback = DrawHeader;
			_reordableList.elementHeight = EditorGUIUtility.singleLineHeight * 2;
		}

		private void OnDisable()
		{
			// Destroy all the editors.
			for (int i = 0; i < _conditionEditors.Length; i++)
			{
				DestroyImmediate(_conditionEditors[i]);
			}
			
			// Null out the editor array.
			_conditionEditors = null;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (GUILayout.Button("Reset all conditions!", GUILayout.Height(30)))
			{
				_allConditions.Reset();
			}

			GUILayout.Space(20);

			// Display all the conditions as Reordable list.
			_reordableList.DoLayoutList();

			GUILayout.Space(20);


			GUILayout.Label("Create new condition:");

			EditorGUILayout.BeginHorizontal();

			// Get and display a string for the name of a new Condition.
			_newConditionName = EditorGUILayout.TextField(GUIContent.none, string.IsNullOrEmpty(_newConditionName) ? DEFAULT_CONDITION_NAME : _newConditionName);

			// Display a button that when clicked adds a new Condition to the AllConditions asset and resets the new description string.
			if (GUILayout.Button("+", GUILayout.Width(BUTTON_WIDTH)))
			{
				AddCondition(_newConditionName);
				_newConditionName = DEFAULT_CONDITION_NAME;
			}

			EditorGUILayout.EndHorizontal();

			// If there are conditions, add a gap.
			if (TryGetConditionsLength() > 0)
			{
				EditorGUILayout.Space(20);
			}
			
			EditorGUILayout.Space(40);

			// Display levels group conditions
			EditorGUILayout.PropertyField(_levelsConditionList, new GUIContent("Level condition groups"));

			if (_allConditions.LevelsCondition.Length > 0 && _allConditions.LevelsCondition[0] != null)
			{
				for (int i = 0; i < _allConditions.LevelsCondition.Length; i++)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);

					_isFoldoutedOriginalList = EditorGUILayout.Foldout(_isFoldoutedOriginalList, _allConditions.LevelsCondition[i].TitleName);

					if (_isFoldoutedOriginalList)
					{
						// Display the Labels for the Conditions evenly split over the width of the inspector.
						float space = EditorGUIUtility.currentViewWidth / 4f;
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Condition name", GUILayout.Width(space + 20f));
						EditorGUILayout.LabelField("IsSatisfied?", GUILayout.Width(space));
						EditorGUILayout.LabelField("Default", GUILayout.Width(space));
						EditorGUILayout.LabelField("Hash", GUILayout.Width(space));
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space(10);

						for (int j = 0; j < _allConditions.LevelsCondition[i].Conditions.Length; j++)
						{
							var condition = _allConditions.LevelsCondition[i].Conditions[j];
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(condition.name, GUILayout.Width(space + 20f));
							EditorGUILayout.Toggle(condition.IsSatisfied, GUILayout.Width(space));
							EditorGUILayout.Toggle(condition.DefaultValue, GUILayout.Width(space));
							EditorGUILayout.LabelField(condition.Hash.ToString(), GUILayout.Width(space));

							EditorGUILayout.EndHorizontal();
						}
					}

					EditorGUILayout.EndVertical();

					if (_isFoldoutedOriginalList)
					{
						EditorGUILayout.Space(20);
					}
				}
			}
			
			EditorGUILayout.Space(40);

			
			// // Display conditions using subetitors
			// // If there are different number of editors to Conditions, create them afresh.
			// if (_conditionEditors.Length != TryGetConditionsLength())
			// {
			// 	// Destroy all the old editors.
			// 	foreach (var editor in _conditionEditors)
			// 	{
			// 		DestroyImmediate(editor);
			// 	}
			//
			// 	// Create new editors.
			// 	CreateEditors();
			// }
			//
			// // Display all the conditions. (In old way)
			// _isFoldoutedOriginalList = EditorGUILayout.Foldout(_isFoldoutedOriginalList, "All Conditions (display in original way)");
			// if (_isFoldoutedOriginalList)
			// {
			// 	for (int i = 0; i < _conditionEditors.Length; i++)
			// 	{
			// 		_conditionEditors[i].OnInspectorGUI();
			// 	}
			// }

			
			serializedObject.ApplyModifiedProperties();
		}

		// Draw reordable list header
		private void DrawHeader(Rect rect)
		{
			string headerName = "Al conditions list";
			EditorGUI.LabelField(rect, headerName);
		}

		// Draw each item in reordable list
		private void DrawConditionItem(Rect rect, int index, bool isActive, bool isFocused)
		{
			Rect nameRect = new Rect(rect.x, rect.y, 160, EditorGUIUtility.singleLineHeight * 2f);

			float gap = 5f;
			int numColumns = 5;
			float width = ((rect.width - nameRect.width) - (numColumns - 1) * gap) / numColumns;
			// rect.height = EditorGUIUtility.singleLineHeight*2;
			rect.width = width;

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;

			if (!_isInRenamingState)
			{
				string labelName = $"{_allConditions.Conditions[index].name}\n <color=#7F7F7F>Hash: {_allConditions.Conditions[index].Hash}</color>";
				EditorGUI.LabelField(nameRect, labelName, style);
				// rect.x += nameRect.width + gap;
				rect.x += rect.width + 90f + gap;

				var currentValueField = new Rect(rect.x, rect.y, 70f, EditorGUIUtility.singleLineHeight * 2);
				EditorGUI.LabelField(currentValueField, "IsSatisfied)");
				rect.x += currentValueField.width + gap;
				var newIsSatisfiedRectOffset = rect;
				// newIsSatisfiedRectOffset.y -= 7f;
				_allConditions.Conditions[index].IsSatisfied = EditorGUI.Toggle(newIsSatisfiedRectOffset, _allConditions.Conditions[index].IsSatisfied);

				rect.x += rect.width - 50f + gap;

				var defaultValueField = new Rect(rect.x, rect.y, 50f, EditorGUIUtility.singleLineHeight * 2);
				EditorGUI.LabelField(defaultValueField, "Default:");
				rect.x += defaultValueField.width + gap;

				var newDefaultValueRectOffset = rect;
				// newDefaultValueRectOffset.y -= 7f;
				_allConditions.Conditions[index].DefaultValue = EditorGUI.Toggle(newDefaultValueRectOffset, _allConditions.Conditions[index].DefaultValue);

				rect.x += rect.width - 20f + gap;


				if (GUI.Button(rect, "Rename"))
				{
					_isInRenamingState = true;
				}

				rect.x += rect.width + gap;

				// Delete button
				if (GUI.Button(rect, "-"))
				{
					RemoveCondition(_allConditions.Conditions[index]);
				}

				rect.x += rect.width + gap;
			}
			else
			{
				_allConditions.Conditions[index].Description = EditorGUI.TextField(nameRect, _allConditions.Conditions[index].Description);

				rect.x += nameRect.width + gap;
				if (GUI.Button(rect, "Save"))
				{
					_allConditions.Conditions[index].name = _allConditions.Conditions[index].Description;
					AssetDatabase.SaveAssets();
					_isInRenamingState = false;
				}
			}
		}
		
		private static void SetAllConditionDescriptions()
		{
			// Create a new array that has the same number of elements as there are Conditions.
			AllConditionDescriptions = new string[TryGetConditionsLength()];

			// Go through the array and assign the description of the condition at the same index.
			for (int i = 0; i < AllConditionDescriptions.Length; i++)
			{
				AllConditionDescriptions[i] = TryGetConditionAt(i).Description;
			}
		}
		
		private void CreateEditors ()
		{
			// Create a new array for the editors which is the same length at the conditions array.
			_conditionEditors = new ConditionEditor[_allConditions.Conditions.Length];

			// Go through all the empty array...
			for (int i = 0; i < _conditionEditors.Length; i++)
			{
				// ... and create an editor with an editor type to display correctly.
				_conditionEditors[i] = CreateEditor (TryGetConditionAt (i)) as ConditionEditor;
				_conditionEditors[i].editorType = ConditionEditor.EditorType.AllConditionAsset;
			}
		}

		private void AddCondition(string description)
		{
			// If there isn't an AllConditions instance yet, put a message in the console and return.
			if (!AllConditions.Instance)
			{
				Debug.LogError("AllConditions has not been created yet.");
				return;
			}

			// Create a condition based on the description.
			Condition newCondition = ConditionEditor.CreateCondition(description);

			// The name is what is displayed by the asset so set that too.
			newCondition.name = description;

			// Record all operations on the newConditions so they can be undone.
			Undo.RecordObject(newCondition, "Created new Condition");

			// Attach the Condition to the AllConditions asset.
			AssetDatabase.AddObjectToAsset(newCondition, AllConditions.Instance);

			// Import the asset so it is recognised as a joined asset.
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newCondition));

			// Add the Condition to the AllConditions array.
			ArrayUtility.Add(ref AllConditions.Instance.Conditions, newCondition);

			// Mark the AllConditions asset as dirty so the editor knows to save changes to it when a project save happens.
			EditorUtility.SetDirty(AllConditions.Instance);

			// Recreate the condition description array with the new added Condition.
			SetAllConditionDescriptions();
		}

		public static void RemoveCondition(Condition condition)
		{
			// If there isn't an AllConditions asset, do nothing.
			if (!AllConditions.Instance)
			{
				Debug.LogError("AllConditions has not been created yet.");
				return;
			}

			// Record all operations on the AllConditions asset so they can be undone.
			Undo.RecordObject(AllConditions.Instance, "Removing condition");

			// Remove the specified condition from the AllConditions array.
			ArrayUtility.Remove(ref AllConditions.Instance.Conditions, condition);

			// Destroy the condition, including its asset and save the assets to recognize the change.
			DestroyImmediate(condition, true);
			AssetDatabase.SaveAssets();

			// Mark the AllConditions asset as dirty so the editor knows to save changes to it when a project save happens.
			EditorUtility.SetDirty(AllConditions.Instance);

			// Recreate the condition description array without the removed condition.
			SetAllConditionDescriptions();
		}

		public static int TryGetConditionIndex(Condition condition)
		{
			// Go through all the Conditions...
			for (int i = 0; i < TryGetConditionsLength(); i++)
			{
				// ... and if one matches the given Condition, return its index.
				if (TryGetConditionAt(i).Hash == condition.Hash)
				{
					return i;
				}
			}

			// If the Condition wasn't found, return -1.
			return -1;
		}

		public static Condition TryGetConditionAt(int index)
		{
			// Cache the AllConditions array.
			Condition[] allConditions = AllConditions.Instance.Conditions;

			// If it doesn't exist or there are null elements, return null.
			if (allConditions == null || allConditions.Length == 0 || allConditions[0] == null)
			{
				return null;
			}

			// If the given index is beyond the length of the array return the first element.
			if (index >= allConditions.Length)
			{
				return allConditions[0];
			}

			// Otherwise return the Condition at the given index.
			return allConditions[index];
		}

		public static int TryGetConditionsLength()
		{
			return AllConditions.Instance.Conditions == null ? 0 : AllConditions.Instance.Conditions.Length;
		}

		// Call this function when the menu item is selected.
		[MenuItem("Assets/Create/AllConditions")]
		private static void CreateAllConditionsAsset()
		{
			// Create an instance of the AllConditions object and make an asset for it.
			AllConditions instance = CreateInstance<AllConditions>();
			AssetDatabase.CreateAsset(instance, CREATION_PATH);

			// Set this as the singleton instance.
			AllConditions.Instance = instance;

			// Create a new empty array of Conditions.
			instance.Conditions = new Condition[0];
		}
	}
}