using System;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace PostcyberPunk.AutoCompilation
{
	[InitializeOnLoad]
	public static class AutoCompilation
	{
		static AutoCompilation()
		{
			if (HttpManager.instance == null)
				new HttpManager();
			Debug.Log(HttpManager.instance.listener==null);
		}

		[MenuItem("Tools/AutoCompilation/Toggle Auto-Completion")]
		public static void ToggleAutoCompilation()
		{
			bool toggle = SessionState.GetBool("DisableAutoComplation", false);
			SessionState.SetBool("DisableAutoComplation", !toggle);
			Debug.Log("Auto Completion is " + (!toggle ? "Off" : "On"));
		}
	}
}
