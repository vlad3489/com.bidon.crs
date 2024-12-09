using UnityEngine;
using System.Collections;

namespace CRS
{
	// This is a base class for Reactions that need to have a delay.
	public abstract class DelayedReaction : Reaction
	{
		public float Delay; // All DelayedReactions need to have a time that they are delayed by.
		protected WaitForSeconds Wait; // Storing the wait created from the delay, so it doesn't need to be created each time.
		private IEnumerator _routine;
		
		// This function 'hides' the Init function from the Reaction class.
		// Hiding generally happens when the original function doesn't meet
		// the requirements for the function in the inheriting class.
		// Previously, it was assumed that all Reactions just needed to call
		// specifically but with DelayedReactions, wait needs to be set too.
		public override void Init()
		{
			Wait = new WaitForSeconds(Delay);
		}

		// This function 'hides' the React function from the Reaction class.
		// It replaces the functionality with starting a coroutine instead.
		public override void React(MonoBehaviour monoBehaviour)
		{
			if (_routine != null)
			{
				monoBehaviour.StopCoroutine(_routine);
				_routine = null;
			}

			_routine = ReactCoroutine();
			monoBehaviour.StartCoroutine(_routine);
		}

		protected IEnumerator ReactCoroutine()
		{
			// Wait for the specified time.
			yield return Wait;

			// Then call the ImmediateReaction function which must be defined in inheriting classes.
			ImmediateReaction();
		}
	}
}