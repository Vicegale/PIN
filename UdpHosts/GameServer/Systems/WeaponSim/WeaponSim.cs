using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using BepuUtilities;
using GameServer.Data.SDB;
using GameServer.Entities;
using GameServer.Entities.Character;
using GameServer.Enums;
using static GameServer.Entities.Character.CharacterEntity;

namespace GameServer;

public class WeaponSim
{
    private Shard _shard;
    private readonly Dictionary<ulong, WeaponSimState> _weaponSimState;

    public WeaponSim(Shard shard)
    {
        _shard = shard;
        _weaponSimState = new();
    }

    public void OnFireWeaponProjectile(CharacterEntity entity, uint time, Vector3 localAimDir)
    {
        // Weapon
        var activeWeaponDetails = entity.GetActiveWeaponDetails();
        if (activeWeaponDetails == null)
        {
            Console.WriteLine($"Will not fire projectile because failed to get active weapon from the entity");
            return;
        }
        var weapon = activeWeaponDetails.Weapon;
        var weaponId = activeWeaponDetails.WeaponId;
        var weaponAttributeSpread = activeWeaponDetails.Spread;
        var weaponAttributeRateOfFire = activeWeaponDetails.RateOfFire;

        // Weapon Sim State
        var weaponSimState = _weaponSimState.GetValueOrDefault(entity.EntityId, new WeaponSimState()); // TODO: Handle switching weapons reset

        if (weaponSimState.LastWeaponId == 0)
        {
            weaponSimState.LastWeaponId = weaponId;
        }
        else if (weaponSimState.LastWeaponId != weaponId)
        {
            // Drop state if we swapped weapon   
            weaponSimState = new WeaponSimState();
            weaponSimState.LastWeaponId = weaponId;
        }

        Console.WriteLine($"Selected weapon {weapon.DebugName} and attribute spread {weaponAttributeSpread}");

        // Ammo
        var ammo = SDBInterface.GetAmmo(weapon.AmmoId); // TODO: Handle ammo overrides
        
        // Projectile origin
        var origin = GetProjectileOrigin(entity, localAimDir);

        // Determine number of rounds to fire with this proj
        // If weapon has burst duration, we expect to receive multiple proj calls and only fire 1.
        byte roundsToFire = 1;
        if (weapon.MsBurstDuration == 0 && weapon.RoundsPerBurst > 1)
        {
            roundsToFire = weapon.RoundsPerBurst;
        }

        // Calculate spreadPct
        float spreadValue, spreadFactor;
        if (weapon.SpreadRampTime > 0)
        {
            uint elapsedTime = time - weaponSimState.LastBurstTime; // The time elapsed since we last fired
            uint returnedTime = Math.Min(Math.Max(0, elapsedTime - weapon.MsSpreadReturnDelay), weapon.MsSpreadReturn); // Subtract delay and account for up to the full return time
            float ratioToReturn = returnedTime / weapon.MsSpreadReturn; // Get how much progress was made
            weaponSimState.Ramp = (uint)(weaponSimState.Ramp * (1f - ratioToReturn)); // Reduce the accumulated time to the remaining amount

            // Calculate spread value based on ramp time
            float rampTimeFactor = weaponSimState.Ramp / weapon.SpreadRampTime;
            spreadValue = weapon.MinSpread + ((weapon.MaxSpread - weapon.MinSpread) * rampTimeFactor);

            // Calcualte spread based on spread value
            spreadFactor = weaponAttributeSpread / weapon.MaxSpread;
        }
        else
        {
            // TODO: How should this work when there is no ramp time?
            spreadValue = weapon.MinSpread;
            spreadFactor = weaponAttributeSpread / weapon.MaxSpread;
        }

        float spreadPct = spreadValue * spreadFactor;
        if (entity.IsAirborne)
        {
            spreadPct += weapon.JumpMinSpread;
        }
        else if (entity.IsMoving)
        {
            spreadPct += weapon.RunMinSpread;
        }

        spreadPct = 2.25f; // TEMP:
        Console.WriteLine($"Firing with spreadPct {spreadPct}");

        // Fire rounds
        for (byte round = 0; round < roundsToFire; round++)
        {
            Vector3 aimForward = localAimDir; // entity.AimDirection;
            Vector3 aimRight = Vector3.Normalize(Vector3.Cross(aimForward, Vector3.UnitZ));
            Vector3 aimUp = Vector3.Normalize(Vector3.Cross(aimRight, aimForward));
            Vector3 lastSpreadDirection = weaponSimState.LastSpreadDirection;
            uint lastSpreadTime = weaponSimState.LastSpreadTime;
            PRNG.Spread(time, weapon.SlotIndex, round, aimForward, aimRight, aimUp, spreadPct, lastSpreadDirection, lastSpreadTime, out Vector3 direction);
            uint trace = PRNG.Trace(time, round);
            _shard.ProjectileSim.FireProjectile(entity, trace, origin, direction, ammo);
            weaponSimState.LastSpreadDirection = direction;
            weaponSimState.LastSpreadTime = time;
        }

        if (time - weaponSimState.LastBurstTime < weapon.MsSpreadReturnDelay + weapon.MsSpreadReturn)
        {
            // FIXME: Just make getters
            if (entity.Character_CombatView.WeaponBurstFiredProp > entity.Character_CombatView.WeaponBurstEndedProp)
            {
                uint fireTime = time - entity.Character_CombatView.WeaponBurstFiredProp;
                weaponSimState.Ramp = Math.Min(weapon.SpreadRampTime, weaponSimState.Ramp + fireTime);
            }
            else
            {
                weaponSimState.Ramp = Math.Min(weapon.SpreadRampTime, weaponSimState.Ramp + (uint)weaponAttributeRateOfFire);
            }
        }
        else
        {
            weaponSimState.Ramp = weapon.MsPerBurst;
        }

        weaponSimState.LastBurstTime = time;
        _weaponSimState[entity.EntityId] = weaponSimState;
    }

    private Vector3 GetProjectileOrigin(CharacterEntity entity, Vector3 aimDirection)
    {
        var muzzleBase = new Vector3(0.2f, 0.0f, 1.62f); // TODO: Should probably vary by character
        if (entity.IsCrouching)
        {
            muzzleBase.Z = 1.08f;
        }
        var muzzleBaseWorld = QuaternionEx.Transform(muzzleBase, QuaternionEx.Inverse(entity.Rotation)); // Match the characters orientation
        var muzzleOffset = new Vector3(aimDirection.X, aimDirection.Y, aimDirection.Z) * 0.1f; // Offset like a sphere based on aim
        var muzzleOffsetWorld = muzzleBaseWorld + muzzleOffset; // Apply offset to base in world
        var origin = entity.Position + muzzleOffsetWorld; // Translate to character
        return origin;
    }


    /*
    public void Tick(double deltaTime, ulong currentTime, CancellationToken ct)
    {
        var players = GetWeaponSimPlayers();
        var entities = GetWeaponSimEntities();
        foreach (var entity in entities)
        {
            ProcessEntity(entity as CharacterEntity);
        }
    }

    private void ProcessEntity(CharacterEntity entity)
    {
        var aimForward = entity.AimDirection;
        // Get active weapon
        // Get active fire mode
    }

    private IEnumerable<INetworkPlayer> GetWeaponSimPlayers()
    {
        return _shard.Clients.Values.Where((client) => (client.Status.Equals(IPlayer.PlayerStatus.Playing) || client.Status.Equals(IPlayer.PlayerStatus.Loading)) && client.NetClientStatus.Equals(Status.Connected));
    }

    private IEnumerable<IEntity> GetWeaponSimEntities()
    {
        return _shard.Entities.Values.Where((entity) => entity is CharacterEntity);
    }
    */

    public class WeaponSimState
    {
        public Vector3 LastSpreadDirection;
        public uint LastSpreadTime;
        public uint LastBurstTime;
        public uint Ramp;
        public uint LastWeaponId;
    }
}