using System;

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

public enum Key
{
    None = 0,
    // ----- Alphanumeric Keys -----
    A = 0x41, B = 0x42, C = 0x43, D = 0x44, E = 0x45, F = 0x46, G = 0x47, H = 0x48,
    I = 0x49, J = 0x4A, K = 0x4B, L = 0x4C, M = 0x4D, N = 0x4E, O = 0x4F, P = 0x50,
    Q = 0x51, R = 0x52, S = 0x53, T = 0x54, U = 0x55, V = 0x56, W = 0x57, X = 0x58,
    Y = 0x59, Z = 0x5A,

    // ----- Numeric Keys -----
    Digit0 = 0x30, Digit1 = 0x31, Digit2 = 0x32, Digit3 = 0x33, Digit4 = 0x34, Digit5 = 0x35,
    Digit6 = 0x36, Digit7 = 0x37, Digit8 = 0x38, Digit9 = 0x39,

    // ----- Special Characters -----
    Exclamation = 0x21, DoubleQuote = 0x22, Hash = 0x23, Dollar = 0x24, Percent = 0x25,
    Ampersand = 0x26, Quote = 0x27, LeftParenthesis = 0x28, RightParenthesis = 0x29,
    Asterisk = 0x2A, Plus = 0x2B, Comma = 0x2C, Minus = 0x2D, Period = 0x2E, Slash = 0x2F,
    Colon = 0x3A, Semicolon = 0x3B, LessThan = 0x3C, Equals = 0x3D, GreaterThan = 0x3E,
    Question = 0x3F, At = 0x40,

    // ----- Brackets -----
    LeftBracket = 0xDB, RightBracket = 0xDD, LeftCurlyBracket = 0xDB, RightCurlyBracket = 0xDD,

    // ----- Arrow Keys -----
    UpArrow = 0x26, DownArrow = 0x28, RightArrow = 0x27, LeftArrow = 0x25,

    // ----- Function Keys -----
    F1 = 0x70, F2 = 0x71, F3 = 0x72, F4 = 0x73, F5 = 0x74, F6 = 0x75, F7 = 0x76,
    F8 = 0x77, F9 = 0x78, F10 = 0x79, F11 = 0x7A, F12 = 0x7B,

    // ----- Modifier Keys -----
    LeftShift = 0xA0, RightShift = 0xA1, LeftControl = 0xA2, RightControl = 0xA3,
    LeftAlt = 0xA4, RightAlt = 0xA5, LeftCommand = 0xDB, RightCommand = 0xDC,
    LeftWindows = 0x5B, RightWindows = 0x5C,

    // ----- Navigation and Editing Keys -----
    Space = 0x20, Enter = 0x0D, Escape = 0x1B, Backspace = 0x08, Tab = 0x09, 
    CapsLock = 0x14, Numlock = 0x90, ScrollLock = 0x91,

    // ----- Keypad Keys -----
    Keypad0 = 0x60, Keypad1 = 0x61, Keypad2 = 0x62, Keypad3 = 0x63, Keypad4 = 0x64,
    Keypad5 = 0x65, Keypad6 = 0x66, Keypad7 = 0x67, Keypad8 = 0x68, Keypad9 = 0x69,
    KeypadPeriod = 0x6E, KeypadDivide = 0x6F, KeypadMultiply = 0x6A, KeypadMinus = 0x6D,
    KeypadPlus = 0x6B, KeypadEnter = 0x6C, KeypadEquals = 0x6F,

    // ----- Miscellaneous Keys -----
    Insert = 0x2D, Home = 0x24, End = 0x23, PageUp = 0x21, PageDown = 0x22,
    Print = 0x2C, Menu = 0x2F, Help = 0x6A, Break = 0x13, SysReq = 0x5C,

    // ----- Mouse Buttons -----
    Mouse0 = 0x100, Mouse1 = 0x101, Mouse2 = 0x102, Mouse3 = 0x103, Mouse4 = 0x104,
    Mouse5 = 0x105, Mouse6 = 0x106
    
    // ----- Joystick Buttons -----
    /*, JoystickButton0 = 0x120, JoystickButton1 = 0x121,
    JoystickButton2 = 0x122, JoystickButton3 = 0x123, JoystickButton4 = 0x124,
    JoystickButton5 = 0x125, JoystickButton6 = 0x126, JoystickButton7 = 0x127,
    JoystickButton8 = 0x128, JoystickButton9 = 0x129*/
}
