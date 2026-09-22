using System;
using UnityEngine.Scripting;
using Utf8Json;

namespace Webserver.WebAPI.APIs.ServerState
{
	// Token: 0x02001ADE RID: 6878
	[Preserve]
	public class GameStats : KeyValueListAbs
	{
		// Token: 0x0600CF05 RID: 52997 RVA: 0x004B89D0 File Offset: 0x004B6BD0
		public GameStats() : base("GameStats")
		{
		}

		// Token: 0x0600CF06 RID: 52998 RVA: 0x004B89E0 File Offset: 0x004B6BE0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void iterateList(ref JsonWriter _writer, ref bool _first)
		{
			foreach (EnumGameStats enumGameStats in EnumUtils.Values<EnumGameStats>())
			{
				string key = enumGameStats.ToStringCached<EnumGameStats>();
				GameStats.EnumType? statType = GameStats.GetStatType(enumGameStats);
				if (statType != null)
				{
					switch (statType.GetValueOrDefault())
					{
					case GameStats.EnumType.Int:
						base.addItem(ref _writer, ref _first, key, GameStats.GetInt(enumGameStats));
						break;
					case GameStats.EnumType.Float:
						base.addItem(ref _writer, ref _first, key, GameStats.GetFloat(enumGameStats));
						break;
					case GameStats.EnumType.String:
						base.addItem(ref _writer, ref _first, key, GameStats.GetString(enumGameStats));
						break;
					case GameStats.EnumType.Bool:
						base.addItem(ref _writer, ref _first, key, GameStats.GetBool(enumGameStats));
						break;
					}
				}
			}
		}
	}
}
