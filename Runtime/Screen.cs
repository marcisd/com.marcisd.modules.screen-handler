using System;
using UnityEngine;
using UnityEngine.Events;

/*===============================================================
Project:	Screen Handler
Developer:	Marci San Diego
Company:	Personal - marcisandiego@gmail.com
Date:       20/09/2019 19:08
===============================================================*/

namespace MSD.Modules.ScreenHandler
{
	/// <summary>
	/// A basic implementation of IScreen.
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	public class Screen : MonoBehaviour, IScreen
	{
		public event Action OnShow = delegate { };
		public event Action OnShowComplete = delegate { };
		public event Action OnHide = delegate { };
		public event Action OnHideComplete = delegate { };

		public bool isManualInvokeCompleteEvents = false;

		[SerializeField]
		private UnityEvent _onShow;

		[SerializeField]
		private UnityEvent _onShowComplete;

		[SerializeField]
		private UnityEvent _onHide;

		[SerializeField]
		private UnityEvent _onHideComplete;

		private CanvasGroup _canvasGroup;

		public CanvasGroup CanvasGroup => _canvasGroup == null ? _canvasGroup = GetComponent<CanvasGroup>() : _canvasGroup;

		public GameObject GameObject => gameObject;

		public virtual void Show()
		{
			OnShow?.Invoke();
			if (!isManualInvokeCompleteEvents) { InvokeShowComplete(); }
		}

		public virtual void InvokeShowComplete()
		{
			OnShowComplete?.Invoke();
		}

		public virtual void Hide()
		{
			OnHide?.Invoke();
			if (!isManualInvokeCompleteEvents) { InvokeHideComplete(); }
		}

		public virtual void InvokeHideComplete()
		{
			OnHideComplete?.Invoke();
		}

		protected virtual void Awake()
		{
			OnShow += _onShow.Invoke;
			OnShowComplete += _onShowComplete.Invoke;
			OnHide += _onHide.Invoke;
			OnHideComplete += _onHideComplete.Invoke;
		}

		protected virtual void OnDestroy()
		{
			OnShow -= _onShow.Invoke;
			OnShowComplete -= _onShowComplete.Invoke;
			OnHide -= _onHide.Invoke;
			OnHideComplete -= _onHideComplete.Invoke;
		}
	}
}
