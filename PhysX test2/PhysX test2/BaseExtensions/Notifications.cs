using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2
{
    public delegate void NotificationEventHandler(object data);

    public class NotificationListener
    {
        public NotificationEventHandler EventHandler;
        public object observer;
        public NotificationListener(NotificationEventHandler eventHandler, object self)
        {
            observer = self;
            EventHandler = eventHandler;
        }
    }

    public class AssociativeContainer<T> where T : class
    {

        public sealed class ContainerComparer : IComparer<AssociativeContainer<T>>
        {
            private static readonly ContainerComparer instance = new ContainerComparer();

            public static ContainerComparer Comparer
            {
                get
                {
                    return instance;
                }
            }

            public int Compare(AssociativeContainer<T> x, AssociativeContainer<T> y)
            {
                return x.key.CompareTo(y.key);
            }
        }

        public static ContainerComparer Comparer
        {
            get
            {
                return ContainerComparer.Comparer;
            }
        }

        public string key;
        public MyContainer<T> objects;

        private AssociativeContainer()
        { }

        public AssociativeContainer(string key)
        {
            this.key = key;
            objects = new MyContainer<T>();
        }
    }

    public class ContainerArray<T> where T : class
    {
        public List<AssociativeContainer<T>> Array;

        public ContainerArray()
        {
            Array = new List<AssociativeContainer<T>>();
        }

        public AssociativeContainer<T> GetContainer(string key)
        {
            return Array.Find(new Predicate<AssociativeContainer<T>>(o => o.key == key));
        }

        public void AddElement(T element, string key)
        {
            AssociativeContainer<T> container = GetContainer(key);
            if (container == null)
            {
                container = new AssociativeContainer<T>(key);
                container.objects.Add(element);
                Array.Add(container);
                Array.Sort(AssociativeContainer<T>.Comparer);
            }
            else
            {
                container.objects.Add(element);
            }
        }

    }

    public class NotificationCenter
    {
        private sealed class NotificationCenterCreator
        {
            private static readonly NotificationCenter instance = new NotificationCenter();

            public static NotificationCenter _NotificationCenter
            {
                get
                {
                    return instance;
                }
            }
        }

        private static NotificationCenter DefaultCenter
        {
            get
            {
                return NotificationCenterCreator._NotificationCenter;
            }
        }

        private ContainerArray<NotificationListener> observers = new ContainerArray<NotificationListener>();

        protected NotificationCenter()
        { }

        public static void addObserver(object observer, NotificationEventHandler eventHandler, string notificationName)
        {
            DefaultCenter.observers.AddElement(new NotificationListener(eventHandler, observer), notificationName);
        }

        public static void postNotification(string name, object data)
        {
            AssociativeContainer<NotificationListener> container = DefaultCenter.observers.GetContainer(name);
            if (container != null)
            {
                foreach (NotificationListener nl in container.objects)
                    nl.EventHandler(data);
            }
        }

        public static void removeObserver(object observer)
        {
            foreach (AssociativeContainer<NotificationListener> ac in DefaultCenter.observers.Array)
            {
                while (ac.objects.Remove(new Predicate<NotificationListener>(o => o.observer == observer))) ;
            }
        }
    }
}
