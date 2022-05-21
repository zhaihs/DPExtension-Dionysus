﻿using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extension.Ext;
using Extension.Utilities;

namespace ScriptUniversal.Strategy
{
    [Serializable]
    public class WeaponFireStrategy : FireStrategy
    {
        public WeaponFireStrategy(Pointer<TechnoClass> techno, Pointer<WeaponTypeClass> weapon)
        {
            _techno = TechnoExt.ExtMap.Find(techno);
            _weapon = weapon;
        }


        public TechnoExt Techno
        {
            get => _techno;
            set => _techno = value;
        }

        public Pointer<WeaponTypeClass> Weapon
        {
            get => _weapon;
            set => _weapon = value;
        }

        public override int FireTime => Weapon.Ref.Burst;

        public override int CoolDownTime => Weapon.Ref.ROF;

        public event Action<BulletExt> OnCreateBullet;

        public override int GetDelay(int fireTime) => 1;

        public override bool CanAttack(CoordStruct where)
        {
            if (MapClass.Instance.TryGetCellAt(where, out Pointer<CellClass> pCell))
            {
                CoordStruct ground = pCell.Ref.Base.GetCoords();
                if (ground.Z + Game.LevelHeight >= where.Z)
                {
                    return Weapon.Ref.Projectile.Ref.AG;
                }
                else
                {
                    return Weapon.Ref.Projectile.Ref.AA;
                }
            }

            return false;
        }

        public override bool CanAttack(Pointer<AbstractClass> pTarget)
        {
            return Techno != null && Techno.OwnerObject.Ref.CanAttack(pTarget);
        }

        protected override void Fire(CoordStruct where)
        {
            throw new NotImplementedException();
        }

        protected override void Fire(Pointer<AbstractClass> pTarget)
        {
            if (Techno != null)
            {
                Pointer<BulletClass> pBullet = BulletFactory.CreateBullet(pTarget, Weapon, Techno.OwnerObject);
                var pExt = BulletExt.ExtMap.Find(pBullet);
                OnCreateBullet?.Invoke(pExt);
            }
        }

        protected virtual void CreateBullet(Pointer<AbstractClass> pTarget)
        {

        }

        private ExtensionReference<TechnoExt> _techno;
        private SwizzleablePointer<WeaponTypeClass> _weapon;
    }
}
