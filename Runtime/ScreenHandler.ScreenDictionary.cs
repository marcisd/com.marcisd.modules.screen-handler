using System;
using System.Collections.Generic;

/*===============================================================
Project:	Core Library
Developer:	Marci San Diego
Company:	Personal - marcisandiego@gmail.com
Date:		27/09/2020 22:29
===============================================================*/

namespace MSD.Modules.ScreenHandler
{
	public partial class ScreenHandler
	{
		[Serializable]
		public class SerializableScreen : SerializableInterface<IScreen>
		{
			public SerializableScreen(IScreen value) : base(value) { }
		}

		[Serializable]
		public class ScreenDictionary : SerializableDictionary<ScreenKey, SerializableScreen>
		{
			// Iterate only on valid screens
			public new IEnumerator<KeyValuePair<ScreenKey, IScreen>> GetEnumerator()
			{
				foreach (ScreenKey key in Keys) {
					if (this[key].Value != null) {
						yield return new KeyValuePair<ScreenKey, IScreen>(key, this[key].Value);
					}
				}
			}

			internal void ForEachValidScreen(Action<IScreen> action)
			{
				foreach (KeyValuePair<ScreenKey, IScreen> screenPair in this) {
					action.Invoke(screenPair.Value);
				}
			}

			internal void SetNewScreen(ScreenKey screenKey, IScreen screen)
			{
				this[screenKey] = new SerializableScreen(screen);
			}
		}
	}
}
