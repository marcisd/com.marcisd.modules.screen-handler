using UnityEngine;

/*===============================================================
Project:	Screen Handler
Developer:	Marci San Diego
Company:	Personal - marcisandiego@gmail.com
Date:       25/09/2019 19:08
===============================================================*/

namespace MSD.Modules.ScreenHandler
{
	[CreateAssetMenu(menuName = "DMED/Modules/Screen Handler/Screen Key")]
	public class ScreenKey : ScriptableObject
	{
		private static readonly string DEBUG_PREPEND = $"[{nameof(ScreenKey)}]";

		private ScreenHandler _screenHandler = null;

		public void ShowScreen()
		{
			if (_screenHandler == null) {
				Debugger.LogError(DEBUG_PREPEND, "Cannot show an unregistered Screen Key!");
				return;
			}
			_screenHandler.ShowScreen(this);
		}

		public void HideScreen()
		{
			if (_screenHandler == null) {
				Debugger.LogError(DEBUG_PREPEND, "Cannot hide an unregistered Screen Key!");
				return;
			}
			_screenHandler.HideScreen(this);
		}

		internal void Register(ScreenHandler screenHandler)
		{
			if (_screenHandler != null) {
				Debugger.LogWarning(DEBUG_PREPEND, "Attempted to register to more than one Screen Handler! Will only register to the latest one.");
			}
			_screenHandler = screenHandler;
		}

		internal void Deregister()
		{
			if (_screenHandler == null) {
				Debugger.LogWarning(DEBUG_PREPEND, "Attempt to deregister a an unregistered Screen Key!");
			}
			_screenHandler = null;
		}
	}
}
