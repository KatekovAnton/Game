using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.SceneGraph
{
    class SGQdTree
    {
        public const float maxSize = 8192;
        public const float halfHeight = 128;

        SGNode baseNode;
        private Stack<SGNode> _subtreeStack;

        private int _maxNestingLevel = 7;
        private Stack<SGNode> _nodeStack;
        private Dictionary<int, SGNode> _objectNodeMap;
        private SGNode[,] _leafs;
        private int _leafMassiveSize;
        private float _leafSize;
        private float _updateTime;
        private int _visibleObjectCount;
        private int _nodeTestCount;
        private int _entityTestCount;
        private long _queryTime;
        public int _entityRecalculateCount;
        private Stopwatch _timer;
        Vector3 leafSize;
        public SGQdTree()
        {
            _nodeStack = new Stack<SGNode>();
            Build();
            _timer = new Stopwatch();
            _objectNodeMap = new Dictionary<int, SGNode>();
            _leafs[0, 0].boundingBox.GetSize(out leafSize);
        }

        private void Build()
        {

            _subtreeStack = new Stack<SGNode>();

            BoundingBox rootBbox = new BoundingBox(new Vector3(-maxSize,-halfHeight,-maxSize), new Vector3(maxSize, halfHeight, maxSize));

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
                        foreach (var child in parent.Children)
                            _nodeStack.Push(child);
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

            parent.Children = new SGNode[4];

            //левый верхний передний
            tempBbox.Min = new Vector3(min.X, -halfHeight, min.Z);
            tempBbox.Max = new Vector3(c.X, halfHeight, c.Z);
            parent.Children[0] = new SGNode(parent, tempBbox, newlwvwl);

            //правый верхний передний
            tempBbox.Min = new Vector3(c.X, -halfHeight, min.Z);
            tempBbox.Max = new Vector3(max.X, halfHeight, c.Z);
            parent.Children[1] = new SGNode(parent, tempBbox, newlwvwl);

            //левый верхний задний
            tempBbox.Min = new Vector3(min.X, -halfHeight, c.Z);
            tempBbox.Max = new Vector3(c.X, halfHeight, max.Z);
            parent.Children[2] = new SGNode(parent, tempBbox, newlwvwl);

            //правый верхний задний
            tempBbox.Min = new Vector3(c.X, -halfHeight, c.Z);
            tempBbox.Max = new Vector3(max.X, halfHeight, max.Z);
            parent.Children[3] = new SGNode(parent, tempBbox, newlwvwl);

         

            //заносим новые узлы в массив листьев, если уровень вложенности достиг максимального
            if (newlwvwl >= _maxNestingLevel)
            {
                if (_leafs == null)
                {
                    _leafSize = parent.Children[0].boundingBox.Max.X - parent.Children[0].boundingBox.Min.X;
                    _leafMassiveSize = (int)Extensions.Round(((baseNode.boundingBox.Max.X - baseNode.boundingBox.Min.X) / _leafSize), 0);
                    _leafs = new SGNode[_leafMassiveSize, _leafMassiveSize];
                }
                for (int i = 0; i < 4; i++)
                {
                    //заносим листы в массив
                    Vector3 center;
                    parent.Children[i].boundingBox.GetCenter(out center);
                    int x = (int)(Math.Abs(baseNode.boundingBox.Min.X - center.X) / _leafSize);
                    int z = (int)(Math.Abs(baseNode.boundingBox.Min.Z - center.Z) / _leafSize);
                    _leafs[x, z] = parent.Children[i];
                }
            }
        }
        struct points
        {
            public Point pmin, pmax;
            public points(int minx, int maxx, int miny, int maxy)
            {
                pmin.X = minx;
                pmin.Y = miny;
                pmax.X = maxx;
                pmax.Y = maxy;
            }
        }
        private SGNode GetLeaf(PivotObject entity)
        {
            BoundingBox rootBbox = baseNode.boundingBox;
            

            //получаем индексы листа, в котором расположена точка Min bbox'а объекта
            int minX = (int)((entity.raycastaspect.boundingShape.aabb.XNAbb.Min.X - rootBbox.Min.X) / leafSize.X);
            if (minX < 0)
                minX = 0;

            if (minX > _leafMassiveSize - 1)
                minX = _leafMassiveSize - 1;

            int minZ = (int)((entity.raycastaspect.boundingShape.aabb.XNAbb.Min.Z - rootBbox.Min.Z) / leafSize.Z);
            if (minZ < 0)
                minZ = 0;

            if (minZ > _leafMassiveSize - 1)
                minZ = _leafMassiveSize - 1;


            return _leafs[minX, minZ];
        }

        private void RegistrateEntity(PivotObject entity, SGNode node)
        {
            node.Entities.Add(entity);
            int hashCode = entity.GetHashCode();
            if (_objectNodeMap.ContainsKey(hashCode))
                _objectNodeMap[hashCode] = node;
            else
                _objectNodeMap.Add(hashCode, node);
        }



        public void AddEntity(PivotObject entity)
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
                PivotObject[] objects = n.Entities.ToArray();
                foreach (PivotObject obj in objects)
                    obj.SetVisible(true);
                    
                visibleEntities.AddRange(n.Entities.ToArray());
                if (n.Children != null)
                    for (int i = 0; i < n.Children.Length; i++)
                    {
                        _subtreeStack.Push(n.Children[i]);
                    }
            }
        }

        /// <summary>
        /// for bullets
        /// </summary>
        /// <param name="box"></param>
        /// <param name="intersectedEntities"></param>
        public void Query(BoundingBox box, MyContainer<PivotObject> intersectedEntities)
        { }

        /// <summary>
        /// for explosions
        /// </summary>
        /// <param name="box"></param>
        /// <param name="intersectedEntities"></param>
        public void Query(BoundingSphere sphere, MyContainer<PivotObject> intersectedEntities)
        { }

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
                        {
                            GetSubtree(node, visibleEntities);
                        } break;

                    //case ContainmentType.Disjoint:
                    // ничего не делаем
                    //    break;

                    //если узел пересекается с пирамидой, то проверяе видимость всех его объектов
                    //а вложенные узлы добавляем в стэк для дальнейшей проверки
                    case ContainmentType.Intersects:
                        {
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
                                    wo.SetVisible(true);
                                }
                            }
                            if(node.nestingLevel!=_maxNestingLevel)
                                for (int i = 0; i < node.Children.Length; i++)
                                {
                                    _nodeStack.Push(node.Children[i]);
                                }
                        }break;
                    default: break;
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
            _entityRecalculateCount = 0;
            foreach (PivotObject obj in objects)
            {
                //удаляем объект
                if (obj.moved)
                {
                    //TODO
                    _entityRecalculateCount++;
                    RemoveObject(obj);
                    
                    //добавляем объект в дерево.
                    AddEntity(obj);
                }
            }

            _timer.Stop();
            _updateTime = _timer.ElapsedMilliseconds;
        }
    }
}
