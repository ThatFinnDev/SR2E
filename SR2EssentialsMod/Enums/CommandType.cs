using System;

namespace SR2E.Enums;

[Flags]
public enum CommandType
{
    None = 0,
    DontLoad = 1 << 1,
    DevOnly = 1 << 2,
    Cheat = 1 << 3,
    Binding = 1 << 4,
    Warp = 1 << 5,
    Common = 1 << 6,
    Menu = 1 << 7,
    Miscellaneous = 1 << 8,
    Fun = 1 << 9,
    Experimental = 1 << 10
}