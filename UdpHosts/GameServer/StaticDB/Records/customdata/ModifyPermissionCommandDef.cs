namespace GameServer.Data.SDB.Records.customdata;
public record class ModifyPermissionCommandDef
{
    public uint Id { get; set; }
    public bool? Glider { get; set; }
    public bool? GliderHud { get; set; }
}