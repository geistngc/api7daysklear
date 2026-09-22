using System;
using System.IO;

// Token: 0x02000C54 RID: 3156
public class AuthoredText
{
	// Token: 0x170009C3 RID: 2499
	// (get) Token: 0x06006013 RID: 24595 RVA: 0x00261573 File Offset: 0x0025F773
	// (set) Token: 0x06006014 RID: 24596 RVA: 0x0026157B File Offset: 0x0025F77B
	public string Text { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170009C4 RID: 2500
	// (get) Token: 0x06006015 RID: 24597 RVA: 0x00261584 File Offset: 0x0025F784
	// (set) Token: 0x06006016 RID: 24598 RVA: 0x0026158C File Offset: 0x0025F78C
	public PlatformUserIdentifierAbs Author { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06006017 RID: 24599 RVA: 0x00261595 File Offset: 0x0025F795
	public AuthoredText()
	{
		this.Update(string.Empty, null);
	}

	// Token: 0x06006018 RID: 24600 RVA: 0x002615A9 File Offset: 0x0025F7A9
	public AuthoredText(string _text, PlatformUserIdentifierAbs _author)
	{
		this.Update(_text, _author);
	}

	// Token: 0x06006019 RID: 24601 RVA: 0x002615B9 File Offset: 0x0025F7B9
	public void Update(string _text, PlatformUserIdentifierAbs _author)
	{
		this.Author = _author;
		this.Text = _text;
	}

	// Token: 0x0600601A RID: 24602 RVA: 0x002615CC File Offset: 0x0025F7CC
	public static AuthoredText FromStream(BinaryReader _reader)
	{
		if (_reader == null)
		{
			return null;
		}
		if (!_reader.ReadBoolean())
		{
			return null;
		}
		string text = _reader.ReadString();
		PlatformUserIdentifierAbs author = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
		AuthoredText authoredText = new AuthoredText();
		authoredText.Update(text, author);
		return authoredText;
	}

	// Token: 0x0600601B RID: 24603 RVA: 0x00261605 File Offset: 0x0025F805
	public static void ToStream(AuthoredText _instance, BinaryWriter _writer)
	{
		if (_writer == null)
		{
			return;
		}
		if (_instance == null)
		{
			_writer.Write(0);
			return;
		}
		_writer.Write(1);
		_writer.Write(_instance.Text);
		_instance.Author.ToStream(_writer, false);
	}

	// Token: 0x0600601C RID: 24604 RVA: 0x00261636 File Offset: 0x0025F836
	public AuthoredText Clone()
	{
		return new AuthoredText(this.Text, this.Author);
	}
}
