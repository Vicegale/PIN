namespace GameServer.Data.SDB.Records.aptfs;
public record class ConsumeEnergyOverTimeCommandDef
{
    public float Amount { get; set; }
    public uint Id { get; set; }
    public byte AmountRegop { get; set; }
    public byte AllowPrediction { get; set; }
}
