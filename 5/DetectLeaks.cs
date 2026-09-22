using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020013DC RID: 5084
public class DetectLeaks : MonoBehaviour
{
	// Token: 0x06009FAA RID: 40874 RVA: 0x003C45C0 File Offset: 0x003C27C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGUI()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType<UnityEngine.Object>(FindObjectsSortMode.None);
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array2[i].GetType().ToString();
			if (dictionary.ContainsKey(text))
			{
				Dictionary<string, int> dictionary2 = dictionary;
				string key = text;
				int num = dictionary2[key];
				dictionary2[key] = num + 1;
			}
			else
			{
				dictionary[text] = 1;
			}
		}
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(dictionary);
		list.Sort((KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair) => nextPair.Value.CompareTo(firstPair.Value));
		foreach (KeyValuePair<string, int> keyValuePair in list)
		{
			GUILayout.Label(keyValuePair.Key + ": " + keyValuePair.Value.ToString(), Array.Empty<GUILayoutOption>());
		}
	}
}
