using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000275 RID: 629
[Preserve]
public class ConsoleCmdWorldChunkReset : ConsoleCmdAbstract
{
	// Token: 0x060012A0 RID: 4768 RVA: 0x0007455B File Offset: 0x0007275B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"worldchunkreset",
			"wcr"
		};
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00074573 File Offset: 0x00072773
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Resets all unprotected chunks across the world.";
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x0007457A File Offset: 0x0007277A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage: wcr [-g|-gmode <n>]\n\nExamples: \n'wcr' - reset all unprotected chunks using the default grouping.\n'wcr -g 2' - reset all unprotected chunks, grouping mode 2.\n\nGrouping modes (-g|-gmode, default 3): \n'1' - NoGrouping: Chunks are reset based on their own protection flags only.\n'2' - SeparatePOIs: Each POI gets the combined protection flags from all chunks overlapping the POI.\n'3' - GroupedPOIs: Like SeparatePOIs, but POIs with overlapping chunks also merge into groups. Standard reset; should not cause POI discontinuities.\n\nNotes: \n - All chunk protection statuses are respected. To bypass protections, see 'rr'.\n - Use with caution! This operation permanently deletes all saved data for affected chunks.";
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x00074584 File Offset: 0x00072784
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		EnumResetUnprotectedChunksGroupingMode enumResetUnprotectedChunksGroupingMode;
		if (!ChunkResetCommandHelpers.TryParseGroupingMode(_params, out enumResetUnprotectedChunksGroupingMode))
		{
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Running world chunk reset: -g {0} ({1}).", (int)enumResetUnprotectedChunksGroupingMode, enumResetUnprotectedChunksGroupingMode));
		ChunkResetCommandHelpers.ExecuteReset(ChunkProtectionLevel.All, enumResetUnprotectedChunksGroupingMode, null, null, "World chunk reset");
	}
}
