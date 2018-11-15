using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ImageAspectRatioFitter : UIBehaviour, ILayoutSelfController, ILayoutElement {
    public enum AspectMode {
        None,
        WidthControlsHeight,
        HeightControlsWidth
    }
    public AspectMode aspectMode;
    private DrivenRectTransformTracker drivenRectTransformTracker;
    protected override void OnEnable () {
        base.OnEnable();
        UpdateRect();
    }

    protected override void OnDisable () {
        base.OnDisable();
        drivenRectTransformTracker.Clear();
    }

    protected override void OnRectTransformDimensionsChange () {
        UpdateRect();
    }

    public void SetLayoutHorizontal () {}
    public void SetLayoutVertical () {}

    private bool updatingRect = false;
    private void UpdateRect () {
        if (!IsActive()) return;
        if (updatingRect) return;

        updatingRect = true;

        var rt = GetComponent<RectTransform>();
        drivenRectTransformTracker.Clear();

        var spr = GetComponent<Image>().sprite;
        if (spr != null) {
            var aspect = (float)spr.rect.width / spr.rect.height;
            switch (aspectMode) {
            case AspectMode.WidthControlsHeight:
                if (layoutElement) {
                    preferredHeight = rt.sizeDelta.y / aspect;
                } else {
                    drivenRectTransformTracker.Add(this, rt, DrivenTransformProperties.SizeDeltaY);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rt.rect.width / aspect);
                }
                break;
            case AspectMode.HeightControlsWidth:
                if (layoutElement) {
                    preferredWidth = aspect * rt.sizeDelta.y;
                } else {
                    drivenRectTransformTracker.Add(this, rt, DrivenTransformProperties.SizeDeltaX);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, aspect * rt.rect.height);
                }
                break;
            }
        }
        updatingRect = false;
    }

#if UNITY_EDITOR
    protected override void OnValidate () {
        base.OnValidate();
        UpdateRect();
    }
#endif
#region ILayoutElement
    public bool layoutElement;

    public int layoutPriority {
        get {
            return layoutElement ? 1 : -1;
        }
    }

    public float flexibleWidth {
        get {
            return 0;
        }
    }

    public float flexibleHeight {
        get {
            return 0;
        }
    }

    public float minHeight {
        get {
            return 0;
        }
    }

    public float minWidth {
        get {
            return 0;
        }
    }

    public float preferredWidth { get; private set; }

    public float preferredHeight { get; private set; }

    public void CalculateLayoutInputHorizontal () {
        UpdateRect();
    }

    public void CalculateLayoutInputVertical () {
        UpdateRect();
    }
#endregion
}
