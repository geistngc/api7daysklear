using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000274 RID: 628
[Preserve]
public class ConsoleCmdRegionReset : ConsoleCmdAbstract
{
	// Token: 0x0600129B RID: 4763 RVA: 0x00074484 File Offset: 0x00072684
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"regionreset",
			"rr"
		};
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x0007449C File Offset: 0x0007269C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Resets chunks within a target region, or for the entire map.";
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x000744A3 File Offset: 0x000726A3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage: rr [-p|-pmode <n>] [-g|-gmode <n>] [-r|-region <x> <z>]\n\nExamples: \n'rr' - reset all unprotected chunks in all regions (defaults).\n'rr -p 2' - all regions, protection mode 2.\n'rr -r 1 -2' - only region (1,-2), default modes.\n'rr -r 1 -2 -p 0 -g 3' - region (1,-2) with protection mode 0 and grouping mode 3.\n\nProtection modes (-p|-pmode, default 0): \n'0' - Default: All protection statuses are respected, including the dynamic protection of synced chunks around active player position(s).\n'1' - EXPERIMENTAL: Most protection statuses are respected, excepting the dynamic protection of synced chunks around active player position(s). Chunks whose *only* protection status is \"CurrentlySynced\" will be treated as unprotected and are subject to being reset.\n'2' - EXPERIMENTAL: All protection statuses are ignored. Every chunk in the target area will be reset whether protected or not.\n'3' - EXPERIMENTAL: Most protection statuses are ignored, excepting the dynamic protection of synced chunks around active player position(s). Chunks whose protection status includes \"CurrentlySynced\" will be treated as protected; all other chunks are subject to being reset.\n\nGrouping modes (-g|-gmode, default 3): \n'1' - NoGrouping: Chunks are reset based on their own protection flags only.\n'2' - SeparatePOIs: Each POI gets the combined protection flags from all chunks overlapping the POI.\n'3' - GroupedPOIs: Like SeparatePOIs, but POIs with overlapping chunks also merge into groups. Standard reset; should not cause POI discontinuities.\n\nNotes: \n - Use with caution! This operation permanently deletes all saved data for affected chunks.\n - The experimental protection modes are provided for debug purposes only. They bypass various protections in order to force chunks to be reset. This can cause a significant hitch whilst any synced chunks are regenerated, and may cause other side effects such as failing to clean up nav markers for land claims, etc.";
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x000744AC File Offset: 0x000726AC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		ChunkProtectionLevel protectionMask;
		int num;
		if (!ChunkResetCommandHelpers.TryParseProtectionMode(_params, out protectionMask, out num))
		{
			return;
		}
		EnumResetUnprotectedChunksGroupingMode enumResetUnprotectedChunksGroupingMode;
		if (!ChunkResetCommandHelpers.TryParseGroupingMode(_params, out enumResetUnprotectedChunksGroupingMode))
		{
			return;
		}
		int? regionX;
		int? regionZ;
		if (!ChunkResetCommandHelpers.TryParseRegion(_params, out regionX, out regionZ))
		{
			return;
		}
		string text = (regionX != null) ? string.Format("-r {0} {1}", regionX.Value, regionZ.Value) : "all regions";
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Running region reset: -p {0}, -g {1} ({2}), {3}.", new object[]
		{
			num,
			(int)enumResetUnprotectedChunksGroupingMode,
			enumResetUnprotectedChunksGroupingMode,
			text
		}));
		ChunkResetCommandHelpers.ExecuteReset(protectionMask, enumResetUnprotectedChunksGroupingMode, regionX, regionZ, "Region reset");
	}
}
