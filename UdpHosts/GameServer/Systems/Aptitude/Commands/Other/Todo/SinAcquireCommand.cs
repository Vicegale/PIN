using GameServer.Data.SDB.Records.customdata;

namespace GameServer.Aptitude;

public class SinAcquireCommand : Command, ICommand
{
    private SinAcquireCommandDef Params;

    public SinAcquireCommand(SinAcquireCommandDef par)
: base(par)
    {
        Params = par;
    }

    public bool Execute(Context context)
    {
        return true;
    }
}