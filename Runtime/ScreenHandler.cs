using System;
using System.Collections.Generic;
using UnityEngine;

/*===============================================================
Project:	Screen Handler
Developer:	Marci San Diego
Company:	Personal - marcisandiego@gmail.com
Date:       20/09/2019 19:08
===============================================================*/

namespace MSD.Modules.ScreenHandler
{
	[RequireComponent(typeof(Canvas))]
	public partial class ScreenHandler : MonoBehaviour
	{
		private static readonly string DEBUG_PREPEND = $"[{nameof(ScreenHandler)}]";

		[SerializeField]
		private ScreenKey _startingScreenKey;

		[SerializeField]
		private ScreenDictionary _screensLookup = new ScreenDictionary();

		private readonly List<IScreen> _screenStack = new List<IScreen>();
		private List<Action> _onShowListenerCache = new List<Action>();
		private List<Action> _onShowCompleteListenerCache = new List<Action>();
		private List<Action> _onHideListenerCache = new List<Action>();
		private List<Action> _onHideCompleteListenerCache = new List<Action>();

		private Lazy<Canvas> _canvasLazyLoader;

		public Canvas ScreenCanvas => _canvasLazyLoader.Value;

		public IScreen CurrentScreen => _screenStack.Count > 0 ? _screenStack[0] : null;

		/// <summary>
		/// Hides all <see cref="IScreen"/> and shows one specified by a <see cref="ScreenKey"/>.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		public void ShowScreen(ScreenKey screenKey)
		{
			ShowScreen(screenKey, null);
		}

		/// <summary>
		/// Hides all <see cref="IScreen"/> and shows one specified by a <see cref="ScreenKey"/>.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		/// <param name="onComplete">An event that gets triggered once the <see cref="IScreen.Show"/> routine has completed.</param>
		public void ShowScreen(ScreenKey screenKey, Action onComplete)
		{
			InternalAppendScreen(screenKey, () => { HideAllScreenExcept(screenKey); }, onComplete);
		}

		/// <summary>
		/// Adds a <see cref="IScreen"/> specified by a <see cref="ScreenKey"/> into the screen stack.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		public void AppendScreen(ScreenKey screenKey)
		{
			AppendScreen(screenKey, null);
		}

		/// <summary>
		/// Adds a <see cref="IScreen"/> specified by a <see cref="ScreenKey"/> into the screen stack.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		/// <param name="onComplete">An event that gets triggered once the <see cref="IScreen.Show"/> routine has completed.</param>
		public void AppendScreen(ScreenKey screenKey, Action onComplete)
		{
			InternalAppendScreen(screenKey, null, onComplete);
		}

		/// <summary>
		/// Hides all shown <see cref="IScreen"/>s.
		/// </summary>
		public void HideAllScreen()
		{
			while (_screenStack.Count > 0) { HideCurrentScreen(); }
		}

		/// <summary>
		/// Hides the last shown <see cref="IScreen"/>.
		/// </summary>
		public void HideCurrentScreen()
		{
			HideCurrentScreen(null);
		}

		/// <summary>
		/// Hides the last shown <see cref="IScreen"/>.
		/// </summary>
		/// <param name="onComplete">An event that gets triggered once the <see cref="IScreen.Hide"/> routine has completed.</param>
		public void HideCurrentScreen(Action onComplete)
		{
			IScreen screen = CurrentScreen;
			if (screen != null) {
				InternalHideScreen(screen, onComplete);
			}
		}

		/// <summary>
		/// Hides the <see cref="IScreen"/> specified by a <see cref="ScreenKey"/>.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		public void HideScreen(ScreenKey screenKey)
		{
			HideScreen(screenKey, null);
		}

		/// <summary>
		/// Hides the <see cref="IScreen"/> specified by a <see cref="ScreenKey"/>.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		/// <param name="onComplete">An event that gets triggered once the <see cref="IScreen.Hide"/> routine has completed.</param>
		public void HideScreen(ScreenKey screenKey, Action onComplete)
		{
			if (TryGetScreen(screenKey, out IScreen screen)) {
				if (_screenStack.Contains(screen)) {
					InternalHideScreen(screen, onComplete);
				}
			}
		}

		/// <summary>
		/// Checks if the <see cref="IScreen"/> specified by a <see cref="ScreenKey"/> is shown.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		/// <returns><c>true</c> if shown, <c>false</c> otherwise.</returns>
		public bool IsScreenShown(ScreenKey screenKey)
		{
			if (TryGetScreen(screenKey, out IScreen screen)) {
				return _screenStack.Contains(screen);
			}
			return false;
		}

		/// <summary>
		/// Gets the <see cref="IScreen"/> specified by a <see cref="ScreenKey"/>.
		/// </summary>
		/// <param name="screenKey">The <see cref="ScreenKey"/> associated to the <see cref="IScreen"/>.</param>
		/// <param name="screen">When this method returns, contains the <see cref="IScreen"/> associated with the specified <see cref="ScreenKey"/>, if the key is found; otherwise, <c>null</c>.</param>
		/// <returns><c>true</c> if a screen is found, <c>false</c> otherwise.</returns>
		public bool TryGetScreen(ScreenKey screenKey, out IScreen screen)
		{
			bool result = _screensLookup.TryGetValue(screenKey, out SerializableScreen iScreen);
			if (!result) {
				Debugger.LogWarning(DEBUG_PREPEND, $"Screen with ScreenKey: {screenKey} doesn't exist in the lookup!");
			}
			screen = iScreen?.Value;
			return result;
		}

		private void InternalAppendScreen(ScreenKey screenKey, Action onBeforeShow, Action onComplete)
		{
			if (TryGetScreen(screenKey, out IScreen screen)) {

				onBeforeShow?.Invoke();

				if (CurrentScreen == screen) { return; }
				if (_screenStack.Contains(screen)) { return; }

				screen.Show();

				// Fire and remove
				if (onComplete != null) {
					onComplete += () => { screen.OnShowComplete -= onComplete; };
					screen.OnShowComplete += onComplete;
				}

				_screenStack.Insert(0, screen);
			}
		}

		private void InternalHideScreen(IScreen screen, Action onComplete)
		{
			screen.Hide();

			// Fire and remove
			if(onComplete != null) {
				onComplete += () => { screen.OnHideComplete -= onComplete; };
				screen.OnHideComplete += onComplete;
			}

			_screenStack.Remove(screen);
		}

		private void HideAllScreenExcept(ScreenKey screenKey)
		{
			while (_screenStack.Count > 0) {
				TryGetScreen(screenKey, out IScreen screen);
				if (CurrentScreen != screen) {
					HideCurrentScreen();
				} else {
					if (_screenStack.Count > 1) {
						InternalHideScreen(_screenStack[1], null);
					} else {
						break;
					}
				}
			}
		}

		private void OnShow(IScreen screen)
		{
			Debugger.Log(DEBUG_PREPEND, $"Show: {screen.GameObject.name}");
			SetCanvasGroupInteractable(screen, false);
			SetGameObjectActive(screen, true);
		}

		private void OnShowComplete(IScreen screen)
		{
			Debugger.Log(DEBUG_PREPEND, $"Show complete: {screen.GameObject.name}");
			SetCanvasGroupInteractable(screen, true);
		}

		private void OnHide(IScreen screen)
		{
			Debugger.Log(DEBUG_PREPEND, $"Hide: {screen.GameObject.name}");
			SetCanvasGroupInteractable(screen, false);
		} 

		private void OnHideComplete(IScreen screen)
		{
			Debugger.Log(DEBUG_PREPEND, $"Hide complete: {screen.GameObject.name}");
			SetGameObjectActive(screen, false);
		}

		private void SetCanvasGroupInteractable(IScreen screen, bool value)
		{
			screen.CanvasGroup.interactable = value;
		}

		private void SetGameObjectActive(IScreen screen, bool value)
		{
			screen.GameObject.SetActive(value);
		}

		#region MonoBehaviour

		private void Awake()
		{
			_canvasLazyLoader = new Lazy<Canvas>(() => GetComponent<Canvas>());
			InstantiatePrefabScreens();
		}

		private void Start()
		{
			ForceHideAllScreens();
			ShowStartingScreen();
		}

		private void OnEnable()
		{
			HookToScreenEvents();
			RegisterScreenKeys();
		}

		private void OnDisable()
		{
			UnhookToScreenEvents();
			DeregisterScreenKeys();
		}

		#endregion

		private void InstantiatePrefabScreens()
		{
			Dictionary<ScreenKey, IScreen> instantiatedScreens = new Dictionary<ScreenKey, IScreen>();

			foreach (KeyValuePair<ScreenKey, IScreen> screenPair in _screensLookup) {
				IScreen screen = screenPair.Value;

				if (screen.GameObject.IsPrefab()) {
					GameObject newScreenGO = Instantiate(screen.GameObject, ScreenCanvas.transform);
					newScreenGO.name = screen.GameObject.name;

					IScreen newScreen = newScreenGO.GetComponent<IScreen>();
					instantiatedScreens.Add(screenPair.Key, newScreen);
				}
			}

			foreach (KeyValuePair<ScreenKey, IScreen> pair in instantiatedScreens) {
				_screensLookup.SetNewScreen(pair.Key, pair.Value);
			}
		}

		private void ForceHideAllScreens()
		{
			_screensLookup.ForEachValidScreen((screen) => {
				SetCanvasGroupInteractable(screen, false);
				SetGameObjectActive(screen, false);
			});
		}

		private void ShowStartingScreen()
		{
			if (_startingScreenKey != null) {
				ShowScreen(_startingScreenKey);
			}
		}

		private void HookToScreenEvents()
		{
			_onShowCompleteListenerCache.Clear();
			_onShowListenerCache.Clear();
			_onHideCompleteListenerCache.Clear();
			_onHideListenerCache.Clear();

			_screensLookup.ForEachValidScreen((screen) => {
				screen.OnShow += CreateCachedAction(() => { OnShow(screen); }, ref _onShowListenerCache);
				screen.OnShowComplete += CreateCachedAction(() => { OnShowComplete(screen); }, ref _onShowCompleteListenerCache);
				screen.OnHide += CreateCachedAction(() => { OnHide(screen); }, ref _onHideListenerCache);
				screen.OnHideComplete += CreateCachedAction(() => { OnHideComplete(screen); }, ref _onHideCompleteListenerCache);
			});

			static Action CreateCachedAction(Action action, ref List<Action> cache)
			{
				cache.Add(action);
				return action;
			}
		}

		private void RegisterScreenKeys()
		{
			foreach (ScreenKey screenKey in _screensLookup.Keys) {
				screenKey.Register(this);
			} 
		}

		private void UnhookToScreenEvents()
		{
			int index = 0;

			_screensLookup.ForEachValidScreen((screen) => {
				screen.OnShow -= _onShowListenerCache[index];
				screen.OnShowComplete -= _onShowCompleteListenerCache[index];
				screen.OnHide -= _onHideListenerCache[index];
				screen.OnHideComplete -= _onHideCompleteListenerCache[index];

				index++;
			});
		}

		private void DeregisterScreenKeys()
		{
			foreach (ScreenKey screenKey in _screensLookup.Keys) {
				screenKey.Deregister();
			}
		}
	}
}

