using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine
{
    class fuckenelement
    {
        public int a;
        public fuckenelement(int f)
        {
            a = f;
        }
        public override string ToString()
        {
            return a.ToString();
        }
    }
    public class ebuchest
    {
        public ebuchest()
        {
            MyContainer<fuckenelement> cont = new MyContainer<fuckenelement>(10, 2);
            fuckenelement f1 = new fuckenelement(159);
            fuckenelement f2 = new fuckenelement(34);
            fuckenelement f3 = new fuckenelement(13);
            fuckenelement f4 = new fuckenelement(675);
            fuckenelement f5 = new fuckenelement(64);
            fuckenelement f6 = new fuckenelement(56785);
            fuckenelement f7 = new fuckenelement(25);
            fuckenelement f8 = new fuckenelement(305);
            fuckenelement f9 = new fuckenelement(98);
            fuckenelement[] mas1 = new fuckenelement[] { new fuckenelement(435), new fuckenelement(23) ,
            new fuckenelement(54),new fuckenelement(90),new fuckenelement(65),new fuckenelement(2),
            new fuckenelement(18),new fuckenelement(88),new fuckenelement(91),new fuckenelement(87),new fuckenelement(435), new fuckenelement(23) ,
            new fuckenelement(54),new fuckenelement(90),new fuckenelement(65),new fuckenelement(2),
            new fuckenelement(18),new fuckenelement(88),new fuckenelement(91),new fuckenelement(87),new fuckenelement(435), new fuckenelement(23) ,
            new fuckenelement(54),new fuckenelement(90),new fuckenelement(65),new fuckenelement(2),
            new fuckenelement(18),new fuckenelement(88),new fuckenelement(91),new fuckenelement(87)};
            cont.Add(f1);
            cont.Add(f2); 
            cont.Add(f3);
            cont.Add(f3);
            cont.Add(f4);
            cont.Add(f5);
            cont.Add(f6);

            cont.RemoveAt(4);
            cont.RemoveAt(0);

            cont.Add(f9);
            cont.Add(f8);

            
            cont.RemoveAt(10);

            cont.AddRange(mas1);

            int t = 0;
            int r = t;
        }
    }
    public class MyContainer<T> : IEnumerable<T>
       where T : class
    {
        T[] array;
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
            // firstnullindex = 0;
            //   LastNotNullIndex = -1;

        }
        public MyContainer(int _m, int _n)
        {
            Count = 0;
            IsEmpty = true;
            m = _m;
            n = _n;
            array = new T[m * n];
            //   firstnullindex = 0;
            //  LastNotNullIndex = -1;
        }
        int m, n;
        // int firstnullindex;
        /* public int LastNotNullIndex
         {
             get;
             private set;
         }*/

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                array[i] = null;
            Count = 0;
            // firstnullindex = 0;
            // LastNotNullIndex = -1;
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
                //  LastNotNullIndex += objects.Length;
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

                //   firstnullindex += objects.Length;
            }
            // LastNotNullIndex+=objects.Length;
            Count += objects.Length;
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
            // LastNotNullIndex++;
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
        public void RemoveAt(int index)
        {
            if (Count != 0)
            {
                if (array[index] != null)
                {
                    array[index] = null;
                    array[index] = array[Count - 1];
                    array[Count - 1] = null;

                    Count--;
                    if (Count == 0)
                    {
                        IsEmpty = true;
                    }

                }
            }
        }
    }
}