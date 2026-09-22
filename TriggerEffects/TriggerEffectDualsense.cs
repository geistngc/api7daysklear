using System;

namespace TriggerEffects
{
	// Token: 0x02001692 RID: 5778
	[PublicizedFrom(EAccessModifier.Internal)]
	public static class TriggerEffectDualsense
	{
		// Token: 0x0600B549 RID: 46409 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void InitTriggerEffectManager()
		{
		}

		// Token: 0x0600B54A RID: 46410 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void ConnectedUpdate(TriggerEffectManager.TriggerEffectDS currentEffectLeft, TriggerEffectManager.TriggerEffectDS currentEffectRight)
		{
		}

		// Token: 0x0600B54B RID: 46411 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void Update(TriggerEffectManager.TriggerEffectDS left, TriggerEffectManager.TriggerEffectDS right)
		{
		}

		// Token: 0x0600B54C RID: 46412 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void SetEffectToOff()
		{
		}

		// Token: 0x0600B54D RID: 46413 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void ResetControllerIdentification()
		{
		}

		// Token: 0x0600B54E RID: 46414 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void SetControllerIdentification()
		{
		}

		// Token: 0x0600B54F RID: 46415 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void ApplyImmediate(TriggerEffectManager.GamepadTrigger trigger, TriggerEffectManager.ControllerTriggerEffect currentEffect)
		{
		}

		// Token: 0x0600B550 RID: 46416 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void ApplyEffectPS5Input(int slot, TriggerEffectManager.GamepadTrigger triggerGeneric, TriggerEffectManager.TriggerEffectDS effect)
		{
		}

		// Token: 0x0600B551 RID: 46417 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void SetTriggerEffectVibration(int userID, TriggerEffectManager.GamepadTrigger trigger, byte position, byte amplitude, byte frequency)
		{
		}

		// Token: 0x0600B552 RID: 46418 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void SetTriggerEffectVibrationMultiplePosition(int userID, TriggerEffectManager.GamepadTrigger trigger, byte[] amplitudes, byte frequency)
		{
		}

		// Token: 0x0600B553 RID: 46419 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void SetWeaponEffect(int userID, TriggerEffectManager.GamepadTrigger trigger, byte startPosition, byte endPosition, byte strength)
		{
		}

		// Token: 0x0600B554 RID: 46420 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void EnableVibration()
		{
		}

		// Token: 0x0600B555 RID: 46421 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void SetDualSenseVibration(byte _smallMotor, byte _largeMotor)
		{
		}

		// Token: 0x0600B556 RID: 46422 RVA: 0x000027FC File Offset: 0x000009FC
		public static void SetLightbar(int userId, byte colorR, byte colorG, byte colorB)
		{
		}
	}
}
