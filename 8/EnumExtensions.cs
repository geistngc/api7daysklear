using System;
using System.Reflection;
using UniLinq;

// Token: 0x020013FC RID: 5116
public static class EnumExtensions
{
	// Token: 0x0600A091 RID: 41105 RVA: 0x003C7BA0 File Offset: 0x003C5DA0
	public static string GetDocumentation(this Enum enumValue)
	{
		MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault<MemberInfo>();
		string text;
		if (memberInfo == null)
		{
			text = null;
		}
		else
		{
			Documentation customAttribute = memberInfo.GetCustomAttribute<Documentation>();
			text = ((customAttribute != null) ? customAttribute.Text : null);
		}
		return text ?? enumValue.ToString();
	}
}
