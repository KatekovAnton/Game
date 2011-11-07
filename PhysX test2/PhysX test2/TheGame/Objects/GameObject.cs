using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Level;

namespace PhysX_test2.TheGame.Objects
{
    /// <summary>
    /// базовый класс для всех объеков, обеспечивает смены состояний если надо
    /// онже инкапсулирует PivotObject-ы нижнего уровня и переключает их при 
    /// смене состояния геймобжекта (если надо)
    /// </summary>
    public abstract class GameObject
    {
        public GameLevel _hisLevel;
        protected bool _bulletRayCasted;
        protected bool _mouseRayCasted;

        public bool BulletRayCasted
        {
            get { return _bulletRayCasted; }
        }

        public bool MouseRayCasted
        {
            get { return _bulletRayCasted; }
        }

        public GameObject(GameLevel __level, bool __mouseRC, bool __bulletRC)
        {
            _hisLevel = __level;
            _bulletRayCasted = __bulletRC;
            _mouseRayCasted = __mouseRC;
        }

        public virtual void Rotate(float __angle)
        {
 
        }
    }
}
