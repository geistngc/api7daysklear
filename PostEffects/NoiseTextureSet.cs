using System;
using UnityEngine;

namespace PostEffects
{
	// Token: 0x02001D67 RID: 7527
	public sealed class NoiseTextureSet : ScriptableObject
	{
		// Token: 0x0600DE89 RID: 56969 RVA: 0x004FDE56 File Offset: 0x004FC056
		public Texture2D GetTexture()
		{
			return this.GetTexture(Time.frameCount);
		}

		// Token: 0x0600DE8A RID: 56970 RVA: 0x004FDE63 File Offset: 0x004FC063
		public Texture2D GetTexture(int frameCount)
		{
			return this._textures[frameCount % this._textures.Length];
		}

		// Token: 0x0400A931 RID: 43313
		[SerializeField]
		[PublicizedFrom(EAccessModifier.Private)]
		public Texture2D[] _textures;
	}
}
