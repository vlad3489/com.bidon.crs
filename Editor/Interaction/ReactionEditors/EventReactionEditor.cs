using UnityEditor;

namespace CRS
{
	[CustomEditor (typeof (EventReaction))]
	public class EventReactionEditor : ReactionEditor
	{
		protected override string GetFoldoutLabel () => $"Event reaction";
	}
}
