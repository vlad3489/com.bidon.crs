﻿using System.Collections.Generic;
using UnityEngine;

namespace CRS
{
	// This script acts as a collection for all the
	// individual Reactions that happen as a result
	// of an interaction.
	public class ReactionCollection : MonoBehaviour
	{
		public string Description;
		public Reaction[] Reactions = new Reaction[0]; // Array of all the Reactions to play when React is called.
		public List<Reaction> ReactionsList = new(); // For editor reordable list.

		// Debug options fields (used for editor)
		public bool UseDebugOptions;
		public bool PauseEditorOnReactionCollection;
		public bool PrintExecutedReactionCollection;

		private void Start()
		{
			// Go through all the Reactions and call their Init function.
			for (int i = 0; i < Reactions.Length; i++)
			{
				Reactions[i].Init();
			}
		}

		public void React()
		{
			// Reaction debug options actions
			DebugModeActions();

			// Go through all the Reactions and call their React function.
			for (int i = 0; i < Reactions.Length; i++)
			{
				Reactions[i].React(this);
			}
		}

		private void DebugModeActions()
		{
			#if UNITY_EDITOR
			if (!UseDebugOptions)
			{
				return;
			}

			if (PauseEditorOnReactionCollection)
			{
				Debug.Log($"<color=#943F1E><b>DEBUG OPTION: </b></color>Set editor is on pause : {gameObject.name}", gameObject);
				Debug.Break();
			}

			if (PrintExecutedReactionCollection)
			{
				Debug.Log($"<color=#943F1E><b>DEBUG OPTION: </b></color> Execute reaction : {gameObject.name}", gameObject);
			}
			#endif
		}
	}
}