using SR2E.Enums;
using SR2E.Storage;

namespace SR2EExampleExpansion;

public static class StaticClass
{
    // Get events from EVERYWHERE
    // Add a CallOn attribute to a static void
    // Check the tooltip of the enum value to see
    // if it has any custom arguments you might wanna use
    
    [CallOn(CallEvent.AfterGameContextLoad)]
    public static void CustomTest(GameContext gameContext)
    {
        
        MelonLogger.Msg("GameContext has been loaded!");
        MelonLogger.Msg(gameContext.name);
    }
}