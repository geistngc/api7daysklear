using System;
using System.Collections.Generic;
using System.IO;

namespace SDF
{
	// Token: 0x02001695 RID: 5781
	public class SdfFile
	{
		// Token: 0x0600B568 RID: 46440 RVA: 0x0043B252 File Offset: 0x00439452
		public SdfFile(string path)
		{
			this.data = new SdfData();
			this.filePath = path;
		}

		// Token: 0x0600B569 RID: 46441 RVA: 0x0043B26C File Offset: 0x0043946C
		public void Load()
		{
			try
			{
				this.valuesChanged = false;
				if (SdFile.Exists(this.filePath))
				{
					using (Stream stream = SdFile.Open(this.filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						this.data.Nodes = SdfReader.Read(stream);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error opening SDF file: " + ex.Message);
			}
		}

		// Token: 0x0600B56A RID: 46442 RVA: 0x0043B2F4 File Offset: 0x004394F4
		public void Save()
		{
			try
			{
				if (this.valuesChanged)
				{
					if (!SdDirectory.Exists(Path.GetDirectoryName(this.filePath)))
					{
						SdDirectory.CreateDirectory(Path.GetDirectoryName(this.filePath));
					}
					using (Stream stream = SdFile.Open(this.filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
					{
						SdfWriter.Write(stream, this.data.Nodes);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error("Error opening SDF file:");
				Log.Exception(e);
			}
		}

		// Token: 0x0600B56B RID: 46443 RVA: 0x0043B38C File Offset: 0x0043958C
		public void Set(string name, int val)
		{
			this.data.Add(new SdfInt(name, val));
			this.valuesChanged = true;
		}

		// Token: 0x0600B56C RID: 46444 RVA: 0x0043B3A8 File Offset: 0x004395A8
		public void Set(string name, float val)
		{
			this.data.Add(new SdfFloat(name, val));
			this.valuesChanged = true;
		}

		// Token: 0x0600B56D RID: 46445 RVA: 0x0043B3C4 File Offset: 0x004395C4
		public void Set(string name, string val)
		{
			this.Set(name, val, false);
		}

		// Token: 0x0600B56E RID: 46446 RVA: 0x0043B3CF File Offset: 0x004395CF
		public void Set(string name, string val, bool isBinary)
		{
			if (!isBinary)
			{
				this.data.Add(new SdfString(name, val));
			}
			else
			{
				this.data.Add(new SdfBinary(name, val));
			}
			this.valuesChanged = true;
		}

		// Token: 0x0600B56F RID: 46447 RVA: 0x0043B403 File Offset: 0x00439603
		public void Set(string name, byte[] byteArray)
		{
			this.data.Add(new SdfByteArray(name, byteArray));
		}

		// Token: 0x0600B570 RID: 46448 RVA: 0x0043B418 File Offset: 0x00439618
		public void Set(string name, bool val)
		{
			this.data.Add(new SdfBool(name, val));
			this.valuesChanged = true;
		}

		// Token: 0x0600B571 RID: 46449 RVA: 0x0043B434 File Offset: 0x00439634
		public float? GetFloat(string name)
		{
			return this.data.GetFloat(name);
		}

		// Token: 0x0600B572 RID: 46450 RVA: 0x0043B442 File Offset: 0x00439642
		public int? GetInt(string name)
		{
			return this.data.GetInt(name);
		}

		// Token: 0x0600B573 RID: 46451 RVA: 0x0043B450 File Offset: 0x00439650
		public string GetString(string name)
		{
			return this.GetString(name, false);
		}

		// Token: 0x0600B574 RID: 46452 RVA: 0x0043B45A File Offset: 0x0043965A
		public string GetString(string name, bool isBinary)
		{
			if (!isBinary)
			{
				return this.data.GetString(name);
			}
			return Utils.FromBase64(this.data.GetString(name));
		}

		// Token: 0x0600B575 RID: 46453 RVA: 0x0043B47D File Offset: 0x0043967D
		public byte[] GetByteArray(string name)
		{
			return this.data.GetByteArray(name);
		}

		// Token: 0x0600B576 RID: 46454 RVA: 0x0043B48B File Offset: 0x0043968B
		public bool? GetBool(string name)
		{
			return this.data.GetBool(name);
		}

		// Token: 0x0600B577 RID: 46455 RVA: 0x0043B499 File Offset: 0x00439699
		public void Remove(string name)
		{
			this.data.Remove(name);
			this.valuesChanged = true;
		}

		// Token: 0x0600B578 RID: 46456 RVA: 0x0043B4B0 File Offset: 0x004396B0
		public string[] GetKeys()
		{
			string[] array = new string[this.data.Nodes.Count];
			this.data.Nodes.CopyKeysTo(array);
			return array;
		}

		// Token: 0x0600B579 RID: 46457 RVA: 0x0043B4E8 File Offset: 0x004396E8
		public string[] GetStoredGamePrefs()
		{
			string[] array = new string[this.data.Nodes.Count];
			this.data.Nodes.CopyKeysTo(array);
			return array;
		}

		// Token: 0x0600B57A RID: 46458 RVA: 0x0043B520 File Offset: 0x00439720
		public void CopyFrom(SdfFile other)
		{
			foreach (KeyValuePair<string, SdfTag> keyValuePair in other.data.Nodes)
			{
				string text;
				SdfTag sdfTag;
				keyValuePair.Deconstruct(out text, out sdfTag);
				string key = text;
				SdfTag value = sdfTag;
				this.data.Nodes[key] = value;
			}
			this.valuesChanged = true;
		}

		// Token: 0x04008814 RID: 34836
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly SdfData data;

		// Token: 0x04008815 RID: 34837
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string filePath;

		// Token: 0x04008816 RID: 34838
		[PublicizedFrom(EAccessModifier.Private)]
		public bool valuesChanged;
	}
}
