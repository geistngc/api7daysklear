using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200171E RID: 5918
	public class PrefabManagerData
	{
		// Token: 0x0600B82E RID: 47150 RVA: 0x00448158 File Offset: 0x00446358
		public void LoadPrefabs()
		{
			if (this.AllPrefabDatas.Count != 0)
			{
				return;
			}
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			List<PathAbstractions.AbstractedLocation> availablePathsList = PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList(null, true, null);
			FastTags<TagGroup.Poi> other = FastTags<TagGroup.Poi>.Parse("navonly,devonly,testonly,biomeonly");
			for (int i = 0; i < availablePathsList.Count; i++)
			{
				PathAbstractions.AbstractedLocation abstractedLocation = availablePathsList[i];
				int num = abstractedLocation.Folder.LastIndexOf("/Prefabs/");
				if (num < 0 || !abstractedLocation.Folder.Substring(num + 8, 5).EqualsCaseInsensitive("/test"))
				{
					PrefabData prefabData = PrefabData.LoadPrefabData(abstractedLocation);
					try
					{
						if (prefabData != null && !prefabData.Tags.Test_AnySet(other) && !prefabData.Tags.IsEmpty)
						{
							this.AllPrefabDatas[abstractedLocation.Name.ToLower()] = prefabData;
						}
					}
					catch (Exception)
					{
						Log.Error("Could not load prefab data for " + abstractedLocation.Name);
					}
				}
			}
			Log.Out("LoadPrefabs {0} of {1} in {2} s", new object[]
			{
				this.AllPrefabDatas.Count,
				availablePathsList.Count,
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
		}

		// Token: 0x0600B82F RID: 47151 RVA: 0x004482AC File Offset: 0x004464AC
		public void ShufflePrefabData(int _seed)
		{
			this.prefabDataList.Clear();
			this.AllPrefabDatas.CopyValuesTo(this.prefabDataList);
			this.prefabDataList.Shuffle(_seed);
		}

		// Token: 0x0600B830 RID: 47152 RVA: 0x004482D6 File Offset: 0x004464D6
		public void Cleanup()
		{
			this.AllPrefabDatas.Clear();
			this.prefabDataList.Clear();
		}

		// Token: 0x0600B831 RID: 47153 RVA: 0x004482F0 File Offset: 0x004464F0
		public Prefab GetPreviewPrefabWithAnyTags(FastTags<TagGroup.Poi> _tags, int _townshipId, Vector2i size = default(Vector2i), bool useAnySizeSmaller = false)
		{
			Vector2i minSize = useAnySizeSmaller ? Vector2i.zero : size;
			List<PrefabData> list = this.prefabDataList.FindAll((PrefabData _pd) => !_pd.Tags.Test_AnySet(this.PartsAndTilesTags) && PrefabManager.isSizeValid(_pd, minSize, size) && _pd.Tags.Test_AnySet(_tags));
			if (list.Count == 0)
			{
				return null;
			}
			list.Shuffle(this.previewSeed);
			Prefab prefab = new Prefab();
			prefab.Load(list[0].location, true, true, false, false);
			this.previewSeed++;
			return prefab;
		}

		// Token: 0x040089E8 RID: 35304
		public readonly Dictionary<string, PrefabData> AllPrefabDatas = new Dictionary<string, PrefabData>();

		// Token: 0x040089E9 RID: 35305
		public readonly List<PrefabData> prefabDataList = new List<PrefabData>();

		// Token: 0x040089EA RID: 35306
		public readonly FastTags<TagGroup.Poi> PartsAndTilesTags = FastTags<TagGroup.Poi>.Parse("streettile,part");

		// Token: 0x040089EB RID: 35307
		public readonly FastTags<TagGroup.Poi> WildernessTags = FastTags<TagGroup.Poi>.Parse("wilderness");

		// Token: 0x040089EC RID: 35308
		public readonly FastTags<TagGroup.Poi> TraderTags = FastTags<TagGroup.Poi>.Parse("trader");

		// Token: 0x040089ED RID: 35309
		public readonly FastTags<TagGroup.Poi> HasTags = FastTags<TagGroup.Poi>.Parse("has");

		// Token: 0x040089EE RID: 35310
		[PublicizedFrom(EAccessModifier.Private)]
		public int previewSeed;
	}
}
