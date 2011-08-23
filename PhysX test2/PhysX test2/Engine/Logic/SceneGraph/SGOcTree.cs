using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.SceneGraph
{
    class SGOcTree
    {
        public const float maxSize = 8192;
        SGNode baseNode;
        private Stack<SGNode> _subtreeStack;

        private int _maxNestingLevel = 7;
        private Stack<SGNode> _nodeStack;
        private Dictionary<int, SGNode> _objectNodeMap;
        private Dictionary<int, SGNode> _staticObjectNodeMap;
        private SGNode[, ,] _leafs;
        private int _leafMassiveSize;
        private float _leafSize;
        private float _updateTime;
        private int _visibleObjectCount;
        private int _nodeTestCount;
        private int _entityTestCount;
        private long _queryTime;
        private Stopwatch _timer;

        public SGOcTree()
        {
            _nodeStack = new Stack<SGNode>();
            Build();
            _timer = new Stopwatch();
            _objectNodeMap = new Dictionary<int, SGNode>();
            _staticObjectNodeMap = new Dictionary<int, SGNode>();
            
        }

        private void Build()
        {

            _subtreeStack = new Stack<SGNode>();

            BoundingBox rootBbox = new BoundingBox(new Vector3(-maxSize), new Vector3(maxSize));

            //инициализируем дерево
            baseNode = new SGNode(null, rootBbox, 0);
            _nodeStack.Clear();
            _nodeStack.Push(baseNode);

            while (_nodeStack.Count > 0)
            {
                SGNode parent = _nodeStack.Pop();
                if (parent.nestingLevel < _maxNestingLevel)
                {
                    SplitNode(parent);

                    if (parent.nestingLevel + 1 < _maxNestingLevel)
                    {
                        foreach (var child in parent.Children)
                        {
                            _nodeStack.Push(child);
                        }
                    }
                }
            }
        }

        private void SplitNode(SGNode parent)
        {
            Vector3 min = parent.boundingBox.Min;
            Vector3 max = parent.boundingBox.Max;

            //получаем координаты центра нода
            Vector3 c = (min + max) / 2.0f;

            //получаем дочерние ноды
            BoundingBox tempBbox = new BoundingBox();
            int newlwvwl = parent.nestingLevel + 1;

            parent.Children = new SGNode[8];

            //левый верхний передний
            tempBbox.Min = new Vector3(min.X, c.Y, min.Z);
            tempBbox.Max = new Vector3(c.X, max.Y, c.Z);
            parent.Children[0] = new SGNode(parent, tempBbox, newlwvwl);

            //правый верхний передний
            tempBbox.Min = new Vector3(c.X, c.Y, min.Z);
            tempBbox.Max = new Vector3(max.X, max.Y, c.Z);
            parent.Children[1] = new SGNode(parent, tempBbox, newlwvwl);

            //левый нижний передний
            tempBbox.Min = new Vector3(min.X, min.Y, min.Z);
            tempBbox.Max = new Vector3(c.X, c.Y, c.Z);
            parent.Children[2] = new SGNode(parent, tempBbox, newlwvwl);

            //правый нижний передний
            tempBbox.Min = new Vector3(c.X, min.Y, min.Z);
            tempBbox.Max = new Vector3(max.X, c.Y, c.Z);
            parent.Children[3] = new SGNode(parent, tempBbox, newlwvwl);


            //левый верхний задний
            tempBbox.Min = new Vector3(min.X, c.Y, c.Z);
            tempBbox.Max = new Vector3(c.X, max.Y, max.Z);
            parent.Children[4] = new SGNode(parent, tempBbox, newlwvwl);

            //правый верхний задний
            tempBbox.Min = new Vector3(c.X, c.Y, c.Z);
            tempBbox.Max = new Vector3(max.X, max.Y, max.Z);
            parent.Children[5] = new SGNode(parent, tempBbox, newlwvwl);

            //левый нижний задний
            tempBbox.Min = new Vector3(min.X, min.Y, c.Z);
            tempBbox.Max = new Vector3(c.X, c.Y, max.Z);
            parent.Children[6] = new SGNode(parent, tempBbox, newlwvwl);

            //правый нижний задний
            tempBbox.Min = new Vector3(c.X, min.Y, c.Z);
            tempBbox.Max = new Vector3(max.X, c.Y, max.Z);
            parent.Children[7] = new SGNode(parent, tempBbox, newlwvwl);

            //заносим новые узлы в массив листьев, если уровень вложенности достиг максимального
            if (newlwvwl >= _maxNestingLevel)
            {
                if (_leafs == null)
                {
                    _leafSize = parent.Children[0].boundingBox.Max.X - parent.Children[0].boundingBox.Min.X;
                    _leafMassiveSize = (int)Extensions.Round(((baseNode.boundingBox.Max.X - baseNode.boundingBox.Min.X) / _leafSize), 0);
                    _leafs = new SGNode[_leafMassiveSize, _leafMassiveSize, _leafMassiveSize];
                }
                for (int i = 0; i < 8; i++)
                {
                    //заносим листы в массив
                    Vector3 center;
                    parent.Children[i].boundingBox.GetCenter(out center);
                    int x = (int)(Math.Abs(baseNode.boundingBox.Min.X - center.X) / _leafSize);
                    int y = (int)(Math.Abs(baseNode.boundingBox.Min.Y - center.Y) / _leafSize);
                    int z = (int)(Math.Abs(baseNode.boundingBox.Min.Z - center.Z) / _leafSize);
                    _leafs[x, y, z] = parent.Children[i];
                }
            }
        }

        private SGNode GetLeaf(PivotObject entity)
        {
            Vector3 leafSize;
            
            BoundingBox rootBbox = baseNode.boundingBox;
            _leafs[0, 0, 0].boundingBox.GetSize(out leafSize);

            //получаем индексы листа, в котором расположена точка Min bbox'а объекта
            int minX = (int)((entity.raycastaspect.boundingShape.aabb.XNAbb.Min.X - rootBbox.Min.X) / leafSize.X);
            if (minX < 0)
            {
                minX = 0;
            }

            if (minX > _leafMassiveSize - 1)
            {
                minX = _leafMassiveSize - 1;
            }

            int minY = (int)((entity.raycastaspect.boundingShape.aabb.XNAbb.Min.Y - rootBbox.Min.Y) / leafSize.Y);
            if (minY < 0)
            {
                minY = 0;
            }

            if (minY > _leafMassiveSize - 1)
            {
                minY = _leafMassiveSize - 1;
            }

            int minZ = (int)((entity.raycastaspect.boundingShape.aabb.XNAbb.Min.Z - rootBbox.Min.Z) / leafSize.Z);
            if (minZ < 0)
            {
                minZ = 0;
            }

            if (minZ > _leafMassiveSize - 1)
            {
                minZ = _leafMassiveSize - 1;
            }

            return _leafs[minX, minY, minZ];
        }

        private void RegistrateEntity(PivotObject entity, SGNode node)
        {
            node.Entities.Add(entity);
            int hashCode = entity.GetHashCode();
            if (_objectNodeMap.ContainsKey(hashCode))
            {
                _objectNodeMap[hashCode] = node;
            }
            else
            {
                _objectNodeMap.Add(hashCode, node);
            }
        }


        private void AddEntity(PivotObject entity)
        {
            //получаем лист, в котором находится объект
            SGNode leaf = GetLeaf(entity);

            _nodeStack.Clear();
            _nodeStack.Push(leaf);

            while (_nodeStack.Count > 0)
            {
                SGNode node = _nodeStack.Pop();

                ContainmentType containmentType = ContainmentType.Disjoint;

                node.boundingBox.Contains(ref entity.raycastaspect.boundingShape.aabb.XNAbb, out containmentType);
                switch (containmentType)
                {
                    case ContainmentType.Contains: //если объект полностью помещается в узел, то регистрируем его в этом узле
                        {
                            RegistrateEntity(entity, node);
                        } break;
                    case ContainmentType.Disjoint://если объект не пересекается с узлом вообще, то значит объект вышел за пределы дерева
                        {
                            //поэтому регистрируем его в листе, который вычислили ранее
                            RegistrateEntity(entity, leaf);
                        } break;
                    case ContainmentType.Intersects://если объект пересекается с узлом, то нужно проверить является ли узел корневым
                        {
                            if (node.ParentNode == null)
                                //если да, то объект лежит на границе дерева, поэтому регистрируем его в ЛИСТЕ
                                RegistrateEntity(entity, leaf);
                            else
                                //если нет, то передаем объект в родительский узел
                                _nodeStack.Push(node.ParentNode);
                        } break;
                }
            }
        }

        private void GetSubtree(SGNode node, MyContainer<PivotObject> visibleEntities)
        {
            _subtreeStack.Clear();
            _subtreeStack.Push(node);

            while (_subtreeStack.Count > 0)
            {
                SGNode n = _subtreeStack.Pop();
                visibleEntities.AddRange(n.Entities.ToArray());

                for (int i = 0; i < n.Children.Length; i++)
                {
                    _subtreeStack.Push(n.Children[i]);
                }
            }
        }

        public void Query(BoundingFrustum frustum, MyContainer<PivotObject> visibleEntities)
        {
            _visibleObjectCount = 0;
            _nodeTestCount = 0;
            _entityTestCount = 0;
            _timer.Reset();
            _timer.Start();

            ContainmentType containmentType = ContainmentType.Disjoint;

            _nodeStack.Clear();
            _nodeStack.Push(this.baseNode);

            while (_nodeStack.Count > 0)
            {
                SGNode node = _nodeStack.Pop();
                _nodeTestCount++;
                frustum.Contains(ref node.boundingBox, out containmentType);

                switch (containmentType)
                {
                    //если узел полностью входит в пирамиду,
                    //то заносим все поддерево в список видимых сущностей
                    case ContainmentType.Contains:
                        GetSubtree(node, visibleEntities);
                        break;

                    //case ContainmentType.Disjoint:
                    // ничего не делаем
                    //    break;

                    //если узел пересекается с пирамидой, то проверяе видимость всех его объектов
                    //а вложенные узлы добавляем в стэк для дальнейшей проверки
                    case ContainmentType.Intersects:
                        ContainmentType entContType = ContainmentType.Disjoint;
                        for (int i = 0; i < node.Entities.Count; i++)
                        {
                            PivotObject wo = node.Entities[i];
                            _entityTestCount++;
                            entContType = ContainmentType.Disjoint;
                            frustum.Contains(ref wo.raycastaspect.boundingShape.aabb.XNAbb, out entContType);
                            if (entContType != ContainmentType.Disjoint)
                            {
                                visibleEntities.Add(wo);
                            }
                        }

                        for (int i = 0; i < node.Children.Length; i++)
                        {
                            _nodeStack.Push(node.Children[i]);
                        }

                        break;
                }
            }

            _visibleObjectCount = visibleEntities.Count;
            _timer.Stop();
            _queryTime = _timer.ElapsedMilliseconds;
        }

        public bool RemoveObject(PivotObject wo)
        {
            int hashCode = wo.GetHashCode();
            if (_objectNodeMap.ContainsKey(hashCode))
            {
                SGNode node = _objectNodeMap[hashCode];
                bool r1 = node.Entities.Remove(wo);
                bool r2 = _objectNodeMap.Remove(hashCode);

                return r1 && r2;
            }

            return false;
        }
        
        /// <summary>
        /// ВОТ ТУТ БЛЯТЬ ВСЁ МЕНЯТЬ НАХУЙ ЧТОБ АПДЕЙТИЛИСЬ ТОКА ДВИГАВШИЕСЯ
        /// </summary>
        /// <param name="objects"></param>
        public void Update(MyContainer<PivotObject> objects)
        {
            _timer.Reset();
            _timer.Start();

            foreach (PivotObject obj in objects)
            {
                //удаляем объект
                bool result = RemoveObject(obj);
                Debug.Assert(result, "Не удалось удалить объект из дерева.");

                //добавляем объект в дерево.
                AddEntity(obj);
            }

            _timer.Stop();
            _updateTime = _timer.ElapsedMilliseconds;
        }
    }
}
