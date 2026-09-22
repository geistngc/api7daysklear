using System;

namespace PrefabVolumes
{
	// Token: 0x020018C0 RID: 6336
	public abstract class PrefabVolumeAbs<T> : PrefabVolumeAbs where T : PrefabVolumeAbs<T>, new()
	{
		// Token: 0x0600C388 RID: 50056 RVA: 0x004866A5 File Offset: 0x004848A5
		public override PrefabVolumeAbs Clone()
		{
			return this.CloneGeneric();
		}

		// Token: 0x0600C389 RID: 50057 RVA: 0x004866B4 File Offset: 0x004848B4
		public virtual T CloneGeneric()
		{
			T t = Activator.CreateInstance<T>();
			this.CopyValues(t, false);
			return t;
		}

		// Token: 0x0600C38A RID: 50058 RVA: 0x004866D8 File Offset: 0x004848D8
		public override void CopyValues(PrefabVolumeAbs _target, bool _nonBasicOnly = false)
		{
			T t = _target as T;
			if (t == null)
			{
				throw new ArgumentException("Can not copy values to volume of type " + _target.VolumeType.ToStringCached<PrefabVolumeAbs.EVolumeType>() + " from volume of type " + this.VolumeType.ToStringCached<PrefabVolumeAbs.EVolumeType>(), "_target");
			}
			this.CopyValues(t, _nonBasicOnly);
		}

		// Token: 0x0600C38B RID: 50059 RVA: 0x00486736 File Offset: 0x00484936
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void CopyValues(PrefabVolumeAbs<T> _target, bool _nonBasicOnly = false)
		{
			_target.used = base.Used;
			_target.startPos = this.startPos;
			_target.size = this.size;
		}

		// Token: 0x0600C38C RID: 50060 RVA: 0x0048675C File Offset: 0x0048495C
		[PublicizedFrom(EAccessModifier.Protected)]
		public PrefabVolumeAbs()
		{
		}
	}
}
