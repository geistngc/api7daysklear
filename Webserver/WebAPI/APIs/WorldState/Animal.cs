using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.LiveData;

namespace Webserver.WebAPI.APIs.WorldState
{
	// Token: 0x02001AD9 RID: 6873
	[Preserve]
	[PublicizedFrom(EAccessModifier.Internal)]
	public class Animal : AbsRestApi
	{
		// Token: 0x0600CEF5 RID: 52981 RVA: 0x004B7F40 File Offset: 0x004B6140
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteBeginArray();
			List<EntityAnimal> obj = this.entities;
			lock (obj)
			{
				Animals.Instance.Get(this.entities);
				for (int i = 0; i < this.entities.Count; i++)
				{
					if (i > 0)
					{
						jsonWriter.WriteValueSeparator();
					}
					EntityAlive entityAlive = this.entities[i];
					Vector3i position = new Vector3i(entityAlive.GetPosition());
					jsonWriter.WriteRaw(Animal.jsonKeyId);
					jsonWriter.WriteInt32(entityAlive.entityId);
					jsonWriter.WriteRaw(Animal.jsonKeyName);
					jsonWriter.WriteString((!string.IsNullOrEmpty(entityAlive.EntityName)) ? entityAlive.EntityName : string.Format("animal class #{0}", entityAlive.entityClass));
					jsonWriter.WriteRaw(Animal.jsonKeyPosition);
					JsonCommons.WriteVector3I(ref jsonWriter, position);
					jsonWriter.WriteEndObject();
				}
			}
			jsonWriter.WriteEndArray();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CEF6 RID: 52982 RVA: 0x004B8068 File Offset: 0x004B6268
		public Animal() : base(null)
		{
		}

		// Token: 0x04009D42 RID: 40258
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<EntityAnimal> entities = new List<EntityAnimal>();

		// Token: 0x04009D43 RID: 40259
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyId = JsonWriter.GetEncodedPropertyNameWithBeginObject("id");

		// Token: 0x04009D44 RID: 40260
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyName = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name");

		// Token: 0x04009D45 RID: 40261
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPosition = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("position");
	}
}
