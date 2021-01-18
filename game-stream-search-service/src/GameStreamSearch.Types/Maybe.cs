using System;
using System.Threading.Tasks;

namespace GameStreamSearch.Types
{
    public class Maybe<T>
    {
        internal bool HasItem { get; }
        internal T Item { get; }

        public Maybe()
        {
            HasItem = false;
        }

        public Maybe(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Item = item;
            HasItem = true;
        }

        public Maybe<TResult> Map<TResult>(Func<T, TResult> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (HasItem)
                return Maybe<TResult>.Just(selector(Item));
            else
                return Maybe<TResult>.Nothing();
        }

        public T GetOrElse(T fallbackValue)
        {
            if (fallbackValue == null)
                throw new ArgumentNullException(nameof(fallbackValue));

            if (HasItem)
                return Item;
            else
                return fallbackValue;
        }

        public T Unwrap()
        {
            if (!HasItem)
            {
                throw new InvalidOperationException();
            }

            return Item;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Maybe<T>;

            if (other == null)
                return false;

            return Equals(Item, other.Item);
        }

        public override int GetHashCode()
        {
            return HasItem ? Item.GetHashCode() : 0;
        }

        public bool IsNothing => !HasItem;
        public bool IsJust => HasItem;

        public static Maybe<T> ToMaybe(T? value)
        {
            return value != null ? Just(value) : Nothing();
        }

        public static Maybe<T> Just(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> Nothing()
        {
            return new Maybe<T>();
        }
    }
}
