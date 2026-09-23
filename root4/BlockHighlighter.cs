using System;
using UnityEngine;

// Token: 0x0200091D RID: 2333
public static class BlockHighlighter
{
	// Token: 0x0600439B RID: 17307 RVA: 0x001A5583 File Offset: 0x001A3783
	public static void AddBlock(Vector3i _pos)
	{
		BlockHighlighter.EnforceGo();
		BlockHighlighter.EnforceTemplateLoaded();
		UnityEngine.Object.Instantiate<GameObject>(BlockHighlighter.blockPrefab, BlockHighlighter.topGameObject.transform).transform.position = _pos.ToVector3() + BlockHighlighter.halfBlockOffset;
	}

	// Token: 0x0600439C RID: 17308 RVA: 0x001A55BE File Offset: 0x001A37BE
	[PublicizedFrom(EAccessModifier.Private)]
	public static void EnforceGo()
	{
		if (BlockHighlighter.topGameObject != null)
		{
			return;
		}
		BlockHighlighter.topGameObject = new GameObject("BlockHighlighter");
	}

	// Token: 0x0600439D RID: 17309 RVA: 0x001A55DD File Offset: 0x001A37DD
	[PublicizedFrom(EAccessModifier.Private)]
	public static void EnforceTemplateLoaded()
	{
		if (BlockHighlighter.blockPrefab != null)
		{
			return;
		}
		BlockHighlighter.blockPrefab = DataLoader.LoadAsset<GameObject>("@:Entities/Misc/block_highlightPrefab.prefab", false);
	}

	// Token: 0x0600439E RID: 17310 RVA: 0x001A55FD File Offset: 0x001A37FD
	public static void Cleanup()
	{
		if (BlockHighlighter.topGameObject != null)
		{
			UnityEngine.Object.Destroy(BlockHighlighter.topGameObject);
			BlockHighlighter.topGameObject = null;
		}
	}

	// Token: 0x040036A3 RID: 13987
	[PublicizedFrom(EAccessModifier.Private)]
	public const string TemplatePath = "@:Entities/Misc/block_highlightPrefab.prefab";

	// Token: 0x040036A4 RID: 13988
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector3 halfBlockOffset = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x040036A5 RID: 13989
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject topGameObject;

	// Token: 0x040036A6 RID: 13990
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject blockPrefab;
}
