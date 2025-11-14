using System;
using System.IO;
using Il2CppPlayFab.ClientModels;
/*
namespace CottonLibrary.Save;

public class ModdedV01 : ModdedSaveData<ModdedV01>
{
    public List<dynamic> components = new List<dynamic>();

    public ModdedV01(BinaryReader reader, BinaryWriter writer) : base(reader, writer) { }
    public ModdedV01() { }

    public TC? GetComponent<TC>() where TC : ModdedSaveData<TC>
    {
        foreach (var component in components)
        {
            if (component is TC comp)
            {
                return comp;
            }
        }
        return null;
    }

    public override string ComponentIdentifier => "MSAV";
    public override int ComponentVersion => 1;

    public override void WriteComponent()
    {
        Write(components.Count);
        foreach (var component in components)
            component.WriteData();
    }

    public override void ReadComponent()
    {
        var count =  ReadInt();
        for (int i = 0; i < count; i++)
        {
            var id = ReadString();
            
            var comp = SaveComponents.LoadComponent(id, Reader);
            
            comp?.ReadData();
            if (comp != null)
                components.Add(comp);
        }
    }

    public override void UpgradeComponent(ModdedV01 old)
    {
        throw new NotImplementedException();
    }
}*/