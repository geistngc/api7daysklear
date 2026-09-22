using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.LiveData;

namespace Webserver.WebAPI.APIs.WorldState
{
	// Token: 0x02001ADB RID: 6875
	[Preserve]
	[PublicizedFrom(EAccessModifier.Internal)]
	public class Hostile : AbsRestApi
	{
		// Token: 0x0600CEFB RID: 52987 RVA: 0x004B81D8 File Offset: 0x004B63D8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteBeginArray();
			List<EntityEnemy> obj = this.entities;
			lock (obj)
			{
				Hostiles.Instance.Get(this.entities);
				for (int i = 0; i < this.entities.Count; i++)
				{
					if (i > 0)
					{
						jsonWriter.WriteValueSeparator();
					}
					EntityAlive entityAlive = this.entities[i];
					Vector3i position = new Vector3i(entityAlive.GetPosition());
					jsonWriter.WriteRaw(Hostile.jsonKeyId);
					jsonWriter.WriteInt32(entityAlive.entityId);
					jsonWriter.WriteRaw(Hostile.jsonKeyName);
					jsonWriter.WriteString((!string.IsNullOrEmpty(entityAlive.EntityName)) ? entityAlive.EntityName : string.Format("enemy class #{0}", entityAlive.entityClass));
					jsonWriter.WriteRaw(Hostile.jsonKeyPosition);
					JsonCommons.WriteVector3I(ref jsonWriter, position);
					jsonWriter.WriteEndObject();
				}
			}
			jsonWriter.WriteEndArray();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CEFC RID: 52988 RVA: 0x004B8300 File Offset: 0x004B6500
		public Hostile() : base(null)
		{
		}

		// Token: 0x04009D4A RID: 40266
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<EntityEnemy> entities = new List<EntityEnemy>();

		// Token: 0x04009D4B RID: 40267
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyId = JsonWriter.GetEncodedPropertyNameWithBeginObject("id");

		// Token: 0x04009D4C RID: 40268
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyName = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name");

		// Token: 0x04009D4D RID: 40269
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPosition = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("position");
	}
}
