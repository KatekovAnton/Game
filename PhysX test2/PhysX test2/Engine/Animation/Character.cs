using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Animation
{
    public class Character
    {
        public CharacterStatic _baseCharacter;
        /// <summary>
        /// текущие ноды анимации (по 1му на каждый кусок чарактера)
        /// </summary>
        public AnimationNode[] _currentNodes;           // текущие узлы- по одному на часть

        /// <summary>
        /// итоговые матрицы для шейдера
        /// </summary>
        public Matrix[] _currentFames;
        /// <summary>
        /// текущее время анимации каждого куска чара 0....1
        /// </summary>
        public float[] _currentAnimTime;

        public Character(CharacterStatic characterBase, string[] startNodes)
        {
            _baseCharacter = characterBase;

            _currentNodes = new AnimationNode[characterBase.parts.Length];
            _currentAnimTime = new float[characterBase.parts.Length];

            _currentFames = new Matrix[_baseCharacter.skeleton.baseskelet.bones.Length];

            for (int i = 0; i < _currentFames.Length; i++)
                _currentFames[i] = Matrix.Identity;
            if (startNodes != null)
                for (int i = 0; i < startNodes.Length; i++)
                    SetCurrentNode(i, startNodes[i]);

           
        }

        /// <summary>
        /// установить другую анимацию
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        public void SetCurrentNode(int index, string name)
        {
            _currentNodes[index] = _baseCharacter.parts[index].animGraph.FindNodeWithName(name);
        }

        /// <summary>
        /// произошло какоето событие персонажа - меняем анимацию
        /// </summary>
        /// <param name="_event"></param>
        public void ReceiveEvent(CharacterEvent _event)
        {
            foreach (AnimationNode node in _currentNodes)
                node.Advance(_event.eventName);
        }


        /// <summary>
        /// на каждый раз обновляем итоговые матрицы для шейдера 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public bool Update(GameTime gameTime)
        {
            ///брать инфу из public AnimationNode[] _currentNodes;как то обрабатывать и строить _currentFames
            return false;
        }
    }
}
