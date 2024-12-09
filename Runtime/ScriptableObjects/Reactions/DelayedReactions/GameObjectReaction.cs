using UnityEngine;

namespace CRS
{
	public class GameObjectReaction : DelayedReaction
	{
		public GameObject gameObject;       // The gameobject to be turned on or off.
		public bool activeState;            // The state that the gameobject will be in after the Reaction.


		protected override void ImmediateReaction ()
		{
			if (!gameObject)
				throw new UnityException ($"Field gameObject is empty!");

			gameObject.SetActive (activeState);
		}
	}
}