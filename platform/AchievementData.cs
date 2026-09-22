using System;
using System.Collections.Generic;
using System.IO;

namespace Platform
{
	// Token: 0x02001B36 RID: 6966
	public class AchievementData : Serializable
	{
		// Token: 0x0600D0E0 RID: 53472 RVA: 0x004C65B8 File Offset: 0x004C47B8
		[PublicizedFrom(EAccessModifier.Private)]
		public static EnumDictionary<EnumAchievementManagerAchievement, EnumAchievementDataStat> CreateAchievementToStat()
		{
			EnumDictionary<EnumAchievementManagerAchievement, EnumAchievementDataStat> enumDictionary = new EnumDictionary<EnumAchievementManagerAchievement, EnumAchievementDataStat>();
			foreach (AchievementData.AchievementStatDecl achievementStatDecl in AchievementData.propertyList)
			{
				foreach (AchievementData.AchievementInfo achievementInfo in achievementStatDecl.achievementInfos)
				{
					enumDictionary.Add(achievementInfo.achievement, achievementStatDecl.name);
				}
			}
			return enumDictionary;
		}

		// Token: 0x0600D0E1 RID: 53473 RVA: 0x004C663C File Offset: 0x004C483C
		public static EnumStatType GetStatType(EnumAchievementDataStat _stat)
		{
			if (_stat != EnumAchievementDataStat.Last)
			{
				return AchievementData.propertyList[(int)_stat].type;
			}
			return EnumStatType.Invalid;
		}

		// Token: 0x0600D0E2 RID: 53474 RVA: 0x004C6655 File Offset: 0x004C4855
		public static AchievementData.EnumUpdateType GetUpdateType(EnumAchievementDataStat _stat)
		{
			if (_stat != EnumAchievementDataStat.Last)
			{
				return AchievementData.propertyList[(int)_stat].updateType;
			}
			return AchievementData.EnumUpdateType.Replace;
		}

		// Token: 0x0600D0E3 RID: 53475 RVA: 0x004C666E File Offset: 0x004C486E
		public static List<AchievementData.AchievementInfo> GetAchievementInfos(EnumAchievementDataStat _stat)
		{
			return AchievementData.propertyList[(int)_stat].achievementInfos;
		}

		// Token: 0x0600D0E4 RID: 53476 RVA: 0x004C6680 File Offset: 0x004C4880
		public static EnumAchievementDataStat GetStat(EnumAchievementManagerAchievement _achievement)
		{
			return AchievementData.achievementToStat[_achievement];
		}

		// Token: 0x170019A0 RID: 6560
		// (get) Token: 0x0600D0E5 RID: 53477 RVA: 0x004C668D File Offset: 0x004C488D
		// (set) Token: 0x0600D0E6 RID: 53478 RVA: 0x004C6695 File Offset: 0x004C4895
		public bool IsDirty { get; set; }

		// Token: 0x0600D0E7 RID: 53479 RVA: 0x004C66A0 File Offset: 0x004C48A0
		public AchievementData()
		{
			this.statValues = new object[AchievementData.propertyList.Length];
			this.achievementStatuses = new EnumDictionary<EnumAchievementManagerAchievement, bool>();
			AchievementData.AchievementStatDecl[] array = AchievementData.propertyList;
			for (int i = 0; i < array.Length; i++)
			{
				int name = (int)array[i].name;
				this.statValues[name] = 0;
			}
			for (int j = 0; j < 48; j++)
			{
				this.achievementStatuses[(EnumAchievementManagerAchievement)j] = false;
			}
		}

		// Token: 0x0600D0E8 RID: 53480 RVA: 0x004C671C File Offset: 0x004C491C
		public void UpdateAchievement(EnumAchievementDataStat _stat)
		{
			List<AchievementData.AchievementInfo> achievementInfos = AchievementData.GetAchievementInfos(_stat);
			object achievementStatValue = this.GetAchievementStatValue(_stat);
			EnumStatType statType = AchievementData.GetStatType(_stat);
			for (int i = 0; i < achievementInfos.Count; i++)
			{
				EnumAchievementManagerAchievement achievement = achievementInfos[i].achievement;
				if (statType == EnumStatType.Int)
				{
					if ((int)achievementStatValue >= Convert.ToInt32(achievementInfos[i].triggerPoint) && !this.IsAchievementLocked(achievement))
					{
						this.LockAchievement(achievement);
					}
				}
				else if (Convert.ToSingle(achievementStatValue) >= Convert.ToSingle(achievementInfos[i].triggerPoint) && !this.IsAchievementLocked(achievement))
				{
					this.LockAchievement(achievement);
				}
			}
		}

		// Token: 0x0600D0E9 RID: 53481 RVA: 0x004C67B9 File Offset: 0x004C49B9
		public void SetStatCompleteCallback(Action<EnumAchievementManagerAchievement> _statCompleteCallback)
		{
			this.statCompleteCallback = _statCompleteCallback;
		}

		// Token: 0x0600D0EA RID: 53482 RVA: 0x004C67C2 File Offset: 0x004C49C2
		public int GetIntStatValue(EnumAchievementDataStat _stat)
		{
			if (AchievementData.propertyList[(int)_stat].type == EnumStatType.Int)
			{
				return Convert.ToInt32(this.statValues[(int)_stat]);
			}
			return -1;
		}

		// Token: 0x0600D0EB RID: 53483 RVA: 0x004C67E5 File Offset: 0x004C49E5
		public float GetFloatStatValue(EnumAchievementDataStat _stat)
		{
			if (AchievementData.propertyList[(int)_stat].type == EnumStatType.Float)
			{
				return Convert.ToSingle(this.statValues[(int)_stat]);
			}
			return -1f;
		}

		// Token: 0x0600D0EC RID: 53484 RVA: 0x004C6810 File Offset: 0x004C4A10
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetStatValue(EnumAchievementDataStat _stat, object _value)
		{
			AchievementData.EnumUpdateType updateType = AchievementData.propertyList[(int)_stat].updateType;
			EnumStatType type = AchievementData.propertyList[(int)_stat].type;
			object[] obj = this.statValues;
			lock (obj)
			{
				switch (updateType)
				{
				case AchievementData.EnumUpdateType.Sum:
					if (type == EnumStatType.Int)
					{
						this.statValues[(int)_stat] = Convert.ToInt32(this.statValues[(int)_stat]) + Convert.ToInt32(_value);
					}
					else
					{
						this.statValues[(int)_stat] = Convert.ToSingle(this.statValues[(int)_stat]) + Convert.ToSingle(_value);
					}
					break;
				case AchievementData.EnumUpdateType.Replace:
					this.statValues[(int)_stat] = _value;
					break;
				case AchievementData.EnumUpdateType.Max:
					if (type == EnumStatType.Int)
					{
						this.statValues[(int)_stat] = ((Convert.ToInt32(_value) > Convert.ToInt32(this.statValues[(int)_stat])) ? _value : this.statValues[(int)_stat]);
					}
					else
					{
						this.statValues[(int)_stat] = ((Convert.ToSingle(_value) > Convert.ToSingle(this.statValues[(int)_stat])) ? _value : this.statValues[(int)_stat]);
					}
					break;
				}
			}
			this.IsDirty = true;
			this.UpdateAchievement(_stat);
		}

		// Token: 0x0600D0ED RID: 53485 RVA: 0x004C6940 File Offset: 0x004C4B40
		public virtual void SetAchievementStat(EnumAchievementDataStat _stat, int _value)
		{
			this.SetStatValue(_stat, _value);
		}

		// Token: 0x0600D0EE RID: 53486 RVA: 0x004C694F File Offset: 0x004C4B4F
		public virtual void SetAchievementStat(EnumAchievementDataStat _stat, float _value)
		{
			this.SetStatValue(_stat, _value);
		}

		// Token: 0x0600D0EF RID: 53487 RVA: 0x004C695E File Offset: 0x004C4B5E
		public object GetAchievementStatValue(EnumAchievementDataStat _stat)
		{
			if (_stat == EnumAchievementDataStat.Last)
			{
				return 0;
			}
			return this.statValues[(int)_stat];
		}

		// Token: 0x0600D0F0 RID: 53488 RVA: 0x004C6974 File Offset: 0x004C4B74
		public bool IsAchievementLocked(EnumAchievementManagerAchievement _achievement)
		{
			return this.achievementStatuses[_achievement];
		}

		// Token: 0x0600D0F1 RID: 53489 RVA: 0x004C6984 File Offset: 0x004C4B84
		public void LockAchievement(EnumAchievementManagerAchievement _achievement)
		{
			Dictionary<EnumAchievementManagerAchievement, bool> obj = this.achievementStatuses;
			lock (obj)
			{
				this.achievementStatuses[_achievement] = true;
			}
			Action<EnumAchievementManagerAchievement> action = this.statCompleteCallback;
			if (action == null)
			{
				return;
			}
			action(_achievement);
		}

		// Token: 0x0600D0F2 RID: 53490 RVA: 0x004C69DC File Offset: 0x004C4BDC
		public float GetGameProgress()
		{
			float num = 0f;
			foreach (AchievementData.AchievementStatDecl achievementStatDecl in AchievementData.propertyList)
			{
				int count = achievementStatDecl.achievementInfos.Count;
				for (int j = 0; j < count; j++)
				{
					AchievementData.AchievementInfo achievementInfo = achievementStatDecl.achievementInfos[j];
					if (this.IsAchievementLocked(achievementInfo.achievement))
					{
						num += achievementInfo.progressContribution;
					}
				}
			}
			return num;
		}

		// Token: 0x0600D0F3 RID: 53491 RVA: 0x004C6A54 File Offset: 0x004C4C54
		public void DebugPrintStats()
		{
			for (int i = 0; i < 19; i++)
			{
				string[] array = new string[6];
				array[0] = "Stat: ";
				array[1] = i.ToString();
				array[2] = ", ";
				int num = 3;
				EnumAchievementDataStat enumAchievementDataStat = (EnumAchievementDataStat)i;
				array[num] = enumAchievementDataStat.ToString();
				array[4] = " = ";
				int num2 = 5;
				object obj = this.statValues[i];
				array[num2] = ((obj != null) ? obj.ToString() : null);
				string line = string.Concat(array);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(line);
			}
		}

		// Token: 0x0600D0F4 RID: 53492 RVA: 0x004C6AD0 File Offset: 0x004C4CD0
		public byte[] Serialize()
		{
			byte[] result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				try
				{
					BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
					binaryWriter.Write('t');
					binaryWriter.Write('t');
					binaryWriter.Write('w');
					binaryWriter.Write(0);
					binaryWriter.Write(1U);
					binaryWriter.Write(Constants.cVersionInformation.LongString);
					for (int i = 0; i < 19; i++)
					{
						BinaryWriter binaryWriter2 = binaryWriter;
						EnumAchievementDataStat enumAchievementDataStat = (EnumAchievementDataStat)i;
						binaryWriter2.Write(enumAchievementDataStat.ToString());
						binaryWriter.Write(AchievementData.propertyList[i].type.ToString());
						object[] obj = this.statValues;
						lock (obj)
						{
							if (AchievementData.propertyList[i].type == EnumStatType.Int)
							{
								binaryWriter.Write(Convert.ToInt32(this.statValues[i]));
							}
							else
							{
								binaryWriter.Write(Convert.ToSingle(this.statValues[i]));
							}
						}
					}
					foreach (KeyValuePair<EnumAchievementManagerAchievement, bool> keyValuePair in this.achievementStatuses)
					{
						binaryWriter.Write(keyValuePair.Key.ToString());
						Dictionary<EnumAchievementManagerAchievement, bool> obj2 = this.achievementStatuses;
						lock (obj2)
						{
							binaryWriter.Write(keyValuePair.Value);
						}
					}
					result = memoryStream.ToArray();
				}
				catch (Exception ex)
				{
					Log.Error("Writing header of achievement data: " + ex.Message);
				}
			}
			return result;
		}

		// Token: 0x0600D0F5 RID: 53493 RVA: 0x004C6CF0 File Offset: 0x004C4EF0
		public void DeserializeBytes(byte[] _bytes)
		{
			Stream stream = null;
			try
			{
				stream = new MemoryStream(_bytes, false);
				this.DeserializeFromStream(stream);
				stream.Close();
			}
			catch (Exception ex)
			{
				Log.Error("Reading header of achievements: " + ex.Message);
			}
			if (stream != null)
			{
				stream.Close();
			}
		}

		// Token: 0x0600D0F6 RID: 53494 RVA: 0x004C6D48 File Offset: 0x004C4F48
		[PublicizedFrom(EAccessModifier.Private)]
		public bool DeserializeFromStream(Stream _stream)
		{
			BinaryReader binaryReader = new BinaryReader(_stream);
			long num = 0L;
			long length = binaryReader.BaseStream.Length;
			if (binaryReader.ReadChar() != 't' || binaryReader.ReadChar() != 't' || binaryReader.ReadChar() != 'w' || binaryReader.ReadChar() != '\0')
			{
				return false;
			}
			num += 2L;
			this.version = binaryReader.ReadUInt32();
			num += 4L;
			if (this.version != 1U)
			{
				return false;
			}
			string text = binaryReader.ReadString();
			num += (long)(text.Length * 2);
			if (text != Constants.cVersionInformation.LongString)
			{
				Log.Warning("Loaded achievement data from different version: '" + text + "'");
			}
			for (int i = 0; i < 19; i++)
			{
				string text2 = binaryReader.ReadString();
				num += (long)(text2.Length * 2);
				if ((EnumAchievementDataStat)Enum.Parse(typeof(EnumAchievementDataStat), text2) != (EnumAchievementDataStat)i)
				{
					return false;
				}
				string text3 = binaryReader.ReadString();
				num += (long)(text3.Length * 2);
				EnumStatType enumStatType = (EnumStatType)Enum.Parse(typeof(EnumStatType), text3);
				if (AchievementData.propertyList[i].type != enumStatType)
				{
					return false;
				}
				if (enumStatType == EnumStatType.Int)
				{
					this.statValues[i] = binaryReader.ReadInt32();
					num += 4L;
				}
				else
				{
					this.statValues[i] = binaryReader.ReadSingle();
					num += 4L;
				}
			}
			foreach (KeyValuePair<EnumAchievementManagerAchievement, bool> keyValuePair in this.achievementStatuses)
			{
				string text4 = binaryReader.ReadString();
				num += (long)(text4.Length * 2);
				EnumAchievementManagerAchievement key = (EnumAchievementManagerAchievement)Enum.Parse(typeof(EnumAchievementManagerAchievement), text4);
				binaryReader.ReadBoolean();
				num += 1L;
				this.achievementStatuses[key] = false;
			}
			return true;
		}

		// Token: 0x0600D0F7 RID: 53495 RVA: 0x004C6F34 File Offset: 0x004C5134
		public static void Deserialize(byte[] _bytes, Action<AchievementData> _callback)
		{
			AchievementData achievementData = null;
			TaskManager.Schedule(delegate()
			{
				achievementData = new AchievementData();
				try
				{
					achievementData.DeserializeBytes(_bytes);
				}
				catch (Exception)
				{
					achievementData = null;
				}
			}, delegate()
			{
				Action<AchievementData> callback = _callback;
				if (callback == null)
				{
					return;
				}
				callback(achievementData);
			});
		}

		// Token: 0x0400A023 RID: 40995
		public const string cDataName = "achievements.bin";

		// Token: 0x0400A024 RID: 40996
		[PublicizedFrom(EAccessModifier.Private)]
		public const int CurrentSaveVersion = 1;

		// Token: 0x0400A025 RID: 40997
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly AchievementData.AchievementStatDecl[] propertyList = new AchievementData.AchievementStatDecl[]
		{
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.StoneAxeCrafted, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.StoneAxe, 1.5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.BedrollPlaced, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.Bedroll, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.BleedOutStopped, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.BleedOut, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.WoodFrameCrafted, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.WoodFrame, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.LandClaimPlaced, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.LandClaim, 1.5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.ItemsCrafted, EnumStatType.Int, AchievementData.EnumUpdateType.Sum, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(50, EnumAchievementManagerAchievement.Items50, 2f),
				new AchievementData.AchievementInfo(500, EnumAchievementManagerAchievement.Items500, 2f),
				new AchievementData.AchievementInfo(1500, EnumAchievementManagerAchievement.Items1500, 2f),
				new AchievementData.AchievementInfo(5000, EnumAchievementManagerAchievement.Items5000, 5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.ZombiesKilled, EnumStatType.Int, AchievementData.EnumUpdateType.Sum, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(10, EnumAchievementManagerAchievement.Zombies10, 2f),
				new AchievementData.AchievementInfo(100, EnumAchievementManagerAchievement.Zombies100, 2f),
				new AchievementData.AchievementInfo(500, EnumAchievementManagerAchievement.Zombies500, 2f),
				new AchievementData.AchievementInfo(2500, EnumAchievementManagerAchievement.Zombies2500, 5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.PlayersKilled, EnumStatType.Int, AchievementData.EnumUpdateType.Sum, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.Players1, 1f),
				new AchievementData.AchievementInfo(5, EnumAchievementManagerAchievement.Players5, 2f),
				new AchievementData.AchievementInfo(10, EnumAchievementManagerAchievement.Players10, 2f),
				new AchievementData.AchievementInfo(25, EnumAchievementManagerAchievement.Players25, 5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.KMTravelled, EnumStatType.Float, AchievementData.EnumUpdateType.Sum, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(10, EnumAchievementManagerAchievement.Travel10, 0.5f),
				new AchievementData.AchievementInfo(50, EnumAchievementManagerAchievement.Travel50, 1f),
				new AchievementData.AchievementInfo(250, EnumAchievementManagerAchievement.Travel250, 2f),
				new AchievementData.AchievementInfo(1000, EnumAchievementManagerAchievement.Travel1000, 5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.LongestLifeLived, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(60, EnumAchievementManagerAchievement.Life60Minute, 1f),
				new AchievementData.AchievementInfo(180, EnumAchievementManagerAchievement.Life180Minute, 2f),
				new AchievementData.AchievementInfo(600, EnumAchievementManagerAchievement.Life600Minute, 2.5f),
				new AchievementData.AchievementInfo(1680, EnumAchievementManagerAchievement.Life1680Minute, 7.5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.Deaths, EnumStatType.Int, AchievementData.EnumUpdateType.Sum, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.Die1, 1f),
				new AchievementData.AchievementInfo(7, EnumAchievementManagerAchievement.Die7, 1.5f),
				new AchievementData.AchievementInfo(14, EnumAchievementManagerAchievement.Die14, 2f),
				new AchievementData.AchievementInfo(28, EnumAchievementManagerAchievement.Die28, 2.5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.HeightAchieved, EnumStatType.Int, AchievementData.EnumUpdateType.Replace, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.Height255, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.DepthAchieved, EnumStatType.Int, AchievementData.EnumUpdateType.Replace, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.Height0, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.SubZeroNakedSwim, EnumStatType.Int, AchievementData.EnumUpdateType.Replace, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.SubZeroNaked, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.KilledWith44Magnum, EnumStatType.Int, AchievementData.EnumUpdateType.Sum, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(44, EnumAchievementManagerAchievement.Kills44Mag, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.LegBroken, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(1, EnumAchievementManagerAchievement.LegBreak, 1f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.HighestFortitude, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(4, EnumAchievementManagerAchievement.Fortitude4, 1f),
				new AchievementData.AchievementInfo(6, EnumAchievementManagerAchievement.Fortitude6, 2f),
				new AchievementData.AchievementInfo(8, EnumAchievementManagerAchievement.Fortitude8, 2f),
				new AchievementData.AchievementInfo(10, EnumAchievementManagerAchievement.Fortitude10, 5f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.HighestGamestage, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(10, EnumAchievementManagerAchievement.Gamestage10, 0.5f),
				new AchievementData.AchievementInfo(25, EnumAchievementManagerAchievement.Gamestage25, 1f),
				new AchievementData.AchievementInfo(50, EnumAchievementManagerAchievement.Gamestage50, 2f),
				new AchievementData.AchievementInfo(100, EnumAchievementManagerAchievement.Gamestage100, 5f),
				new AchievementData.AchievementInfo(200, EnumAchievementManagerAchievement.Gamestage200, 10f)
			}),
			new AchievementData.AchievementStatDecl(EnumAchievementDataStat.HighestPlayerLevel, EnumStatType.Int, AchievementData.EnumUpdateType.Max, new List<AchievementData.AchievementInfo>
			{
				new AchievementData.AchievementInfo(7, EnumAchievementManagerAchievement.PlayerLevel7, 0.5f),
				new AchievementData.AchievementInfo(28, EnumAchievementManagerAchievement.PlayerLevel28, 1f),
				new AchievementData.AchievementInfo(70, EnumAchievementManagerAchievement.PlayerLevel70, 2f),
				new AchievementData.AchievementInfo(140, EnumAchievementManagerAchievement.PlayerLevel140, 5f),
				new AchievementData.AchievementInfo(300, EnumAchievementManagerAchievement.PlayerLevel300, 10f)
			})
		};

		// Token: 0x0400A026 RID: 40998
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly EnumDictionary<EnumAchievementManagerAchievement, EnumAchievementDataStat> achievementToStat = AchievementData.CreateAchievementToStat();

		// Token: 0x0400A027 RID: 40999
		[PublicizedFrom(EAccessModifier.Private)]
		public uint version;

		// Token: 0x0400A029 RID: 41001
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object[] statValues;

		// Token: 0x0400A02A RID: 41002
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<EnumAchievementManagerAchievement, bool> achievementStatuses;

		// Token: 0x0400A02B RID: 41003
		[PublicizedFrom(EAccessModifier.Private)]
		public Action<EnumAchievementManagerAchievement> statCompleteCallback;

		// Token: 0x02001B37 RID: 6967
		public enum EnumUpdateType
		{
			// Token: 0x0400A02D RID: 41005
			Sum,
			// Token: 0x0400A02E RID: 41006
			Replace,
			// Token: 0x0400A02F RID: 41007
			Max
		}

		// Token: 0x02001B38 RID: 6968
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly struct AchievementStatDecl
		{
			// Token: 0x0600D0F9 RID: 53497 RVA: 0x004C75EC File Offset: 0x004C57EC
			public AchievementStatDecl(EnumAchievementDataStat _name, EnumStatType _type, AchievementData.EnumUpdateType _updateType, List<AchievementData.AchievementInfo> _achievementPairs)
			{
				this.name = _name;
				this.type = _type;
				this.updateType = _updateType;
				this.achievementInfos = _achievementPairs;
			}

			// Token: 0x0400A030 RID: 41008
			public readonly EnumAchievementDataStat name;

			// Token: 0x0400A031 RID: 41009
			public readonly EnumStatType type;

			// Token: 0x0400A032 RID: 41010
			public readonly AchievementData.EnumUpdateType updateType;

			// Token: 0x0400A033 RID: 41011
			public readonly List<AchievementData.AchievementInfo> achievementInfos;
		}

		// Token: 0x02001B39 RID: 6969
		public readonly struct AchievementInfo
		{
			// Token: 0x0600D0FA RID: 53498 RVA: 0x004C760B File Offset: 0x004C580B
			public AchievementInfo(object _triggerPoint, EnumAchievementManagerAchievement _achievement, float _progressContribution)
			{
				this.triggerPoint = _triggerPoint;
				this.achievement = _achievement;
				this.progressContribution = _progressContribution;
			}

			// Token: 0x0400A034 RID: 41012
			public readonly object triggerPoint;

			// Token: 0x0400A035 RID: 41013
			public readonly EnumAchievementManagerAchievement achievement;

			// Token: 0x0400A036 RID: 41014
			public readonly float progressContribution;
		}
	}
}
