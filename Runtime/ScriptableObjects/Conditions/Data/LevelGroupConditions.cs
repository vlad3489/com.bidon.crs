using UnityEngine;

namespace CRS.Data
{
	[CreateAssetMenu(fileName = "LevelGroupConditions", menuName = "ScriptableObject/LevelGroupConditions", order = 0)]
	public class LevelGroupConditions : ScriptableObject
	{
		[SerializeField]
		private string _titleName;
		
		[SerializeField]
		private Condition[] _conditions;

		public string TitleName => _titleName;
		public Condition[] Conditions => _conditions;
	}
}