namespace SR2E.Enums;

public enum KeyState : short
{
    Released = 0,
    JustReleased = 1,
    JustPressed = -32767,
    Pressed = -32768,
}
