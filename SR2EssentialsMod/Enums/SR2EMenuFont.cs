using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SR2E.Enums;

[Serializable]
[JsonConverter(typeof(StringEnumConverter))]
public enum SR2EMenuFont
{
    Default, SR2, Bold, Regular
}