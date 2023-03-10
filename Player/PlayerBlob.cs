namespace Rift.Player;

public class PlayerBlob
{
    public string CharacterClassAssetId { get; set; }
    public string PlayFabId { get; set; }
    public LeagueTierIds LeagueTierIds { get; set; }
    public string Id { get; set; }
    public LeagueHistory LeagueHistory { get; set; }
    public string DisplayName { get; set; }
    public object CosmeticLoadouts { get; set; }
    public string CurrentAccount { get; set; }
}