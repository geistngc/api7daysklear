using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020002A2 RID: 674
[Preserve]
public class ConsoleCmdSquareSpiral : ConsoleCmdAbstract
{
	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06001396 RID: 5014 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x00077BE0 File Offset: 0x00075DE0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"squarespiral",
			"sqs"
		};
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x00077BF8 File Offset: 0x00075DF8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Move the player chunk by chunk in a square spiral. Will start off paused and required un-pausing. Also gives god mode and flying at the start.";
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x00077BFF File Offset: 0x00075DFF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "<s[tart] [chunks per auto-pause]|p[ause]|r[eset]|waitmode [minimal|meshes|displayed]>";
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x00077C06 File Offset: 0x00075E06
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!this.ExecuteInternal(_params, _senderInfo))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
		}
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x00077C24 File Offset: 0x00075E24
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ExecuteInternal(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!GameManager.Instance.World.GetPrimaryPlayer())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No local player! (Are you in-game?)");
			return true;
		}
		if (_params.Count == 0)
		{
			return false;
		}
		string text = _params[0].ToLowerInvariant();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1697318111U)
		{
			if (num != 367725747U)
			{
				if (num != 1695364032U)
				{
					if (num != 1697318111U)
					{
						return false;
					}
					if (!(text == "start"))
					{
						return false;
					}
				}
				else
				{
					if (!(text == "reset"))
					{
						return false;
					}
					goto IL_17A;
				}
			}
			else
			{
				if (!(text == "waitmode"))
				{
					return false;
				}
				if (_params.Count > 1)
				{
					string a = _params[1].ToLowerInvariant();
					if (a == "minimal")
					{
						this.waitMode = ConsoleCmdSquareSpiral.WaitMode.SingleChunkDecorated;
						this.chunkCondition = ChunkConditions.Decorated;
						return true;
					}
					if (a == "meshes")
					{
						this.waitMode = ConsoleCmdSquareSpiral.WaitMode.SurroundingMeshesCopied;
						this.chunkCondition = ChunkConditions.MeshesCopied;
						return true;
					}
					if (a == "displayed")
					{
						this.waitMode = ConsoleCmdSquareSpiral.WaitMode.SurroundingChunksDisplayed;
						this.chunkCondition = ChunkConditions.Displayed;
						return true;
					}
				}
				return false;
			}
		}
		else
		{
			if (num <= 4111221743U)
			{
				if (num != 1887753101U)
				{
					if (num != 4111221743U)
					{
						return false;
					}
					if (!(text == "p"))
					{
						return false;
					}
				}
				else if (!(text == "pause"))
				{
					return false;
				}
				this.MacroPause();
				return true;
			}
			if (num != 4127999362U)
			{
				if (num != 4144776981U)
				{
					return false;
				}
				if (!(text == "r"))
				{
					return false;
				}
				goto IL_17A;
			}
			else if (!(text == "s"))
			{
				return false;
			}
		}
		if (_params.Count > 2)
		{
			return false;
		}
		int chunksPerAutoPause = 0;
		if (_params.Count > 1 && !int.TryParse(_params[1], out chunksPerAutoPause))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Failed to parse as int " + _params[1]);
			return false;
		}
		this.MacroStart(chunksPerAutoPause);
		return true;
		IL_17A:
		this.MacroReset();
		return true;
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x00077E34 File Offset: 0x00076034
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroStart(int _chunksPerAutoPause)
	{
		if (this.m_coroutine != null)
		{
			return;
		}
		Coroutine coroutine = ThreadManager.StartCoroutine(this.CoroutineSpiral(_chunksPerAutoPause));
		if (this.m_isRunning)
		{
			this.m_coroutine = coroutine;
		}
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x00077E66 File Offset: 0x00076066
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroPause()
	{
		if (this.m_isRunning)
		{
			this.m_isRunning = false;
			return;
		}
		this.m_isRunning = true;
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x00077E7F File Offset: 0x0007607F
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroReset()
	{
		if (this.m_coroutine == null)
		{
			return;
		}
		this.m_isRunning = false;
		ThreadManager.StopCoroutine(this.m_coroutine);
		this.m_coroutine = null;
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x00077EA3 File Offset: 0x000760A3
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator CoroutineSpiral(int _chunksPerAutoPause)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			yield break;
		}
		EntityPlayerLocal player;
		if (!ProfilerGameUtils.TryGetFlyingPlayer(out player))
		{
			yield break;
		}
		this.m_isRunning = true;
		IEnumerator<Vector2i> spiralSequence = this.SpiralSequence();
		float lastY = player.position.y;
		int i = 0;
		DateTime lastPrintTime = DateTime.MinValue;
		while (world == GameManager.Instance.World && !(player != world.GetPrimaryPlayer()))
		{
			if (this.m_isRunning)
			{
				spiralSequence.MoveNext();
				Vector2i chunkPos = spiralSequence.Current;
				int num = i;
				i = num + 1;
				DateTime now = DateTime.Now;
				if (now.Subtract(lastPrintTime).TotalSeconds > 10.0)
				{
					Log.Out("Square Spiral: ({0}, {1}) #{2}", new object[]
					{
						chunkPos.x,
						chunkPos.y,
						i
					});
					lastPrintTime = now;
				}
				Vector2i centreOfChunk = new Vector2i(chunkPos.x * 16 + 8, chunkPos.y * 16 + 8);
				Vector3 rotationEuler = Quaternion.FromToRotation(Vector3.forward, new Vector3((float)(-(float)centreOfChunk.x), 0f, (float)(-(float)centreOfChunk.y))).eulerAngles;
				Chunk chunk = (Chunk)world.GetChunkSync(chunkPos.x, chunkPos.y);
				if (chunk == null)
				{
					player.SetPosition(new Vector3((float)centreOfChunk.x, lastY, (float)centreOfChunk.y), true);
					player.SetRotation(rotationEuler);
					while (chunk == null)
					{
						yield return null;
						chunk = (Chunk)world.GetChunkSync(chunkPos.x, chunkPos.y);
					}
				}
				float y = (float)(world.GetHeight(centreOfChunk.x, centreOfChunk.y) + 10);
				player.SetPosition(new Vector3((float)centreOfChunk.x, y, (float)centreOfChunk.y), true);
				player.SetRotation(rotationEuler);
				lastY = player.position.y;
				switch (this.waitMode)
				{
				case ConsoleCmdSquareSpiral.WaitMode.SingleChunkDecorated:
					yield return ProfilerGameUtils.WaitForSingleChunkToLoad(chunk, this.chunkCondition);
					break;
				case ConsoleCmdSquareSpiral.WaitMode.SurroundingMeshesCopied:
					yield return ProfilerGameUtils.WaitForChunksAroundObserverToLoad(player.ChunkObserver, this.chunkCondition);
					break;
				case ConsoleCmdSquareSpiral.WaitMode.SurroundingChunksDisplayed:
					yield return ProfilerGameUtils.WaitForChunksAroundObserverToLoad(player.ChunkObserver, this.chunkCondition);
					break;
				}
				yield return null;
				if ((_chunksPerAutoPause > 0 && i % _chunksPerAutoPause == 0) || i == 1)
				{
					Log.Out("Square Spiral: ({0}, {1}) #{2} (PAUSED)", new object[]
					{
						chunkPos.x,
						chunkPos.y,
						i
					});
					this.MacroPause();
				}
				rotationEuler = default(Vector3);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
		this.MacroReset();
		yield break;
		yield break;
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x00077EB9 File Offset: 0x000760B9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator<Vector2i> SpiralSequence()
	{
		int x = 0;
		int y = 0;
		int rPos = 1;
		int rNeg = -1;
		ConsoleCmdSquareSpiral.SpiralSequenceState state = ConsoleCmdSquareSpiral.SpiralSequenceState.Left;
		for (;;)
		{
			yield return new Vector2i(x, y);
			switch (state)
			{
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Left:
				if (x == rNeg)
				{
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Down;
					int num = y;
					y = num - 1;
				}
				else
				{
					int num = x;
					x = num - 1;
				}
				break;
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Down:
				if (y == rNeg)
				{
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Right;
					int num = x;
					x = num + 1;
				}
				else
				{
					int num = y;
					y = num - 1;
				}
				break;
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Right:
				if (x == rPos)
				{
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Up;
					int num = y;
					y = num + 1;
				}
				else
				{
					int num = x;
					x = num + 1;
				}
				break;
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Up:
				if (y == rPos)
				{
					int num = rPos;
					rPos = num + 1;
					num = rNeg;
					rNeg = num - 1;
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Left;
					num = x;
					x = num - 1;
				}
				else
				{
					int num = y;
					y = num + 1;
				}
				break;
			}
		}
		yield break;
	}

	// Token: 0x04000D74 RID: 3444
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_isRunning;

	// Token: 0x04000D75 RID: 3445
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine m_coroutine;

	// Token: 0x04000D76 RID: 3446
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkConditions.Delegate chunkCondition = ChunkConditions.Decorated;

	// Token: 0x04000D77 RID: 3447
	[PublicizedFrom(EAccessModifier.Private)]
	public ConsoleCmdSquareSpiral.WaitMode waitMode;

	// Token: 0x020002A3 RID: 675
	[PublicizedFrom(EAccessModifier.Private)]
	public enum WaitMode
	{
		// Token: 0x04000D79 RID: 3449
		SingleChunkDecorated,
		// Token: 0x04000D7A RID: 3450
		SurroundingMeshesCopied,
		// Token: 0x04000D7B RID: 3451
		SurroundingChunksDisplayed
	}

	// Token: 0x020002A4 RID: 676
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SpiralSequenceState
	{
		// Token: 0x04000D7D RID: 3453
		Left,
		// Token: 0x04000D7E RID: 3454
		Down,
		// Token: 0x04000D7F RID: 3455
		Right,
		// Token: 0x04000D80 RID: 3456
		Up
	}
}
