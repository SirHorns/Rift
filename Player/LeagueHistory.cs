namespace Rift.Player;

public class LeagueHistory
{
    public bool bLastSeasonCompleted { get; set; }
    public int LastSeason { get; set; }
    public PreviousSeasonLeagueTierIds PreviousSeasonLeagueTierIds { get; set; }
    public bool bShouldPlayerSeeSeasonEndRewards { get; set; }
    public object HighestRankedTierIds { get; set; }
}