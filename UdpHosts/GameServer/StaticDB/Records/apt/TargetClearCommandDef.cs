namespace GameServer.Data.SDB.Records.apt;
public record class TargetClearCommandDef
{
    public uint Id { get; set; }
    public byte Former { get; set; }
    public byte Current { get; set; }
}