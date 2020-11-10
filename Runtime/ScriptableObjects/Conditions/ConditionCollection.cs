using UnityEngine;

namespace CRS
{
	// This class represents a single outcome from clicking
	// on an interactable.  It has an array of Conditions
	// and if they are all met an ReactionCollection that
	// will happen.
	// Used for Inspector
	public class ConditionCollection : ScriptableObject
	{
		public string Description;                                  // Description of the ConditionCollection.  This is used purely for identification in the inspector.
		public Condition[] RequiredConditions = new Condition[0];   // The Conditions that need to be met in order for the ReactionCollection to React.
		public ReactionCollection ReactionCollection;               // Reference to the ReactionCollection that will React should all the Conditions be met.

		// This is called by the Interactable one at a time for each of its ConditionCollections until one returns true.
		public bool CheckAndReact ()
		{
			// Go through all Conditions...
			for (int i = 0; i < RequiredConditions.Length; i++)
			{
				// ... and check them against the AllConditions version of the Condition.
				// If they don't have the same satisfied flag, return false.
				if (!AllConditions.CheckCondition (RequiredConditions[i]))
					return false;
			}

			// If there is an ReactionCollection assigned, call its React function.
			if (ReactionCollection)
				ReactionCollection.React ();

			// A Reaction happened so return true.
			return true;
		}
	}
}