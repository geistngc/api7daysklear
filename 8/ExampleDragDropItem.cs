using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
[AddComponentMenu("NGUI/Examples/Drag and Drop Item (Example)")]
public class ExampleDragDropItem : UIDragDropItem
{
	// Token: 0x060001A2 RID: 418 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDragDropRelease(GameObject surface)
	{
		if (surface != null)
		{
			ExampleDragDropSurface component = surface.GetComponent<ExampleDragDropSurface>();
			if (component != null)
			{
				GameObject gameObject = component.gameObject.AddChild(this.prefab);
				gameObject.transform.localScale = component.transform.localScale;
				Transform transform = gameObject.transform;
				transform.position = UICamera.lastWorldPosition;
				if (component.rotatePlacedObject)
				{
					transform.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
				}
				NGUITools.Destroy(base.gameObject);
				return;
			}
		}
		base.OnDragDropRelease(surface);
	}

	// Token: 0x04000251 RID: 593
	public GameObject prefab;
}
