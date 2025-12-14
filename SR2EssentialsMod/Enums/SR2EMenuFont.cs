using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SR2E.Enums;

[Serializable]
[JsonConverter(typeof(StringEnumConverter))]
public enum SR2EMenuFont
{
    Default=0,
    SR2=1, 
    Bold=2, 
    Regular=3,
    NotoSans=4
}