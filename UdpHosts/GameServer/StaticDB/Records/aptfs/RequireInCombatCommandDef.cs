namespace GameServer.Data.SDB.Records.aptfs;
public record class RequireInCombatCommandDef
{
    public uint Id { get; set; }
    public byte Negate { get; set; }
    public byte AllowPrediction { get; set; }
}
