using Lumina.Text;

namespace AetherSenseReduxToo.UI.Utils;

public readonly struct MultiString
{
    public static string ParseSeStringLumina(SeString? luminaString)
        => luminaString == null ? string.Empty : Dalamud.Game.Text.SeStringHandling.SeString.Parse(luminaString.RawData).TextValue;
}
