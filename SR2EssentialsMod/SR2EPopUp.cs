﻿using System;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using Il2CppTMPro;
using SR2E.Commands;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Patches.Context;
using SR2E.Storage;

namespace SR2E;
/// <summary>
/// Abstract menu class
/// </summary>
[RegisterTypeInIl2Cpp(false)]
public abstract class SR2EPopUp : MonoBehaviour
{
    internal Transform block;

    public static void PreAwake(GameObject obj,List<object> objects) {}
    private void disableBlock()
    {
        if(block!=null)
            Destroy(block.gameObject);
    }
    public virtual void ApplyFont(TMP_FontAsset font)
    {
        foreach (var text in gameObject.getAllChildrenOfType<TMP_Text>())
            text.font = font;
    }
    
    public new void Close()
    {
        disableBlock();
        openPopUps.Remove(this);
        Destroy(gameObject);
    }

    protected static void _Open(string identifier,Type type,SR2EMenuTheme theme,List<object> objects)
    {
        var asset = SystemContextPatch.bundle.LoadAsset(SystemContextPatch.getPopUpPath(identifier,theme));
        var Object = GameObject.Instantiate(asset, SR2EEntryPoint.SR2EStuff.transform);
        ExecuteInTicks((Action)(() =>
        {
            for (int i = 0; i < SR2EEntryPoint.SR2EStuff.transform.childCount; i++)
            {
                Transform child = SR2EEntryPoint.SR2EStuff.transform.GetChild(i);
                if (child.name == Object.name)
                {
                    try
                    {
                        var methodInfo = type.GetMethod(nameof(SR2EPopUp.PreAwake), BindingFlags.Static | BindingFlags.Public);
                        var result = methodInfo.Invoke(null, new object[] { child.gameObject,objects });
                        child.gameObject.SetActive(true);
                    }catch (Exception e) { MelonLogger.Error(e); }
                }
            }
        }), 1);
    }
    protected virtual void OnOpen() {}
    public void Awake()
    {
        OpenPopUpBlock(this);
        openPopUps.Add(this);
        
    }

    private void Start()
    {
        OnOpen();
    }

    protected void Update()
    {
        OnUpdate();
    } protected virtual void OnUpdate() {}
    
}

