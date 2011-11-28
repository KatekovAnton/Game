using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Animation
{
    public class CharacterController
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
        /// для одноразовых и переходных анимаций - для одноразовых тут пердыдущая,
        /// для переходных (интрполяция между анимациями) - следующая
        /// </summary>
        private AnimationNode[] _forDropNodes;

        /// <summary>
        /// итоговые матрицы для шейдера
        /// </summary>
        public Matrix[] _currentFrames;
        public DecomposedMatrix[] _sourceFrames;

        /// <summary>
        /// текущее время анимации каждого куска чара 0....1
        /// </summary>
        public float[] _currentAnimTime;
        private int[] _currentFramesInt;

        public Matrix Position;

        public CharacterController(CharacterStatic characterBase, string[] startNodes)
        {
            _baseCharacter = characterBase;

            _currentNodes = new AnimationNode[characterBase.parts.Length];
            _currentAnimTime = new float[characterBase.parts.Length];
            _forDropNodes = new AnimationNode[characterBase.parts.Length];
            _currentFrames = new Matrix[_baseCharacter.skeleton.baseskelet.bones.Length];

            for (int i = 0; i < _currentFrames.Length; i++)
                _currentFrames[i] = Matrix.Identity;
            if (startNodes != null)
                for (int i = 0; i < startNodes.Length; i++)
                    SetCurrentNode(i, startNodes[i]);

            _currentFramesInt = new int[startNodes.Length];
            for (int i = 0; i < _currentFramesInt.Length; i++)
                _currentFramesInt[i] = -1;

            _sourceFrames = new DecomposedMatrix[_baseCharacter.skeleton.baseskelet.bones.Count()];
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
        public void ReceiveEvent(string _event, bool zerodrames)
        {
            for (int i = 0; i < _currentNodes.Length; i++)
            {
                AnimationNode lastnode = _currentNodes[i];
                AnimationNode newnode = _currentNodes[i].Advance(_event);
                _currentNodes[i] = newnode;

                if (newnode.isOneTime && !lastnode.isOneTime)
                    _forDropNodes[i] = lastnode;

                if (zerodrames)
                {
                    _currentFramesInt[i] = 0;
                    _currentAnimTime[i] = 0;
                }
                else
                {
                    while (_currentAnimTime[i] > _currentNodes[i].animTime)
                        _currentAnimTime[i] -= _currentNodes[i].animTime;
                }

            }

            CheckNodes();
        }

        public void CompyPoseFromAnother(CharacterController __anotherController)
        {
            if (__anotherController._baseCharacter != _baseCharacter)
                throw new Exception();

            __anotherController._sourceFrames.CopyTo(_sourceFrames, 0);
        }

        public void MakeUnconditionalTransition(string[] nodeNames, bool __smoothly)
        {
            for (int i = 0; i < _currentNodes.Length; i++)
            {
                AnimationNode node = _baseCharacter.parts[i].NodeWithName(nodeNames[i]);
                if (node != null)
                {
                    _currentNodes[i] = node;
                    while (_currentAnimTime[i] > _currentNodes[i].animTime)
                        _currentAnimTime[i] -= _currentNodes[i].animTime;
                }
            }

            if (__smoothly)
                return;
        }

        public void MakeUnconditionalTransition(string __nodeName, bool __smoothly)
        {
            for (int i = 0; i < _currentNodes.Length; i++)
            {
                AnimationNode node = _baseCharacter.parts[i].NodeWithName(__nodeName);
                if (node != null)
                {
                    _currentAnimTime[i] = 0;
                    if (node == _currentNodes[i])
                        continue;

                    if (node.isOneTime && !_currentNodes[i].isOneTime)
                        _forDropNodes[i] = _currentNodes[i];
                    _currentNodes[i] = node;
                    while (_currentAnimTime[i] > _currentNodes[i].animTime)
                        _currentAnimTime[i] -= _currentNodes[i].animTime;
                }
            }

            CheckNodes();

            if (__smoothly)
                return;

        }

        public void CheckNodes()
        {
            for (int i = 0; i < _currentNodes.Length; i++)
            {
                if (_currentNodes[i].isOneTime && _forDropNodes[i] == null)
                {
                    int a = 0;
                    a++;
                }
                if (_currentNodes[i].isOneTime && _forDropNodes[i].isOneTime)
                {
                    int a = 0;
                    a++;
                }
            }
        }

        /// <summary>
        /// на каждый раз обновляем итоговые матрицы для шейдера 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public void Update(GameTime gameTime, bool __visible = true)
        {   
            int[] frameNambers=new int[_currentNodes.Length];
            for(int i=0; i<_currentNodes.Length;i++)
            {
                frameNambers[i] = (int)(_currentAnimTime[i] * _currentNodes[i].animationSpeed) ;            
                _currentAnimTime[i] += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_currentAnimTime[i] > _currentNodes[i].animTime)
                {
                    if (_currentNodes[i].isOneTime)
                    {
                        if (_forDropNodes[i] == null)
                        {
                            int a = 0;
                            a++;
                        }
                        //go to previous animnode
                        _currentNodes[i] = _forDropNodes[i];
                        _forDropNodes[i] = null;
                    }
                    else
                    {
                        //continue anim
                        _currentAnimTime[i] -= _currentNodes[i].animTime;
                    }
                }
            }
            if(__visible)
                GetFrameMatrix(_currentNodes, _baseCharacter.skeleton, frameNambers, Matrix.Identity);

        }

        public void GetFrameMatrix(AnimationNode[] ans, SkeletonExtended skeleton, int[] frameNamber, Matrix chahgeRoot)
        {
            int a = 0;
            for (int i = 0; i < ans.Length; i++)
            {
                if (frameNamber[i] != _currentFramesInt[i])//если у этой части скелета сменился кадр
                    for (int j = 0; j < _baseCharacter.parts[i].animGraph.boneIndexes.Length; j++)
                    {
                        _sourceFrames[_baseCharacter.parts[i].animGraph.boneIndexes[j]] = ans[i].animation.GetMatrices(frameNamber[i])[j];
                    }
                else //если кадр тотже
                    a++;
            }

            if (a != frameNamber.Length) // если a == frameNamber.Length значит матрицы вообще не надо перерасчитывать
            {
                Matrix[] res = DecomposedMatrix.ConvertToMartixArray(_sourceFrames);
                res[skeleton.TopRootIndex] = res[skeleton.TopRootIndex] * chahgeRoot;
                _currentFrames = Animation.GetIndependentMatrices(skeleton.baseskelet, res);
                _currentFramesInt = frameNamber;
            }
        }
    }
}
