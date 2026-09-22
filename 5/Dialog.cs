using System;
using System.Collections.Generic;

// Token: 0x020002E9 RID: 745
public class Dialog
{
	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001566 RID: 5478 RVA: 0x00080A67 File Offset: 0x0007EC67
	// (set) Token: 0x06001567 RID: 5479 RVA: 0x00080A83 File Offset: 0x0007EC83
	public DialogStatement CurrentStatement
	{
		get
		{
			if (this.ChildDialog != null)
			{
				return this.ChildDialog.CurrentStatement;
			}
			return this.currentStatement;
		}
		set
		{
			if (this.ChildDialog != null)
			{
				this.ChildDialog.CurrentStatement = value;
				return;
			}
			this.currentStatement = value;
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x00080AA4 File Offset: 0x0007ECA4
	public Dialog(string newID)
	{
		this.ID = newID;
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x00080B18 File Offset: 0x0007ED18
	[PublicizedFrom(EAccessModifier.Internal)]
	public DialogStatement GetStatement(string currentStatementID)
	{
		if (this.ChildDialog != null)
		{
			return this.ChildDialog.GetStatement(currentStatementID);
		}
		if (currentStatementID == "")
		{
			currentStatementID = this.StartStatementID;
		}
		for (int i = 0; i < this.Statements.Count; i++)
		{
			if (this.Statements[i].ID == currentStatementID)
			{
				return this.Statements[i];
			}
		}
		return null;
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00080B8C File Offset: 0x0007ED8C
	[PublicizedFrom(EAccessModifier.Internal)]
	public DialogResponse GetResponse(string currentResponseID)
	{
		if (this.ChildDialog != null)
		{
			return this.ChildDialog.GetResponse(currentResponseID);
		}
		for (int i = 0; i < this.Responses.Count; i++)
		{
			if (this.Responses[i].ID == currentResponseID)
			{
				return this.Responses[i];
			}
		}
		return null;
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x00080BEC File Offset: 0x0007EDEC
	[PublicizedFrom(EAccessModifier.Internal)]
	public DialogStatement GetFirstStatment(EntityPlayer player)
	{
		string startStatementID = this.StartStatementID;
		for (int i = 0; i < this.Phases.Count; i++)
		{
			bool flag = true;
			for (int j = 0; j < this.Phases[i].RequirementList.Count; j++)
			{
				if (!this.Phases[i].RequirementList[j].CheckRequirement(player, this.CurrentOwner))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				startStatementID = this.Phases[i].StartStatementID;
				break;
			}
		}
		for (int k = 0; k < this.Statements.Count; k++)
		{
			if (this.Statements[k].ID == startStatementID)
			{
				return this.Statements[k];
			}
		}
		return null;
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x00080CBC File Offset: 0x0007EEBC
	public void RestartDialog(EntityPlayer player)
	{
		this.CurrentStatement = this.GetFirstStatment(player);
		this.ChildDialog = null;
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00080CD4 File Offset: 0x0007EED4
	public void SelectResponse(DialogResponse response, EntityPlayer player)
	{
		if (this.ChildDialog != null)
		{
			this.ChildDialog.SelectResponse(response, player);
			return;
		}
		if (response.Actions.Count > 0)
		{
			for (int i = 0; i < response.Actions.Count; i++)
			{
				response.Actions[i].PerformAction(player);
			}
		}
		if (response is DialogResponseQuest)
		{
			DialogResponseQuest dialogResponseQuest = response as DialogResponseQuest;
			QuestClass questClass = dialogResponseQuest.Quest.QuestClass;
			this.CurrentStatement = new DialogStatement("");
			this.CurrentStatement.NextStatementID = dialogResponseQuest.NextStatementID;
			this.CurrentStatement.Text = dialogResponseQuest.Quest.GetParsedText(questClass.StatementText);
			return;
		}
		this.CurrentStatement = this.GetStatement(response.NextStatementID);
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x00080D98 File Offset: 0x0007EF98
	public static void Cleanup()
	{
		Dialog.DialogList.Clear();
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x00080DA4 File Offset: 0x0007EFA4
	public static void ReloadDialogs()
	{
		Dialog.Cleanup();
		WorldStaticData.Reset("dialogs");
	}

	// Token: 0x04000E5E RID: 3678
	public static Dictionary<string, Dialog> DialogList = new Dictionary<string, Dialog>();

	// Token: 0x04000E5F RID: 3679
	public string ID = "";

	// Token: 0x04000E60 RID: 3680
	public string StartStatementID = "";

	// Token: 0x04000E61 RID: 3681
	public string StartResponseID = "";

	// Token: 0x04000E62 RID: 3682
	public List<DialogPhase> Phases = new List<DialogPhase>();

	// Token: 0x04000E63 RID: 3683
	public List<DialogStatement> Statements = new List<DialogStatement>();

	// Token: 0x04000E64 RID: 3684
	public List<DialogResponse> Responses = new List<DialogResponse>();

	// Token: 0x04000E65 RID: 3685
	public EntityNPC CurrentOwner;

	// Token: 0x04000E66 RID: 3686
	public Dialog ChildDialog;

	// Token: 0x04000E67 RID: 3687
	public List<QuestEntry> QuestEntryList = new List<QuestEntry>();

	// Token: 0x04000E68 RID: 3688
	[PublicizedFrom(EAccessModifier.Private)]
	public DialogStatement currentStatement;

	// Token: 0x04000E69 RID: 3689
	public string currentReturnStatement = "";
}
