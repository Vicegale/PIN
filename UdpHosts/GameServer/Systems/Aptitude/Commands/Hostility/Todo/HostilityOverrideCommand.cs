using GameServer.Data.SDB.Records.customdata;

namespace GameServer.Aptitude;

public class HostilityOverrideCommand : Command, ICommand
{
    private HostilityOverrideCommandDef Params;

    public HostilityOverrideCommand(HostilityOverrideCommandDef par)
: base(par)
    {
        Params = par;
    }

    public bool Execute(Context context)
    {
        return true;
    }
}