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
		[SerializeField]
		private bool _shouldManualInvokeCompleteEvents;

		public event Action OnShow = delegate { };
		public event Action OnShowComplete = delegate { };
		public event Action OnHide = delegate { };
		public event Action OnHideComplete = delegate { };

		[SerializeField]
		private UnityEvent _onShow;

		[SerializeField]
		private UnityEvent _onShowComplete;

		[SerializeField]
		private UnityEvent _onHide;

		[SerializeField]
		private UnityEvent _onHideComplete;

		private Lazy<CanvasGroup> _canvasGroupLazyLoader;

		public CanvasGroup CanvasGroup => _canvasGroupLazyLoader.Value;

		public GameObject GameObject => gameObject;

		public virtual void Show()
		{
			OnShow?.Invoke();
			if (!_shouldManualInvokeCompleteEvents) { InvokeShowComplete(); }
		}

		public virtual void InvokeShowComplete()
		{
			OnShowComplete?.Invoke();
		}

		public virtual void Hide()
		{
			OnHide?.Invoke();
			if (!_shouldManualInvokeCompleteEvents) { InvokeHideComplete(); }
		}

		public virtual void InvokeHideComplete()
		{
			OnHideComplete?.Invoke();
		}

		protected virtual void Awake()
		{
			_canvasGroupLazyLoader = new Lazy<CanvasGroup>(() => GetComponent<CanvasGroup>());

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
