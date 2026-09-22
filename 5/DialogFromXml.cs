using System;
using System.Collections;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x02000D02 RID: 3330
public class DialogFromXml
{
	// Token: 0x060065C3 RID: 26051 RVA: 0x0027D7C8 File Offset: 0x0027B9C8
	public static IEnumerator Load(XmlFile _xmlFile)
	{
		Dialog.DialogList.Clear();
		DialogFromXml.CreateDialogs(_xmlFile);
		yield break;
	}

	// Token: 0x060065C4 RID: 26052 RVA: 0x000027FC File Offset: 0x000009FC
	public static void Reload()
	{
	}

	// Token: 0x060065C5 RID: 26053 RVA: 0x0027D7D7 File Offset: 0x0027B9D7
	public static bool CreateDialogs(XmlFile xmlFile)
	{
		Dialog.DialogList.Clear();
		XElement root = xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <dialogs> found!");
		}
		DialogFromXml.ParseNode(root);
		return true;
	}

	// Token: 0x060065C6 RID: 26054 RVA: 0x0027D808 File Offset: 0x0027BA08
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseNode(XElement root)
	{
		foreach (XElement e in root.Elements("dialog"))
		{
			Dialog dialog = DialogFromXml.ParseDialog(e);
			Dialog.DialogList.Add(dialog.ID, dialog);
		}
	}

	// Token: 0x060065C7 RID: 26055 RVA: 0x0027D870 File Offset: 0x0027BA70
	public static Dialog ParseDialog(XElement e)
	{
		if (!e.HasAttribute("id"))
		{
			throw new Exception("quest must have an id attribute");
		}
		string attribute = e.GetAttribute("id");
		if (Dialog.DialogList.ContainsKey(attribute))
		{
			throw new Exception("Duplicate dialog entry with id " + attribute);
		}
		Dialog dialog = new Dialog(attribute);
		if (e.HasAttribute("startstatementid"))
		{
			dialog.StartStatementID = e.GetAttribute("startstatementid");
		}
		if (e.HasAttribute("startresponseid"))
		{
			dialog.StartResponseID = e.GetAttribute("startresponseid");
		}
		foreach (XElement xelement in e.Elements())
		{
			if (xelement.Name == "phase")
			{
				DialogFromXml.ParsePhase(dialog, xelement);
			}
			else if (xelement.Name == "statement")
			{
				DialogFromXml.ParseStatement(dialog, xelement);
			}
			else
			{
				if (!(xelement.Name == "response"))
				{
					string str = "Unrecognized xml element ";
					XName name = xelement.Name;
					throw new Exception(str + ((name != null) ? name.ToString() : null));
				}
				DialogFromXml.ParseResponse(dialog, xelement);
			}
		}
		return dialog;
	}

	// Token: 0x060065C8 RID: 26056 RVA: 0x0027D9E4 File Offset: 0x0027BBE4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParsePhase(Dialog dialog, XElement e)
	{
		DialogPhase dialogPhase = new DialogPhase(e.GetAttribute("id"));
		dialog.Phases.Add(dialogPhase);
		dialogPhase.OwnerDialog = dialog;
		if (e.HasAttribute("startstatementid"))
		{
			dialogPhase.StartStatementID = e.GetAttribute("startstatementid");
		}
		if (e.HasAttribute("startresponseid"))
		{
			dialogPhase.StartResponseID = e.GetAttribute("startresponseid");
		}
		foreach (XElement e2 in e.Elements("requirement"))
		{
			BaseDialogRequirement baseDialogRequirement = DialogFromXml.ParseRequirement(e2);
			if (baseDialogRequirement != null)
			{
				dialogPhase.AddRequirement(baseDialogRequirement);
			}
		}
	}

	// Token: 0x060065C9 RID: 26057 RVA: 0x0027DAC0 File Offset: 0x0027BCC0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseStatement(Dialog dialog, XElement e)
	{
		DialogStatement dialogStatement = null;
		if (e.HasAttribute("id"))
		{
			dialogStatement = new DialogStatement(e.GetAttribute("id"));
			dialog.Statements.Add(dialogStatement);
			dialogStatement.OwnerDialog = dialog;
		}
		if (e.HasAttribute("nextstatementid"))
		{
			dialogStatement.NextStatementID = e.GetAttribute("nextstatementid");
		}
		if (e.HasAttribute("text"))
		{
			dialogStatement.Text = Localization.Get(e.GetAttribute("text"), false, null);
		}
		foreach (XElement xelement in e.Elements())
		{
			if (xelement.Name == "response_entry")
			{
				DialogResponseEntry dialogResponseEntry = null;
				if (xelement.HasAttribute("id"))
				{
					dialogResponseEntry = new DialogResponseEntry(xelement.GetAttribute("id"));
					dialogStatement.ResponseEntries.Add(dialogResponseEntry);
				}
				if (dialogResponseEntry != null && xelement.HasAttribute("uniqueid"))
				{
					dialogResponseEntry.UniqueID = xelement.GetAttribute("uniqueid");
				}
			}
			else if (xelement.Name == "quest_entry")
			{
				string questID = "";
				string returnStatementID = "";
				string type = "";
				int listIndex = -1;
				int tier = -1;
				if (xelement.HasAttribute("id"))
				{
					questID = xelement.GetAttribute("id");
				}
				if (xelement.HasAttribute("type"))
				{
					type = xelement.GetAttribute("type");
				}
				if (xelement.HasAttribute("nextstatementid"))
				{
					returnStatementID = xelement.GetAttribute("nextstatementid");
				}
				if (xelement.HasAttribute("returnstatementid"))
				{
					returnStatementID = xelement.GetAttribute("returnstatementid");
				}
				if (xelement.HasAttribute("tier"))
				{
					tier = StringParsers.ParseSInt32(xelement.GetAttribute("tier"), 0, -1, NumberStyles.Integer);
				}
				if (xelement.HasAttribute("listindex"))
				{
					listIndex = Convert.ToInt32(xelement.GetAttribute("listindex"));
				}
				dialogStatement.ResponseEntries.Add(new DialogQuestResponseEntry(questID, type, returnStatementID, listIndex, tier));
			}
			else if (xelement.Name == "action")
			{
				BaseDialogAction baseDialogAction = DialogFromXml.ParseAction(xelement);
				if (baseDialogAction != null)
				{
					dialogStatement.AddAction(baseDialogAction);
				}
			}
		}
	}

	// Token: 0x060065CA RID: 26058 RVA: 0x0027DD90 File Offset: 0x0027BF90
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseResponse(Dialog dialog, XElement e)
	{
		DialogResponse dialogResponse = null;
		if (e.HasAttribute("id"))
		{
			dialogResponse = new DialogResponse(e.GetAttribute("id"));
			dialog.Responses.Add(dialogResponse);
			dialogResponse.OwnerDialog = dialog;
		}
		if (e.HasAttribute("nextstatementid"))
		{
			dialogResponse.NextStatementID = e.GetAttribute("nextstatementid");
		}
		if (e.HasAttribute("text"))
		{
			dialogResponse.Text = Localization.Get(e.GetAttribute("text"), false, null);
		}
		if (e.HasAttribute("returnstatementid"))
		{
			dialogResponse.ReturnStatementID = e.GetAttribute("returnstatementid");
		}
		foreach (XElement xelement in e.Elements())
		{
			if (xelement.Name == "requirement")
			{
				BaseDialogRequirement baseDialogRequirement = DialogFromXml.ParseRequirement(xelement);
				if (baseDialogRequirement != null)
				{
					dialogResponse.AddRequirement(baseDialogRequirement);
				}
			}
			if (xelement.Name == "action")
			{
				BaseDialogAction baseDialogAction = DialogFromXml.ParseAction(xelement);
				if (baseDialogAction != null)
				{
					dialogResponse.AddAction(baseDialogAction);
				}
			}
		}
	}

	// Token: 0x060065CB RID: 26059 RVA: 0x0027DEE8 File Offset: 0x0027C0E8
	[PublicizedFrom(EAccessModifier.Private)]
	public static BaseDialogAction ParseAction(XElement e)
	{
		if (!e.HasAttribute("type"))
		{
			throw new Exception("Dialog Action must have a type!");
		}
		BaseDialogAction baseDialogAction = null;
		string attribute = e.GetAttribute("type");
		try
		{
			baseDialogAction = (BaseDialogAction)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("DialogAction", attribute));
		}
		catch (Exception)
		{
			throw new Exception("No action class '" + attribute + " found!");
		}
		if (e.HasAttribute("id"))
		{
			baseDialogAction.ID = e.GetAttribute("id");
		}
		if (e.HasAttribute("value"))
		{
			baseDialogAction.Value = e.GetAttribute("value");
		}
		return baseDialogAction;
	}

	// Token: 0x060065CC RID: 26060 RVA: 0x0027DFB8 File Offset: 0x0027C1B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static BaseDialogRequirement ParseRequirement(XElement e)
	{
		if (!e.HasAttribute("type"))
		{
			throw new Exception("Dialog Action must have a type!");
		}
		BaseDialogRequirement baseDialogRequirement = null;
		string attribute = e.GetAttribute("type");
		try
		{
			baseDialogRequirement = (BaseDialogRequirement)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("DialogRequirement", attribute));
		}
		catch (Exception)
		{
			throw new Exception("No action class '" + attribute + " found!");
		}
		if (e.HasAttribute("id"))
		{
			baseDialogRequirement.ID = e.GetAttribute("id");
		}
		if (e.HasAttribute("value"))
		{
			baseDialogRequirement.Value = e.GetAttribute("value");
		}
		if (e.HasAttribute("tag"))
		{
			baseDialogRequirement.Tag = e.GetAttribute("tag");
		}
		else
		{
			baseDialogRequirement.Tag = "";
		}
		if (e.HasAttribute("requirementtype"))
		{
			baseDialogRequirement.RequirementVisibilityType = EnumUtils.Parse<BaseDialogRequirement.RequirementVisibilityTypes>(e.GetAttribute("requirementtype"), false);
		}
		return baseDialogRequirement;
	}
}
