﻿using System;

namespace SR2E;

[Serializable]
public enum VacModes
{
    AUTO_SHOOT,
    AUTO_VAC,
    NORMAL,
    DRAG,
    NONE,
    LAUNCH,
}

[Serializable]
public enum Trool
{
    False, True, Toggle
}