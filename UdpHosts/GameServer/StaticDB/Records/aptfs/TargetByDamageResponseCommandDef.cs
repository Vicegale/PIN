namespace GameServer.Data.SDB.Records.aptfs;
public record class TargetByDamageResponseCommandDef
{
    public uint DamageresponseId { get; set; }
    public float VulnerableTol { get; set; }
    public uint Id { get; set; }
    public byte NotInvulnerable { get; set; }
    public byte VulnerableDamagetypeId { get; set; }
    public byte UseWeaponDamageType { get; set; }
    public byte Negate { get; set; }
}
