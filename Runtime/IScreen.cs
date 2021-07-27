using System;
using UnityEngine;

/*===============================================================
Project:	Screen Handler
Developer:	Marci San Diego
Company:	Personal - marcisandiego@gmail.com
Date:       20/09/2019 19:08
===============================================================*/

namespace MSD.Modules.ScreenHandler
{
	/// <summary>
	/// Interface for implementing a Screen.
	/// </summary>
	public interface IScreen
	{
		event Action OnShow;
		event Action OnShowComplete;
		event Action OnHide;
		event Action OnHideComplete;

		void Show();
		void Hide();

		GameObject GameObject { get; }
		CanvasGroup CanvasGroup { get; }
	}
}
