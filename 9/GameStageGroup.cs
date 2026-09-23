using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x02000AEE RID: 2798
public sealed class GameStageGroup
{
	// Token: 0x0600536C RID: 21356 RVA: 0x001FE5E4 File Offset: 0x001FC7E4
	public GameStageGroup(GameStageDefinition _spawner)
	{
		this.spawner = _spawner;
	}

	// Token: 0x0600536D RID: 21357 RVA: 0x001FE5F4 File Offset: 0x001FC7F4
	public static void AddGameStageGroup(string _fullName, GameStageGroup _group)
	{
		string key = GameStageGroup.CleanName(_fullName);
		GameStageGroup.groups.Add(key, _group);
		GameStageGroup.groupsFullName.Add(_fullName, _group);
	}

	// Token: 0x0600536E RID: 21358 RVA: 0x001FE620 File Offset: 0x001FC820
	public static GameStageGroup TryGet(string _name)
	{
		GameStageGroup result;
		if (GameStageGroup.groups.TryGetValue(_name, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x0600536F RID: 21359 RVA: 0x001FE63F File Offset: 0x001FC83F
	public static void Clear()
	{
		GameStageGroup.groups.Clear();
		GameStageGroup.groupsFullName.Clear();
	}

	// Token: 0x17000909 RID: 2313
	// (get) Token: 0x06005370 RID: 21360 RVA: 0x001FE655 File Offset: 0x001FC855
	public static Dictionary<string, GameStageGroup> Groups
	{
		get
		{
			return GameStageGroup.groupsFullName;
		}
	}

	// Token: 0x06005371 RID: 21361 RVA: 0x001FE65C File Offset: 0x001FC85C
	public static string CleanName(string _name)
	{
		if (_name.Length > 0 && char.IsDigit(_name[0]))
		{
			_name = _name.Substring(1);
		}
		else if (_name.StartsWith("S_"))
		{
			int startIndex = 2;
			if (_name.StartsWith("S_-"))
			{
				startIndex = 3;
			}
			return _name.Substring(startIndex).Replace("_", "");
		}
		return _name;
	}

	// Token: 0x06005372 RID: 21362 RVA: 0x001FE6C4 File Offset: 0x001FC8C4
	public static string MakeDisplayName(string _name)
	{
		bool flag = false;
		foreach (char c in _name)
		{
			if (!char.IsDigit(c))
			{
				if (char.IsUpper(c) && flag)
				{
					GameStageGroup.stringBuilder.Append(' ');
				}
				GameStageGroup.stringBuilder.Append(c);
				flag = true;
			}
		}
		_name = GameStageGroup.stringBuilder.ToString();
		GameStageGroup.stringBuilder.Clear();
		return _name;
	}

	// Token: 0x04004133 RID: 16691
	public const string cDefaultGroupName = "GroupGenericZombie";

	// Token: 0x04004134 RID: 16692
	public readonly GameStageDefinition spawner;

	// Token: 0x04004135 RID: 16693
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, GameStageGroup> groups = new Dictionary<string, GameStageGroup>();

	// Token: 0x04004136 RID: 16694
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, GameStageGroup> groupsFullName = new Dictionary<string, GameStageGroup>();

	// Token: 0x04004137 RID: 16695
	[PublicizedFrom(EAccessModifier.Private)]
	public static StringBuilder stringBuilder = new StringBuilder();
}
