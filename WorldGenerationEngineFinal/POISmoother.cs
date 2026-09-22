using System;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200171C RID: 5916
	public class POISmoother
	{
		// Token: 0x0600B82B RID: 47147 RVA: 0x0044805D File Offset: 0x0044625D
		public POISmoother(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600B82C RID: 47148 RVA: 0x0044806C File Offset: 0x0044626C
		public void SmoothStreetTiles()
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			int streetTileMapWidth = this.worldBuilder.StreetTileMapWidth;
			int streetTileMapWidth2 = this.worldBuilder.StreetTileMapWidth;
			int num = 0;
			int num2 = streetTileMapWidth * streetTileMapWidth2;
			for (int i = 0; i < streetTileMapWidth2; i++)
			{
				for (int j = 0; j < streetTileMapWidth; j++)
				{
					num++;
					StreetTile streetTile = this.worldBuilder.StreetTileMap[j + i * streetTileMapWidth];
					streetTile.SmoothTownshipTerrain();
					streetTile.UpdateValidity();
				}
				this.worldBuilder.SetTaskMessage(string.Format(this.worldBuilder.messageSmoothingStreetTiles, Mathf.RoundToInt((float)num / (float)num2 * 100f)));
			}
			Log.Out("POISmoother SmoothStreetTiles in {0}", new object[]
			{
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
		}

		// Token: 0x040089E4 RID: 35300
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;
	}
}
