using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000084 RID: 132
[Serializable]
public class AutomationScript
{
	// Token: 0x0600028C RID: 652 RVA: 0x00014538 File Offset: 0x00012738
	public string ResolveSessionDir()
	{
		if (!string.IsNullOrEmpty(this.defaultSessionDir))
		{
			return this.defaultSessionDir;
		}
		return string.Concat(new string[]
		{
			Constants.cVersionInformation.ShortString,
			"_",
			GamePrefs.GetString(EnumGamePrefs.GameWorld),
			"_",
			GamePrefs.GetString(EnumGamePrefs.GameName),
			DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss.fff")
		});
	}

	// Token: 0x0600028D RID: 653 RVA: 0x000145AC File Offset: 0x000127AC
	public int CountStepsOfType(AutomationStep.StepType stepType)
	{
		return this.steps.Count((AutomationStep s) => s.type == stepType);
	}

	// Token: 0x0600028E RID: 654 RVA: 0x000145E0 File Offset: 0x000127E0
	public string Validate()
	{
		if (this.steps.Count == 0)
		{
			return "Script '" + this.name + "' has no steps.";
		}
		int num = 0;
		for (int i = 0; i < this.steps.Count; i++)
		{
			AutomationStep automationStep = this.steps[i];
			if (automationStep.type == AutomationStep.StepType.LoadGame)
			{
				if (string.IsNullOrEmpty(automationStep.world))
				{
					return string.Format("Script '{0}' step [{1}] LoadGame: 'world' is empty.", this.name, i);
				}
				if (string.IsNullOrEmpty(automationStep.gameName))
				{
					return string.Format("Script '{0}' step [{1}] LoadGame: 'gameName' is empty.", this.name, i);
				}
			}
			if (automationStep.type == AutomationStep.StepType.MovePingPong)
			{
				if (automationStep.duration <= 0f)
				{
					return string.Format("Script '{0}' step [{1}] MovePingPong: duration must be > 0.", this.name, i);
				}
				if (automationStep.pingPongCount < 1)
				{
					return string.Format("Script '{0}' step [{1}] MovePingPong: pingPongCount must be >= 1.", this.name, i);
				}
				if (automationStep.position == automationStep.positionB)
				{
					return string.Format("Script '{0}' step [{1}] MovePingPong: point A and point B are identical.", this.name, i);
				}
			}
			if (automationStep.type == AutomationStep.StepType.StartPerfSession)
			{
				if (automationStep.runCount < 1)
				{
					return string.Format("Script '{0}' step [{1}] StartPerfSession: runCount must be >= 1.", this.name, i);
				}
				if (num > 0)
				{
					return string.Format("Script '{0}' step [{1}] StartPerfSession: nested sessions are not supported.", this.name, i);
				}
				num++;
			}
			if (automationStep.type == AutomationStep.StepType.StopPerfSession)
			{
				if (num == 0)
				{
					return string.Format("Script '{0}' step [{1}] StopPerfSession: no matching StartPerfSession.", this.name, i);
				}
				num--;
			}
		}
		if (num > 0)
		{
			return "Script '" + this.name + "': StartPerfSession without matching StopPerfSession.";
		}
		return null;
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00014790 File Offset: 0x00012990
	public string Describe()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Script '{0}'  steps={1}", this.name, this.steps.Count));
		for (int i = 0; i < this.steps.Count; i++)
		{
			stringBuilder.AppendLine(this.steps[i].Describe(i));
		}
		return stringBuilder.ToString().TrimEnd();
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00014804 File Offset: 0x00012A04
	public static string GetScriptsDirectory()
	{
		string text = AutomationRunner.GetAutomationDataPath() + "Scripts";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	// Token: 0x06000291 RID: 657 RVA: 0x00014831 File Offset: 0x00012A31
	public void SaveToFile(string scriptName = null)
	{
		Log.Error("[AutomationScript] Disabled for this build type.");
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0001483D File Offset: 0x00012A3D
	public static AutomationScript LoadFromFile(string scriptName)
	{
		Log.Error("[AutomationRunner] Disabled for this build type.");
		return null;
	}

	// Token: 0x06000293 RID: 659 RVA: 0x0001484C File Offset: 0x00012A4C
	public static List<string> ListSavedScripts()
	{
		return (from n in Directory.GetFiles(AutomationScript.GetScriptsDirectory(), "*.json").Select(new Func<string, string>(Path.GetFileNameWithoutExtension))
		orderby n
		select n).ToList<string>();
	}

	// Token: 0x0400034E RID: 846
	public string name = "unnamed";

	// Token: 0x0400034F RID: 847
	public string defaultSessionDir = string.Empty;

	// Token: 0x04000350 RID: 848
	public List<AutomationStep> steps = new List<AutomationStep>();

	// Token: 0x04000351 RID: 849
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly JsonSerializerSettings s_jsonSettings = new JsonSerializerSettings
	{
		Formatting = Formatting.Indented,
		Converters = 
		{
			new StringEnumConverter(),
			new AutomationScript.Vector3Converter()
		}
	};

	// Token: 0x02000085 RID: 133
	[PublicizedFrom(EAccessModifier.Private)]
	public class Vector3Converter : JsonConverter<Vector3>
	{
		// Token: 0x06000296 RID: 662 RVA: 0x00014900 File Offset: 0x00012B00
		public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("z");
			writer.WriteValue(value.z);
			writer.WriteEndObject();
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00014960 File Offset: 0x00012B60
		public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JObject jobject = JObject.Load(reader);
			return new Vector3(jobject.Value<float>("x"), jobject.Value<float>("y"), jobject.Value<float>("z"));
		}
	}
}
