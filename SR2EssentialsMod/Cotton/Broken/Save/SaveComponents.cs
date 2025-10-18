using System;
using System.IO;
using System.Reflection;
/*
namespace CottonLibrary.Save;

public static class SaveComponents
{
    internal static Dictionary<string,Type> components = new Dictionary<string, Type>();
    public static void RegisterComponent(Type componentType)
    {
        var constructor = componentType.GetConstructor(new[] { typeof(BinaryReader), typeof(BinaryWriter) });
        if (constructor == null)
        {
            throw new InvalidOperationException($"Save Component {componentType} must have a constructor with (BinaryReader, BinaryWriter) parameters!");
        }
        dynamic comp = constructor.Invoke(new object[] { null, null });
        components.Add(comp.ComponentIdentifier, componentType);
    }

    public static dynamic LoadComponent(string id, BinaryReader reader)
    {
        var constructor = components[id].GetConstructor(new[] { typeof(BinaryReader), typeof(BinaryWriter) });
        if (constructor == null)
        {
            throw new InvalidOperationException($"Save Component {components[id]} must have a constructor with (BinaryReader, BinaryWriter) parameters!");
        }
        return constructor.Invoke(new object[] { reader, null });
    }
}*/