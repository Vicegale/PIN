namespace GameServer.Data.SDB.Records.aptfs;
public record class RequireEnergyFromTargetCommandDef
{
    public uint Id { get; set; }
    public float Amount { get; set; }
    public byte AmountRegop { get; set; }
    public byte Negate { get; set; }
}
