using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework.ObjectPool
{
    public static class ObjectPool
    {
        /************************************************************************************************************************/

        /// <summary>Returns a spare item if there are any, or creates a new one.</summary>
        /// <remarks>Remember to <see cref="Release{T}(T)"/> it when you are done.</remarks>
        public static T Acquire<T>()
            where T : class, new()
            => ObjectPool<T>.Acquire();

        /// <summary>Returns a spare `item` if there are any, or creates a new one.</summary>
        /// <remarks>Remember to <see cref="Release{T}(T)"/> it when you are done.</remarks>
        public static void Acquire<T>(out T item)
            where T : class, new()
            => item = ObjectPool<T>.Acquire();

        /************************************************************************************************************************/

        /// <summary>Adds the `item` to the list of spares so it can be reused.</summary>
        public static void Release<T>(T item)
            where T : class, new()
            => ObjectPool<T>.Release(item);

        /// <summary>Adds the `item` to the list of spares so it can be reused and sets it to <c>null</c>.</summary>
        public static void Release<T>(ref T item) where T : class, new()
        {
            ObjectPool<T>.Release(item);
            item = null;
        }

        public static List<T> AcquireList<T>()
        {
            return ObjectPool<List<T>>.Acquire();
        }

        /// <summary>Returns a spare <see cref="List{T}"/> if there are any, or creates a new one.</summary>
        /// <remarks>Remember to <see cref="Release{T}(List{T})"/> it when you are done.</remarks>
        public static void Acquire<T>(out List<T> list)
            => list = AcquireList<T>();

        /// <summary>Clears the `list` and adds it to the list of spares so it can be reused.</summary>
        public static void Release<T>(List<T> list)
        {
            list.Clear();
            ObjectPool<List<T>>.Release(list);
        }

        /// <summary>Clears the `list`, adds it to the list of spares so it can be reused, and sets it to <c>null</c>.</summary>
        public static void Release<T>(ref List<T> list)
        {
            Release(list);
            list = null;
        }

        /************************************************************************************************************************/
        public static Dictionary<TK,TV> AcquireDictionary<TK,TV>()
        {
            return ObjectPool<Dictionary<TK,TV>>.Acquire();
        }

        public static void Acquire<TK, TV>(out Dictionary<TK, TV> dict)
            => dict = AcquireDictionary<TK, TV>();

        /// <summary>Clears the `list` and adds it to the list of spares so it can be reused.</summary>
        public static void Release<TK, TV>(Dictionary<TK, TV> list)
        {
            list.Clear();
            ObjectPool<Dictionary<TK, TV>>.Release(list);
        }

        /// <summary>Clears the `list`, adds it to the list of spares so it can be reused, and sets it to <c>null</c>.</summary>
        public static void Release<TK, TV>(ref Dictionary<TK, TV> list)
        {
            Release(list);
            list = null;
        }


        /************************************************************************************************************************/

        /// <summary>Returns a spare <see cref="HashSet{T}"/> if there are any, or creates a new one.</summary>
        /// <remarks>Remember to <see cref="Release{T}(HashSet{T})"/> it when you are done.</remarks>
        public static HashSet<T> AcquireSet<T>()
        {
            return ObjectPool<HashSet<T>>.Acquire();
        }

        /// <summary>Returns a spare <see cref="HashSet{T}"/> if there are any, or creates a new one.</summary>
        /// <remarks>Remember to <see cref="Release{T}(HashSet{T})"/> it when you are done.</remarks>
        public static void Acquire<T>(out HashSet<T> set)
            => set = AcquireSet<T>();

        /// <summary>Clears the `set` and adds it to the list of spares so it can be reused.</summary>
        public static void Release<T>(HashSet<T> set)
        {
            set.Clear();
            ObjectPool<HashSet<T>>.Release(set);
        }

        /// <summary>Clears the `set`, adds it to the list of spares so it can be reused, and sets it to <c>null</c>.</summary>
        public static void Release<T>(ref HashSet<T> set)
        {
            Release(set);
            set = null;
        }

        /************************************************************************************************************************/

        /// <summary>Returns a spare <see cref="StringBuilder"/> if there are any, or creates a new one.</summary>
        /// <remarks>Remember to <see cref="Release(StringBuilder)"/> it when you are done.</remarks>
        public static StringBuilder AcquireStringBuilder()
        {
            var builder = ObjectPool<StringBuilder>.Acquire();
            return builder;
        }

        /// <summary>Sets the <see cref="StringBuilder.Length"/> = 0 and adds it to the list of spares so it can be reused.</summary>
        public static void Release(StringBuilder builder)
        {
            builder.Length = 0;
            ObjectPool<StringBuilder>.Release(builder);
        }

        /// <summary>[Animancer Extension] Calls <see cref="StringBuilder.ToString()"/> and <see cref="Release(StringBuilder)"/>.</summary>
        public static string ReleaseToString(this StringBuilder builder)
        {
            var result = builder.ToString();
            Release(builder);
            return result;
        }



        /// <summary>
        /// 手动释放对象池
        /// </summary>
        public static class Disposable
        {
            /************************************************************************************************************************/

            /// <summary>
            /// Creates a new <see cref="ObjectPool{T}.Disposable"/> and calls <see cref="ObjectPool{T}.Acquire"/> to set the
            /// <see cref="ObjectPool{T}.Disposable.Item"/> and `item`.
            /// </summary>
            public static ObjectPool<T>.Disposable Acquire<T>(out T item, Action<T> onRelease)
                where T : class, new()
                => new ObjectPool<T>.Disposable(out item, onRelease);

            /************************************************************************************************************************/

            /// <summary>
            /// Creates a new <see cref="ObjectPool{T}.Disposable"/> and calls <see cref="ObjectPool{T}.Acquire"/> to set the
            /// <see cref="ObjectPool{T}.Disposable.Item"/> and `item`.
            /// </summary>
            public static ObjectPool<List<T>>.Disposable AcquireList<T>(out List<T> list)
            {
                return new ObjectPool<List<T>>.Disposable(out list, onRelease: (l) => l.Clear());
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Creates a new <see cref="ObjectPool{T}.Disposable"/> and calls <see cref="ObjectPool{T}.Acquire"/> to set the
            /// <see cref="ObjectPool{T}.Disposable.Item"/> and `item`.
            /// </summary>
            public static ObjectPool<HashSet<T>>.Disposable AcquireSet<T>(out HashSet<T> set)
            {
                return new ObjectPool<HashSet<T>>.Disposable(out set, onRelease: (s) => s.Clear());
            }
            /************************************************************************************************************************/

        }
    }
    public static class ObjectPool<T> where T : class, new()
    {
        /************************************************************************************************************************/

        private static readonly List<T>
            Items = new List<T>();

        /************************************************************************************************************************/

        /// <summary>The <see cref="List{T}.Capacity"/> of the internal list of spare items.</summary>
        public static int Capacity
        {
            get => Items.Capacity;
            set
            {
                if (Items.Count > value)
                    Items.RemoveRange(value, Items.Count - value);
                Items.Capacity = value;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Increases the <see cref="Capacity"/> to equal the `capacity` if it was lower.</summary>
        public static void IncreaseCapacityTo(int capacity)
        {
            if (Capacity < capacity)
                Capacity = capacity;
        }

        /************************************************************************************************************************/

        /// <summary>Returns a spare item if there are any, or creates a new one.</summary>
        /// <remarks>Remember to <see cref="Release(T)"/> it when you are done.</remarks>
        public static T Acquire()
        {
            var count = Items.Count;
            if (count == 0)
            {
                return new T();
            }
            else
            {
                count--;
                var item = Items[count];
                Items.RemoveAt(count);

                return item;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Adds the `item` to the list of spares so it can be reused.</summary>
        public static void Release(T item)
        {
          
            Items.Add(item);

        }

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="IDisposable"/> to allow pooled objects to be acquired and released within <c>using</c>
        /// statements instead of needing to manually release everything.
        /// </summary>
        public readonly struct Disposable : IDisposable
        {
            /************************************************************************************************************************/

            /// <summary>The object acquired from the <see cref="ObjectPool{T}"/>.</summary>
            public readonly T Item;

            /// <summary>Called by <see cref="IDisposable.Dispose"/>.</summary>
            public readonly Action<T> OnRelease;

            /************************************************************************************************************************/

            /// <summary>
            /// Creates a new <see cref="Disposable"/> and calls <see cref="ObjectPool{T}.Acquire"/> to set the
            /// <see cref="Item"/> and `item`.
            /// </summary>
            public Disposable(out T item, Action<T> onRelease)
            {
                Item = item = Acquire();
                OnRelease = onRelease;
            }

            /************************************************************************************************************************/

            void IDisposable.Dispose()
            {
                OnRelease?.Invoke(Item);
                Release(Item);
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
    }
}
