using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Animation
{
    public class Character
    {
        /// <summary>
        /// базовый чар - например один на всех пехотинцев
        /// </summary>
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
        private int[] _currentFrames;

        public Matrix Position;

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

            _currentFrames = new int[startNodes.Length];
            for (int i = 0; i < _currentFrames.Length; i++)
                _currentFrames[i] = -1;

            temp = new DecomposedMatrix[_baseCharacter.skeleton.baseskelet.bones.Count()];
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
            int[] frameNambers=new int[_currentNodes.Length];
            for(int i=0; i<_currentNodes.Length;i++)
            {
                frameNambers[i] = (int)((_currentAnimTime[i] * _currentNodes[i].animationSpeed) % (((FullAnimation)_currentNodes[i].animation).matrices.Length));            
                _currentAnimTime[i] += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            GetFrameMatrix(_currentNodes, _baseCharacter.skeleton.baseskelet, frameNambers, Matrix.Identity);
            return false;
        }


        private DecomposedMatrix[] temp;
        public void GetFrameMatrix(AnimationNode[] ans, Skeleton skeleton, int[] frameNamber, Matrix chahgeRoot)
        {

            int a = 0;
            for (int i = 0; i < ans.Length; i++)
            {
                if (frameNamber[i] != _currentFrames[i])//если у этой части скелета сменился кадр
                    for (int j = 0; j < _baseCharacter.parts[i].animGraph.boneIndexes.Length; j++)
                    {
                        temp[_baseCharacter.parts[i].animGraph.boneIndexes[j]] = ((FullAnimation)ans[i].animation).matrices[frameNamber[i]][j];
                    }
                else //если кадр тотже
                    a++;
                
            }

            if (a != frameNamber.Length) // если a == frameNamber.Length значит матрицы вообще не надо перерасчитывать
            {
                Matrix[] res = DecomposedMatrix.ConvertToMartixArray(temp);
                res[skeleton.RootIndex] = res[skeleton.RootIndex] * chahgeRoot;
                _currentFames = Animation.GetIndependentMatrices(skeleton, res);
                _currentFrames = frameNamber;
            }
        }
    }
}
