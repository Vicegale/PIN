using GameServer.Data.SDB.Records.aptfs;

namespace GameServer.Aptitude;

public class TargetInteractivesCommand : ICommand
{
    private TargetInteractivesCommandDef Params;

    public TargetInteractivesCommand(TargetInteractivesCommandDef par)
    {
        Params = par;
    }

    public bool Execute(Context context)
    {
        return true;
    }
}