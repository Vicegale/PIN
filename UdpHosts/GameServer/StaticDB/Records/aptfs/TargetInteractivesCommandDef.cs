namespace GameServer.Data.SDB.Records.aptfs;
public record class TargetInteractivesCommandDef : ICommandDef
{
    public uint Id { get; set; }
    public byte FailNoTargets { get; set; }
}
