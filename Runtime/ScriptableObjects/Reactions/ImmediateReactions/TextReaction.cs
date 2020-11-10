using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CRS
{
	public class TextReaction : Reaction
	{
		public string Message;
		public TMP_Text TextField;

		protected override void ImmediateReaction ()
		{
			TextField.text = Message;
		}
	}
}
