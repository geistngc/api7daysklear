using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001195 RID: 4501
public class DebugLines : MonoBehaviour
{
	// Token: 0x06008FF0 RID: 36848 RVA: 0x003620FA File Offset: 0x003602FA
	public static DebugLines Create(string _name, Transform _parentT, Color _color1, Color _color2, float _width1, float _width2, float _duration)
	{
		DebugLines debugLines = DebugLines.Create(_name, _parentT);
		debugLines.duration = _duration;
		LineRenderer lineRenderer = debugLines.line;
		lineRenderer.startColor = _color1;
		lineRenderer.startWidth = _width1;
		lineRenderer.endColor = _color2;
		lineRenderer.endWidth = _width2;
		return debugLines;
	}

	// Token: 0x06008FF1 RID: 36849 RVA: 0x00362130 File Offset: 0x00360330
	public static DebugLines Create(string _name, Transform _parentT, Vector3 _pos1, Vector3 _pos2, Color _color1, Color _color2, float _width1, float _width2, float _duration)
	{
		DebugLines debugLines = DebugLines.Create(_name, _parentT);
		debugLines.duration = _duration;
		LineRenderer lineRenderer = debugLines.line;
		lineRenderer.startColor = _color1;
		lineRenderer.startWidth = _width1;
		lineRenderer.endColor = _color2;
		lineRenderer.endWidth = _width2;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, _pos1 - Origin.position);
		lineRenderer.SetPosition(1, _pos2 - Origin.position);
		return debugLines;
	}

	// Token: 0x06008FF2 RID: 36850 RVA: 0x0036219C File Offset: 0x0036039C
	public static DebugLines CreateAttached(string _name, Transform _parentT, Vector3 _pos1, Vector3 _pos2, Color _color1, Color _color2, float _width1, float _width2, float _duration)
	{
		DebugLines debugLines = DebugLines.Create(_name, _parentT);
		debugLines.duration = _duration;
		LineRenderer lineRenderer = debugLines.line;
		lineRenderer.useWorldSpace = false;
		lineRenderer.startColor = _color1;
		lineRenderer.startWidth = _width1;
		lineRenderer.endColor = _color2;
		lineRenderer.endWidth = _width2;
		lineRenderer.positionCount = 2;
		Vector3 position = _parentT.InverseTransformPoint(_pos1 - Origin.position);
		lineRenderer.SetPosition(0, position);
		Vector3 position2 = _parentT.InverseTransformPoint(_pos2 - Origin.position);
		lineRenderer.SetPosition(1, position2);
		return debugLines;
	}

	// Token: 0x06008FF3 RID: 36851 RVA: 0x00362220 File Offset: 0x00360420
	[PublicizedFrom(EAccessModifier.Private)]
	public static DebugLines Create(string _name, Transform _parentT)
	{
		DebugLines debugLines = null;
		string text = "DebugLines";
		if (_name != null)
		{
			text += _name;
			DebugLines.lines.TryGetValue(_name, out debugLines);
		}
		if (!debugLines)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/Debug/DebugLines"), _parentT);
			gameObject.name = text;
			debugLines = gameObject.transform.GetComponent<DebugLines>();
			if (_name != null)
			{
				debugLines.keyName = _name;
				DebugLines.lines[_name] = debugLines;
			}
		}
		else
		{
			debugLines.transform.SetParent(_parentT, false);
		}
		debugLines.line = debugLines.GetComponent<LineRenderer>();
		debugLines.line.positionCount = 0;
		return debugLines;
	}

	// Token: 0x06008FF4 RID: 36852 RVA: 0x003622BC File Offset: 0x003604BC
	public void AddPoint(Vector3 _pos)
	{
		int positionCount = this.line.positionCount;
		this.line.positionCount = positionCount + 1;
		Vector3 position = _pos - Origin.position;
		if (!this.line.useWorldSpace)
		{
			position = base.transform.InverseTransformPoint(position);
		}
		this.line.SetPosition(positionCount, position);
	}

	// Token: 0x06008FF5 RID: 36853 RVA: 0x00362318 File Offset: 0x00360518
	public void AddCube(Vector3 _cornerPos1, Vector3 _cornerPos2)
	{
		Vector3 pos = _cornerPos1;
		Vector3 vector = _cornerPos2 - _cornerPos1;
		this.AddPoint(pos);
		pos.x += vector.x;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.y -= vector.y;
		this.AddPoint(pos);
		pos.z += vector.z;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.y -= vector.y;
		this.AddPoint(pos);
		pos.x -= vector.x;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.y -= vector.y;
		this.AddPoint(pos);
		pos.z -= vector.z;
		this.AddPoint(pos);
		pos.y += vector.y;
		this.AddPoint(pos);
		pos.x += vector.x;
		this.AddPoint(pos);
		pos.z += vector.z;
		this.AddPoint(pos);
		pos.x -= vector.x;
		this.AddPoint(pos);
		pos.z -= vector.z;
		this.AddPoint(pos);
	}

	// Token: 0x06008FF6 RID: 36854 RVA: 0x0036249E File Offset: 0x0036069E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.duration -= Time.deltaTime;
		if (this.duration <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06008FF7 RID: 36855 RVA: 0x003624CA File Offset: 0x003606CA
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		if (this.keyName != null)
		{
			DebugLines.lines.Remove(this.keyName);
		}
	}

	// Token: 0x04006A68 RID: 27240
	public static Vector3 InsideOffsetV = new Vector3(0.05f, 0.05f, 0.05f);

	// Token: 0x04006A69 RID: 27241
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cName = "DebugLines";

	// Token: 0x04006A6A RID: 27242
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Dictionary<string, DebugLines> lines = new Dictionary<string, DebugLines>();

	// Token: 0x04006A6B RID: 27243
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string keyName;

	// Token: 0x04006A6C RID: 27244
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float duration;

	// Token: 0x04006A6D RID: 27245
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LineRenderer line;
}
