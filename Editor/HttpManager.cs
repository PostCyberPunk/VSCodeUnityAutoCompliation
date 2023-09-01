using System;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace PostcyberPunk.AutoCompilation
{
	public class HttpManager
	{
		public HttpListener listener;
		private bool needUpdate = false;
		private string port = "10245";
		public HttpManager()
		{
			Init();
		}
		internal void Init()
		{
			CreateListener();
			// EditorApplication.quitting += CloseListener;
			EditorApplication.update += onUpdate;
		}
		private void CreateListener()
		{
			// Debug.LogWarning("Creating");
			if (listener != null)
			{
				return;
			}
			try
			{
				listener = new HttpListener();
				listener.Prefixes.Add("http://127.0.0.1:" + port + "/refresh/");
				listener.Start();
				listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
				// Debug.Log("Auto Compilation HTTP server started");
			}
			catch (Exception e)
			{
				Debug.LogError("Auto Compilation starting failed:" + e);
			}

		}
		private void OnRequest(IAsyncResult result)
		{
			listener.EndGetContext(result);
			needUpdate = true;
		}
		private void CloseListener()
		{
			// Debug.Log("Closing Listener");
			if (listener == null)
			{
				// Debug.LogWarning("Listener is null");
				return;
			}

			listener.Stop();
			listener.Close();
			listener = null;
			// Debug.Log("Closed Listener");
		}
		private void onUpdate()
		{
			//Check focus
			if (listener.IsListening && !EditorApplication.isCompiling && needUpdate && !SessionState.GetBool("DisableAutoComplation", false))
			{
				needUpdate = false;
				AssetDatabase.Refresh();
				listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
			}
		}
	}
}