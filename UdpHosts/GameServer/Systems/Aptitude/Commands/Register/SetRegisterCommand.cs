using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Data.SDB;
using GameServer.Data.SDB.Records.apt;
using GameServer.Enums;

namespace GameServer.Aptitude;

public class SetRegisterCommand : ICommand
{
    private SetRegisterCommandDef Params;

    public SetRegisterCommand(SetRegisterCommandDef par)
    {
        Params = par;
    }

    public bool Execute(Context context)
    {
        float prevValue = context.Register;
        float paramValue = Params.RegisterVal;
        context.Register = AbilitySystem.RegistryOp(prevValue, paramValue, (Operand)Params.Regop);

        if (true)
        {
            Console.WriteLine($"SetRegisterCommand: ({prevValue}, {paramValue}, {(Operand)Params.Regop}) => {context.Register}");
        }

        return true;
    }
}