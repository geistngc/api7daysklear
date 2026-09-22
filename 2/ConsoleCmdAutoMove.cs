using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001EA RID: 490
[Preserve]
public class ConsoleCmdAutoMove : ConsoleCmdAbstract
{
	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000F1F RID: 3871 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x000626E2 File Offset: 0x000608E2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"automove"
		};
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x000626F2 File Offset: 0x000608F2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Player auto movement";
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x000626F9 File Offset: 0x000608F9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Parameters:\noff - disable\ngototarget - goto the target position\nsettarget - set target to current player position\nclearlookat - disable look at\nsetlookat - set look at to current player position\nline duration loops <x> <y> <z> - move to x y z (or target) over duration with loops (-loops will ping pong)\norbit duration loops <x> <y> <z> - circle around x y z (or target) over duration with loops (-loops will ping pong)\nrelative x z angle - move by x (left/right) and z (forward) and turn by angle per second";
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x00062700 File Offset: 0x00060900
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (!primaryPlayer)
		{
			return;
		}
		string text = _params[0].ToLower();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2872740362U)
		{
			if (num <= 1260422518U)
			{
				if (num <= 400234023U)
				{
					if (num != 361777697U)
					{
						if (num != 400234023U)
						{
							goto IL_52B;
						}
						if (!(text == "line"))
						{
							goto IL_52B;
						}
						goto IL_2E7;
					}
					else
					{
						if (!(text == "cla"))
						{
							goto IL_52B;
						}
						goto IL_2CE;
					}
				}
				else if (num != 890022063U)
				{
					if (num != 1260422518U)
					{
						goto IL_52B;
					}
					if (!(text == "gt"))
					{
						goto IL_52B;
					}
					goto IL_2B3;
				}
				else if (!(text == "0"))
				{
					goto IL_52B;
				}
			}
			else
			{
				if (num <= 1918877446U)
				{
					if (num != 1263673922U)
					{
						if (num != 1918877446U)
						{
							goto IL_52B;
						}
						if (!(text == "settarget"))
						{
							goto IL_52B;
						}
					}
					else if (!(text == "st"))
					{
						goto IL_52B;
					}
					this.targetPos = primaryPlayer.GetPosition();
					return;
				}
				if (num != 2142508988U)
				{
					if (num != 2872740362U)
					{
						goto IL_52B;
					}
					if (!(text == "off"))
					{
						goto IL_52B;
					}
				}
				else
				{
					if (!(text == "clearlookat"))
					{
						goto IL_52B;
					}
					goto IL_2CE;
				}
			}
			primaryPlayer.EnableAutoMove(false);
			return;
			IL_2CE:
			this.lookAtPos = Vector3.zero;
			return;
		}
		if (num > 3926667934U)
		{
			if (num <= 4144776981U)
			{
				if (num != 3946618703U)
				{
					if (num != 4144776981U)
					{
						goto IL_52B;
					}
					if (!(text == "r"))
					{
						goto IL_52B;
					}
				}
				else
				{
					if (!(text == "orbit"))
					{
						goto IL_52B;
					}
					goto IL_3C9;
				}
			}
			else if (num != 4236415261U)
			{
				if (num != 4239927381U)
				{
					goto IL_52B;
				}
				if (!(text == "setlookat"))
				{
					goto IL_52B;
				}
				goto IL_2DA;
			}
			else if (!(text == "relative"))
			{
				goto IL_52B;
			}
			float velX = 0f;
			if (_params.Count >= 2)
			{
				velX = this.FloatParse(_params[1]);
			}
			float velZ = 0f;
			if (_params.Count >= 3)
			{
				velZ = this.FloatParse(_params[2]);
			}
			float rotVel = 0f;
			if (_params.Count >= 4)
			{
				rotVel = this.FloatParse(_params[3]);
			}
			EntityPlayerLocal.AutoMove autoMove = primaryPlayer.EnableAutoMove(true);
			autoMove.SetLookAt(this.lookAtPos);
			autoMove.StartRelative(velX, velZ, rotVel);
			return;
		}
		if (num <= 3308801681U)
		{
			if (num != 3291586103U)
			{
				if (num != 3308801681U)
				{
					goto IL_52B;
				}
				if (!(text == "sla"))
				{
					goto IL_52B;
				}
			}
			else
			{
				if (!(text == "gototarget"))
				{
					goto IL_52B;
				}
				goto IL_2B3;
			}
		}
		else if (num != 3909890315U)
		{
			if (num != 3926667934U)
			{
				goto IL_52B;
			}
			if (!(text == "o"))
			{
				goto IL_52B;
			}
			goto IL_3C9;
		}
		else
		{
			if (!(text == "l"))
			{
				goto IL_52B;
			}
			goto IL_2E7;
		}
		IL_2DA:
		this.lookAtPos = primaryPlayer.GetPosition();
		return;
		IL_3C9:
		if (this.targetPos.sqrMagnitude == 0f && _params.Count < 6)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("orbit target is not set");
			return;
		}
		Vector3 orbitPos = this.targetPos;
		float duration = 5f;
		if (_params.Count >= 2)
		{
			duration = this.FloatParse(_params[1]);
		}
		int loopCount = 0;
		if (_params.Count >= 3)
		{
			loopCount = this.IntParse(_params[2]);
		}
		if (_params.Count >= 4)
		{
			orbitPos.x = this.FloatParse(_params[3]);
		}
		if (_params.Count >= 5)
		{
			orbitPos.y = this.FloatParse(_params[4]);
		}
		if (_params.Count >= 6)
		{
			orbitPos.z = this.FloatParse(_params[5]);
		}
		EntityPlayerLocal.AutoMove autoMove2 = primaryPlayer.EnableAutoMove(true);
		autoMove2.SetLookAt(this.lookAtPos);
		autoMove2.StartOrbit(duration, loopCount, orbitPos, false, false, null);
		return;
		IL_2B3:
		primaryPlayer.SetPosition(this.targetPos, true);
		return;
		IL_2E7:
		if (this.targetPos.sqrMagnitude == 0f && _params.Count < 6)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("line target is not set");
			return;
		}
		Vector3 endPos = this.targetPos;
		float duration2 = 5f;
		if (_params.Count >= 2)
		{
			duration2 = this.FloatParse(_params[1]);
		}
		int loopCount2 = 0;
		if (_params.Count >= 3)
		{
			loopCount2 = this.IntParse(_params[2]);
		}
		if (_params.Count >= 4)
		{
			endPos.x = this.FloatParse(_params[3]);
		}
		if (_params.Count >= 5)
		{
			endPos.y = this.FloatParse(_params[4]);
		}
		if (_params.Count >= 6)
		{
			endPos.z = this.FloatParse(_params[5]);
		}
		EntityPlayerLocal.AutoMove autoMove3 = primaryPlayer.EnableAutoMove(true);
		autoMove3.SetLookAt(this.lookAtPos);
		autoMove3.StartLine(duration2, loopCount2, endPos, null);
		return;
		IL_52B:
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("unknown command " + _params[0]);
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x00062C54 File Offset: 0x00060E54
	public int IntParse(string s)
	{
		int result;
		int.TryParse(s, out result);
		return result;
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x00062C6C File Offset: 0x00060E6C
	public float FloatParse(string s)
	{
		float result;
		float.TryParse(s, out result);
		return result;
	}

	// Token: 0x04000C8D RID: 3213
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 targetPos;

	// Token: 0x04000C8E RID: 3214
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 lookAtPos;
}
