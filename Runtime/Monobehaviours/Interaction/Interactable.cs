using UnityEngine;

namespace CRS
{
	// This is one of the core features of the game.
	// Each one acts like a hub for all things that transpire
	// over the course of the game.
	// The script must be on a game object with a collider and
	// an event trigger. The event trigger should tell the
	// player to approach the interaction Location,
	// and the player should call the Interact function when they arrive.
	public class Interactable : MonoBehaviour
	{
		public Transform InteractionLocation;											// The position and rotation the player should go to in order to interact with this Interactable.
		public ConditionCollection[] ConditionCollections = new ConditionCollection[0]; // All the different Conditions and relevant Reactions that can happen based on them.
		public ReactionCollection DefaultReactionCollection; // If none of the ConditionCollections are reacted to, this one is used.

		// This is called when the player arrives at the interactionLocation.
		public void Interact()
		{
			// Go through all the ConditionCollections...
			for (int i = 0; i < ConditionCollections.Length; i++)
			{
				// ... then check and potentially react to each.  If the reaction happens, exit the function.
				if (ConditionCollections[i].CheckAndReact())
				{
					return;
				}
			}

			// Check for empty default collection
			if (!DefaultReactionCollection)
			{
				throw new UnityException("There is no acceptable condition and default reaction is empty!");
			}

			// If none of the reactions happened, use the default ReactionCollection.
			DefaultReactionCollection.React();
		}
	}
}