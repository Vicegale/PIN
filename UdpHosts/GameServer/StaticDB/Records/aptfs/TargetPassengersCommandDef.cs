namespace GameServer.Data.SDB.Records.aptfs;
public record class TargetPassengersCommandDef
{
    public uint Id { get; set; }
    public byte FailNone { get; set; }
    public byte Filter { get; set; }
}
