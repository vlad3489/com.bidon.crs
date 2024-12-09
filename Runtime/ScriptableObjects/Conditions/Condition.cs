using UnityEngine;

namespace CRS
{
	public class Condition : ScriptableObject
	{
		public string Description;
		public bool IsSatisfied;
		public bool DefaultValue;
		public int Hash;
	}
}