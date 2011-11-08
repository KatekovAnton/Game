using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2
{
    

    public class MyContainer<T> : IEnumerable<T>
       where T : class
    {
        public class MyContainerRule
        {
            public T firstObject;
            public int firstObjectIndex;
            public T secondObject;
            public int secondObjectIndex;

            public MyContainerRule(T _first, T _second, int _fi, int _si)
            {
                firstObject = _first;
                secondObject = _second;
                firstObjectIndex = _fi;
                secondObjectIndex = _si;
            }
        }
        
        List<MyContainerRule> rules;
        T[] array;
        int m, n;

        public IEnumerator<T> GetEnumerator()
        {
            for (int iii = 0; iii < Count; iii++)
                if (array[iii] != null)
                    yield return array[iii];
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int iii = 0; iii < Count; iii++)
                if (array[iii] != null)
                    yield return array[iii];
        }

        public MyContainer()
        {
            Count = 0;
            IsEmpty = true;
            m = 10;
            n = 1;
            array = new T[m * n];
            rules = new List<MyContainerRule>();
        }

        public MyContainer(int _m, int _n)
        {
            Count = 0;
            IsEmpty = true;
            m = _m;
            n = _n;
            array = new T[m * n];
            rules = new List<MyContainerRule>();
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                array[i] = null;
            Count = 0;
            IsEmpty = true;
        }

        public static int CompareByNull(T value1, T value2)
        {
            if (value1 == null)
                return -1;
            if (value2 == null)
                return 1;
            return 0;
        }

        public void AddRange(T[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
                if (objects[i] == null)
                    throw new Exception("Array cannot to contain zeros!!!");


            if (objects.Length + this.Count <= array.Length)
            {
                //если мы можем добавить, не расширяя массива
                objects.CopyTo(array, Count);
            }
            else
            {
                //надо расширять основной массив
                int neededsize = array.Length + objects.Length;
                double times = Math.Floor((double)(objects.Length - (array.Length - Count)) / (double)(m * n)) + 1.0;

                int reserve = m * n * (int)((double)n * times);

                T[] tmparray = new T[array.Length];
                array.CopyTo(tmparray, 0);
                array = new T[array.Length + reserve];
                tmparray.CopyTo(array, 0);

                objects.CopyTo(array, Count);
            }
            Count += objects.Length;
        }

        public int IndexOf(T neededObject)
        {
            for (int i = 0; i < Count; i++)
                if (neededObject == array[i])
                    return i;
            return -1;
        }

        //TODO - test it!!
        public void AddRule(T firstObject, T secondOnject)
        {
            int fi = IndexOf(firstObject);
            if (fi == -1)
                return;
            int si = IndexOf(secondOnject);
            if (si == -1)
                return;
            if (fi == si)
                return;
            if (fi > si)
            {
                //swap before finish
                array[fi] = secondOnject;
                array[si] = firstObject;
            }
            rules.Add(new MyContainerRule(array[fi], array[si], fi, si));
        }

        public void Add(T @object)
        {
            if (Count == array.Length - 1)
            {
                T[] tmparray = new T[array.Length];
                array.CopyTo(tmparray, 0);
                array = new T[array.Length + m * n];
                tmparray.CopyTo(array, 0);
            }
            array[Count] = @object;
            Count++;
            IsEmpty = false;
        }

        public T this[int index]
        {
            get
            {
                return array[index];
            }
        }

        public bool IsEmpty
        {
            get;
            private set;
        }

        public int Count
        {
            get;
            private set;
        }

        public bool Remove(T element)
        {
            if (!IsEmpty)
                for (int index = 0; index < Count; index++)
                    if (array[index] == element)
                    {
                        RemoveAt(index);
                        return true;
                    }
            return false;
        }

        public bool Remove(Predicate<T> match)
        {
            if (!IsEmpty)
                for (int index = 0; index < Count; index++)
                    if (match(array[index]))
                    {
                        RemoveAt(index);
                        return true;
                    }
            return false;
        }

        private int FindRuleForSecondObject(T neededObject, out MyContainerRule rule)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                if (rules[i].secondObject == neededObject)
                {
                    rule = rules[i];
                    return i;
                }
            }
            rule = null;
            return -1;
        }

        public void RemoveAllRulesForObject(T neededObject)
        {
            bool removed = true;
            while (removed)
            {
                removed = false;
                foreach (MyContainerRule rule in rules)
                    if (rule.firstObject == neededObject)
                    {
                        rules.Remove(rule);
                        removed = true;
                        break;
                    }
                    else if (rule.secondObject == neededObject)
                    {
                        rules.Remove(rule);
                        removed = true;
                        break;
                    }
            }
        }

        public List<MyContainerRule> FindAllRulesForObject(T neededObject)//TODO TEST
        {
            List<MyContainerRule> rulesResult = new List<MyContainerRule>();
            foreach (MyContainerRule rule in rules)
                if (rule.firstObject == neededObject || rule.secondObject == neededObject)
                    rulesResult.Add(rule);
                    
            return rulesResult;
        }

        public void RemoveAt(int index)
        {
            if (Count != 0)
            {
                if (array[index] != null)
                {
                    //удалить все правила, в которых первый объект - тот который мы удаляем
                    RemoveAllRulesForObject(array[index]);
                    MyContainerRule rule = null;
                    int ruleposition = FindRuleForSecondObject(array[Count - 1], out rule);
                    if (ruleposition != -1)
                    {
                        if (rule.firstObjectIndex > index)
                        {
                            //последний объект встанет перед тем, после которого должен находиться
                            array[index] = null;
                            array[index] = array[rule.firstObjectIndex];
                            array[rule.firstObjectIndex] = array[rule.secondObjectIndex];
                            array[Count - 1] = null;

                            rule.secondObjectIndex = rule.firstObjectIndex;
                            rule.firstObjectIndex = index;
                        }
                        else
                        {
                            //он назначен, но перемещается в позицию после
                            array[index] = null;
                            array[index] = array[Count - 1];
                            array[Count - 1] = null;

                            rule.secondObjectIndex = index;
                        }
                    }
                    else
                    {
                        //никаких правил - удаляем и всё
                        array[index] = null;
                        array[index] = array[Count - 1];
                        array[Count - 1] = null;
                    }
                    Count--;
                    if (Count == 0)
                    {
                        IsEmpty = true;
                    }
                }
            }
        }

        public void Swap(T __oldobject, T __newobject, bool __searchSame = false)
        {
            if (__searchSame)
                Remove(__newobject);

            for (int i = 0; i < Count; i++)
                if (array[i] == __oldobject)
                {
                    array[i] = __newobject;
                    break;
                }
        }
    }
}