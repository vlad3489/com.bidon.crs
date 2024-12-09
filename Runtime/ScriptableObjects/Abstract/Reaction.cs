using UnityEngine;

namespace CRS
{
	// This is the base class for all Reactions.
	// There are arrays of inheriting Reactions on ReactionCollections.
	public abstract class Reaction : ScriptableObject
	{
		// This is called from ReactionCollection.
		// This function contains everything required to be done for all
		// Reactions as well.
		public virtual void Init()
		{
		}

		// This function is called from ReactionCollection.
		// It contains everything required for all Reactions as
		// well as the part of the Reaction which needs to happen immediately.
		public virtual void React(MonoBehaviour monoBehaviour)
		{
			ImmediateReaction();
		}

		// This is the core of the Reaction and must be overridden to make things happpen.
		protected abstract void ImmediateReaction();
	}
}