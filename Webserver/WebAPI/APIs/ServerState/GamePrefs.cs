using System;
using UnityEngine.Scripting;
using Utf8Json;

namespace Webserver.WebAPI.APIs.ServerState
{
	// Token: 0x02001ADD RID: 6877
	[Preserve]
	public class GamePrefs : KeyValueListAbs
	{
		// Token: 0x0600CF03 RID: 52995 RVA: 0x004B88A2 File Offset: 0x004B6AA2
		public GamePrefs() : base("GamePrefs")
		{
		}

		// Token: 0x0600CF04 RID: 52996 RVA: 0x004B88B0 File Offset: 0x004B6AB0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void iterateList(ref JsonWriter _writer, ref bool _first)
		{
			foreach (EnumGamePrefs enumGamePrefs in EnumUtils.Values<EnumGamePrefs>())
			{
				string text = enumGamePrefs.ToStringCached<EnumGamePrefs>();
				if (!text.Contains("Password", StringComparison.Ordinal))
				{
					GamePrefs.EnumType? prefType = GamePrefs.GetPrefType(enumGamePrefs);
					object @default = GamePrefs.GetDefault(enumGamePrefs);
					if (prefType != null)
					{
						switch (prefType.GetValueOrDefault())
						{
						case GamePrefs.EnumType.Int:
						{
							int? default2 = @default as int?;
							base.addItem(ref _writer, ref _first, text, GamePrefs.GetInt(enumGamePrefs), default2);
							break;
						}
						case GamePrefs.EnumType.Float:
						{
							float? default3 = @default as float?;
							base.addItem(ref _writer, ref _first, text, GamePrefs.GetFloat(enumGamePrefs), default3);
							break;
						}
						case GamePrefs.EnumType.String:
						{
							string default4 = @default as string;
							base.addItem(ref _writer, ref _first, text, GamePrefs.GetString(enumGamePrefs), default4);
							break;
						}
						case GamePrefs.EnumType.Bool:
						{
							bool? default5 = @default as bool?;
							base.addItem(ref _writer, ref _first, text, GamePrefs.GetBool(enumGamePrefs), default5);
							break;
						}
						}
					}
				}
			}
		}
	}
}
