using System;
using CRS.Data;
using UnityEngine;

namespace CRS
{
	// This script works as a singleton asset.  That means that
	// it is globally accessible through a static instance
	// reference.  
	public class AllConditions : ResettableScriptableObject
	{
		private const string LOAD_PATH = "AllConditions";		// The path within the Resources folder that 
		private static AllConditions _instance;					// The singleton instance.
		
		public Condition[] Conditions;							// All the Conditions that exist in the game.
		public LevelGroupConditions[] LevelsCondition;
		
		public static AllConditions Instance // The public accessor for the singleton instance.
		{
			get
			{
				// If the instance is currently null, try to find an AllConditions instance already in memory.
				if (!_instance)
				{
					_instance = FindObjectOfType<AllConditions>();
				}

				// If the instance is still null, try to load it from the Resources folder.
				if (!_instance)
				{
					_instance = Resources.Load<AllConditions>(LOAD_PATH);
				}

				// If the instance is still null, report that it has not been created yet.
				if (!_instance)
				{
					Debug.LogError("AllConditions has not been created yet.  Go to Assets > Create > AllConditions.");
				}

				return _instance;
			}
			
			set => _instance = value;
		}

		// This function will be called at Start once per run of the game.
		public override void Reset()
		{
			// If there are no conditions, do nothing.
			if (Conditions == null)
			{
				return;
			}

			// Set all the conditions to default state.
			foreach (var condition in Conditions)
			{
				condition.IsSatisfied = condition.DefaultValue;
			}
		}

		// This is called from ConditionCollections when they are being checked by an Interactable that has been clicked on.
		public static bool CheckCondition(Condition requiredCondition)
		{
			// Cache the condition array.
			Condition[] allConditions = Instance.Conditions;
			Condition globalCondition = null;

			// If there is at least one condition...
			if (allConditions != null && allConditions[0] != null)
			{
				// ... go through all the conditions...
				for (int i = 0; i < allConditions.Length; i++)
				{
					// ... and if they match the given condition then this is the global version of the requiredConditiond.
					if (allConditions[i].Hash == requiredCondition.Hash)
					{
						globalCondition = allConditions[i];
					}
				}
			}

			// If by this point a globalCondition hasn't been found then return false.
			if (!globalCondition)
			{
				return false;
			}

			// Return true if the satisfied states match, false otherwise.
			return globalCondition.IsSatisfied == requiredCondition.IsSatisfied;
		}
	}
}