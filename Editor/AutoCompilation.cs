using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using System.Net;
using System.Net.Sockets;

namespace PostcyberPunk.AutoCompilation
{
	[InitializeOnLoad]
	public static class AutoCompilation
	{
		private static HttpListener listener;
		private static bool needUpdate;
		private static string port = "10245";
		private static IAsyncResult _result;
		static AutoCompilation()
		{
			if (!SessionState.GetBool("DisableAutoComplation", false))
			{
				needUpdate = false;
				CompilationPipeline.compilationStarted += OnCompilationStarted;
				CompilationPipeline.compilationFinished += OnCompilationFinished;
				EditorApplication.quitting += _closeListener;
				EditorApplication.update += onUpdate;
				_createListener();
			}
		}

		private static void _createListener()
		{
			if (listener != null)
			{
				return;
			};
			try
			{
				listener = new HttpListener();
				listener.Prefixes.Add("http://127.0.0.1:" + port + "/refresh/");
				listener.Start();
				_result = listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
				
				// Debug.Log("Auto Compilation HTTP server started");
			}
			catch (Exception e)
			{
				// ForceReleasePort();
				Debug.LogError("Auto Compilation starting failed:" + e);
			}

		}
		private static void OnRequest(IAsyncResult result)
		{

			if (listener.IsListening && !EditorApplication.isCompiling)
			{
				listener.EndGetContext(result);
				needUpdate = true;
				// var context = listener.EndGetContext(result);
				listener.Stop();
				listener.Close();
				listener = null;
				// var request = context.Request;
				// _result = listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
			}
		}
		private static void _closeListener()
		{
			// Debug.Log("Closing Listener");
			if (listener == null)
			{
				// Debug.LogWarning("Listener is null");
				return;
			}

			// listener.EndGetContext(_result);
			listener.Stop();
			
			listener.Close();
			// listener.Abort();
			listener = null;
		}
		private static void onUpdate()
		{
			if (!EditorApplication.isCompiling && !EditorApplication.isUpdating && needUpdate)
			{
				needUpdate = false;
				// Debug.Log("Compiled in background");
				AssetDatabase.Refresh();
			}
		}
		[MenuItem("Tools/AutoCompilation/Toggle Auto-Completion")]
		public static void ToggleAutoCompilation()
		{
			bool toggle = SessionState.GetBool("DisableAutoComplation", false);
			if (toggle)
			{
				_closeListener();
			}
			else
			{
				_createListener();
			}
			SessionState.SetBool("DisableAutoComplation", !toggle);
			Debug.Log("Auto Completion is " + (toggle ? "Off" : "On"));
		}
		private static void OnCompilationStarted(object _) => _closeListener();
		private static void OnCompilationFinished(object _) => _createListener();
	}
}
