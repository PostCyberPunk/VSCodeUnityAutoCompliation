using UnityEditor;
using UnityEngine;

namespace PostcyberPunk.AutoCompilation
{
	public class AutoCompilation : ScriptableSingleton<AutoCompilation>
	{
		public HttpManager httpManager;
		[InitializeOnLoadMethod]
		public static void Init()
		{
			if (instance.httpManager == null)
			{
				instance.httpManager = new HttpManager();
			}
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
