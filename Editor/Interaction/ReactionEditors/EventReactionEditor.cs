using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CRS
{
	[CustomEditor (typeof (EventReaction))]
	public class EventReactionEditor : ReactionEditor
	{
		protected override string GetFoldoutLabel () => $"Event reaction";
	}
}
