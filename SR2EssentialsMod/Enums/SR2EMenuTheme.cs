using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SR2E.Enums;

[Serializable]
[JsonConverter(typeof(StringEnumConverter))]
public enum SR2EMenuTheme
{
    Default=0, 
    SR2E=1, 
    Black=2
}