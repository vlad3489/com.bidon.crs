using TMPro;
using UnityEngine;

namespace CRS.Examples
{
	public class ExamplesMainEntry : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text _outputText;

		private void Awake()
		{
			#if UNITY_EDITOR
			AllConditions.Instance.Reset();
			#endif
		}

		public void UpdateOutputText(string text)
		{
			_outputText.text = text;
		}
	}
}