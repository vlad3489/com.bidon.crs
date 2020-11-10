using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CRS
{
	[CustomEditor (typeof (TextReaction))]
	public class TextReactionEditor : ReactionEditor
	{
		private SerializedProperty _messageProperty;
		private SerializedProperty _messageOutputField;

		private const string TEXT_REACTION_PROPERTY_MESSAGE_NAME = "Message";
		private const string TEXT_REACTION_PROPERTY_MESSAGE_OUTPUT_FIELD_NAME = "TextField";
		private const float AREA_WIDTH_OFFSET = 20f;
		private const float MESSAGE_GUI_LINES = 3f;

		protected override string GetFoldoutLabel () => "Text Reaction";

		protected override void Init ()
		{
			_messageProperty = serializedObject.FindProperty (TEXT_REACTION_PROPERTY_MESSAGE_NAME);
			_messageOutputField = serializedObject.FindProperty (TEXT_REACTION_PROPERTY_MESSAGE_OUTPUT_FIELD_NAME);
		}

		protected override void DrawReaction ()
		{
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Message", GUILayout.Width (EditorGUIUtility.labelWidth - AREA_WIDTH_OFFSET));
			_messageProperty.stringValue = EditorGUILayout.TextArea (_messageProperty.stringValue, GUILayout.Height (EditorGUIUtility.singleLineHeight * MESSAGE_GUI_LINES));

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.PropertyField (_messageOutputField);
		}
	}
}
