using System;
using Starlight.Components.AssetBundle;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Starlight.UI.Blueprints;

public abstract class UIBlueprint
{
    public static float ScaleFactor => Screen.currentResolution.height / 1080f;
    internal static float ScaleFactorY => Screen.currentResolution.height / 1080f;
    internal static float ScaleFactorX => Screen.currentResolution.height / 1080f;//Screen.width / 1080f;
    [Obsolete("Please use Name instead")]public string mame = null;
    public string Name = "<MissingName>"; 
    public Vector2 Size = new(100,100);
    public Vector2 Position = Vector2.zero;
    public Vector2 Rotation = new ();
    public Vector4 Anchors = new (0.5f, 0.5f, 0.5f, 0.5f);
    public Vector2 Pivot = new (0.5f, 0.5f);
    public List<UIBlueprint> Children;
    public int CornerRadius = 0;
    protected RectTransform CustomChildHolder;
    protected bool IgnoreCorners = false;
    public List<Il2CppSystem.Type> Components;
    public RectTransform Render(UITheme theme, FontTheme fontTheme, Transform parent)
    {
        var obj = new GameObject(mame??Name);
        obj.transform.localRotation=Quaternion.Euler(Rotation.x,Rotation.y,0);
        var rectT = obj.AddComponent<RectTransform>();
        obj.transform.SetParent(parent);
        rectT.pivot = Pivot;
        rectT.sizeDelta = new Vector2(Size.x*ScaleFactorX, Size.y*ScaleFactorY);
        rectT.anchoredPosition = Position*ScaleFactor;
        rectT.anchorMin = new Vector2(Anchors.x, Anchors.y);
        rectT.anchorMax = new Vector2(Anchors.z, Anchors.w);
        try { OnRender(theme, fontTheme, rectT); } catch (Exception e) { LogError(e); }

        if(CornerRadius>0&&!IgnoreCorners)
        {
            var sortGroup = obj.AddComponent<SortingGroup>();
            sortGroup.enabled = false;
            sortGroup.sortingOrder = Mathf.FloorToInt(CornerRadius * ScaleFactor);
            obj.AddComponent<RoundedUIImage>().CornerRadius = CornerRadius * ScaleFactor;
        }
        
        if(Components != null)
            foreach (var comp in Components)
                try { obj.AddComponent(comp); } catch (Exception e) { LogError(e); } 
        
        if (Children != null)
            foreach (var child in Children)
                try {  child.Render(theme, fontTheme, CustomChildHolder ?? rectT); } catch (Exception e) { LogError(e); } 
        
        try { AfterRenderChildren(theme, fontTheme, rectT); } catch (Exception e) { LogError(e); }
        return rectT;
    }
    
    protected abstract void OnRender(UITheme theme, FontTheme fontTheme, RectTransform obj);
    protected virtual void AfterRenderChildren(UITheme theme, FontTheme fontTheme, RectTransform obj) { }
}
