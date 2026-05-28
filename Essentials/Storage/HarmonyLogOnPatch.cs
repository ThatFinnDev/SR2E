using System;

namespace Starlight.Storage;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal class HarmonyLogOnPatch : Attribute
{
}