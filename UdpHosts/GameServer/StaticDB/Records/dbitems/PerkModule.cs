namespace GameServer.Data.SDB.Records.dbitems;
public record class PerkModule
{
    public uint PerkType { get; set; }
    public uint PerkCostCategory { get; set; }
    public uint PrereqPerkId { get; set; }
    public uint PerkFlags { get; set; }
    public uint PointsCost { get; set; }
    public uint Id { get; set; }
}
