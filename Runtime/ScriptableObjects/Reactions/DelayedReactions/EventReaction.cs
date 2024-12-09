using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CRS
{
	public class EventReaction : DelayedReaction
	{
		public UnityEvent GenericUnityEvent;
		
		protected override void ImmediateReaction ()
		{
			GenericUnityEvent?.Invoke ();
		}
	}
}
