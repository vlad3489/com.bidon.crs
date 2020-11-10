using UnityEditor;

namespace CRS
{
	[CustomEditor (typeof (SceneReaction))]
	public class SceneReactionEditor : ReactionEditor
	{
		protected override string GetFoldoutLabel ()
		{
			return "Scene Reaction";
		}
	}
}
