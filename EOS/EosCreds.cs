using System;

namespace Platform.EOS
{
	// Token: 0x02001CEF RID: 7407
	public class EosCreds
	{
		// Token: 0x0600DBCD RID: 56269 RVA: 0x004EBE7E File Offset: 0x004EA07E
		[PublicizedFrom(EAccessModifier.Private)]
		public EosCreds(string _productId, string _sandboxId, string _deploymentId, string _clientId, string _clientSecret, bool _serverMode)
		{
			this.ProductId = _productId;
			this.SandboxId = _sandboxId;
			this.DeploymentId = _deploymentId;
			this.ClientId = _clientId;
			this.ClientSecret = _clientSecret;
			this.ServerMode = _serverMode;
		}

		// Token: 0x0400A65F RID: 42591
		public const string StorageEncKey = "0000000000000000000000000000000000000000000000000000000000000000";

		// Token: 0x0400A660 RID: 42592
		[PublicizedFrom(EAccessModifier.Private)]
		public const string productId = "85fffb61212b491999cd7fc03eb09bf6";

		// Token: 0x0400A661 RID: 42593
		[PublicizedFrom(EAccessModifier.Private)]
		public const string sandboxId = "8a44365d5ccb43328b4df2f8ca199e43";

		// Token: 0x0400A662 RID: 42594
		[PublicizedFrom(EAccessModifier.Private)]
		public const string deploymentId = "c9ccbd00333f4dd6995beb7c75000942";

		// Token: 0x0400A663 RID: 42595
		[PublicizedFrom(EAccessModifier.Private)]
		public const string deploymentId_Old = "30b9e9e5f58b4f4e82930b3bef76d9e1";

		// Token: 0x0400A664 RID: 42596
		public static readonly EosCreds ClientCredentials = new EosCreds("85fffb61212b491999cd7fc03eb09bf6", "8a44365d5ccb43328b4df2f8ca199e43", "c9ccbd00333f4dd6995beb7c75000942", "xyza7891WBnGQuvNMiNyg6SYeYOhbA2F", "aopC/pp4xFK643dkeOOktsSiFV1IC5qQiLfJ8EJjPrw", false);

		// Token: 0x0400A665 RID: 42597
		public static readonly EosCreds ServerCredentials = new EosCreds("85fffb61212b491999cd7fc03eb09bf6", "8a44365d5ccb43328b4df2f8ca199e43", "c9ccbd00333f4dd6995beb7c75000942", "xyza7891nSjSAzYxhnVGWL1xKR4jAL7I", "fkCG6lR19l6KCfXFxxF1dppvCbA76qZT9IO+4eqX5QU", true);

		// Token: 0x0400A666 RID: 42598
		public static readonly EosCreds ServerDeviceIdCredentials = new EosCreds("85fffb61212b491999cd7fc03eb09bf6", "8a44365d5ccb43328b4df2f8ca199e43", "c9ccbd00333f4dd6995beb7c75000942", "xyza7891UbuRjRa7tG4QlJvZC6l62dwE", "Ah8Ul94L+ybtvVhqJAKPMOKKfuLu+8bPlmoZaPLFbkc", false);

		// Token: 0x0400A667 RID: 42599
		public readonly string ProductId;

		// Token: 0x0400A668 RID: 42600
		public readonly string SandboxId;

		// Token: 0x0400A669 RID: 42601
		public readonly string DeploymentId;

		// Token: 0x0400A66A RID: 42602
		public readonly string ClientId;

		// Token: 0x0400A66B RID: 42603
		public readonly string ClientSecret;

		// Token: 0x0400A66C RID: 42604
		public readonly bool ServerMode;
	}
}
