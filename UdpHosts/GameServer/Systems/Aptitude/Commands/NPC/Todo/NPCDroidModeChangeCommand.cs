using GameServer.Data.SDB.Records.customdata;

namespace GameServer.Aptitude;

public class NPCDroidModeChangeCommand : ICommand
{
    private NPCDroidModeChangeCommandDef Params;

    public NPCDroidModeChangeCommand(NPCDroidModeChangeCommandDef par)
    {
        Params = par;
    }

    public bool Execute(Context context)
    {
        return true;
    }
}