using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace RangeDict
{

    public class RangeDict<TKey, TValue> where TKey : IComparable<TKey>
    {
        private readonly SortedSet<Range<TKey>> _set = new SortedSet<Range<TKey>>(new RangeComparer<TKey>());
        private readonly Dictionary<Range<TKey>, TValue> _dictionary = new Dictionary<Range<TKey>, TValue>();

        //private Range<TKey> GetRange(TKey key)
        //{
        //    return _set.GetViewBetween(key, key).FirstOrDefault();
        //}

        private bool TryGetRange(Range<TKey> key, out Range<TKey> outRange)
        {
          
            var foundSet = _set.GetViewBetween(key, key);
            outRange= foundSet.LastOrDefault();
            return (foundSet.Count > 0);
        }

        public bool IsInRange(Range<TKey> key)
        {
            Range<TKey> outRange;
            return TryGetRange(key, out outRange);
        }

        public SortedSet<Range<TKey>> Foo(Range<TKey> key1, Range<TKey> key2)
        {
            return _set.GetViewBetween(key1, key2);
        }


        public TValue this[TKey min, TKey max]
        {
            get
            {
                return this[new Range<TKey>(min, max)];
            }
            set
            {
                 this[new Range<TKey>(min, max)] = value;
            }
        }

        public TValue this[Range<TKey> key]
        {
             get
             {
                 Range<TKey> outRange;
                 if (TryGetRange(key, out outRange))
                 {
                     return _dictionary[outRange];
                 }
                 throw new KeyNotFoundException();
             }
            set
            {
                Range<TKey> outRange;
                if (!TryGetRange(key, out outRange))
                {
                    _set.Add(key);
                    outRange = key;
                }
                _dictionary[outRange] = value;
              
            }
        }
    }

    public class RangeComparer<T> : IComparer<Range<T>> where T : IComparable<T>
    {
        public int Compare(Range<T> x, Range<T> y)
        {
            return x.CompareTo(y);
        }
    }

    public class Range<T> : IComparable<T>, IComparable<Range<T>> where T : IComparable<T>
    {
        public Range(T comparable)
            : this(comparable, comparable)
        {
        }

        public Range(T minimum, T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
        
        protected bool Equals(T other)
        {
            return Equals(new Range<T>(other));
        }

        protected bool Equals(Range<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Minimum, other.Minimum) && EqualityComparer<T>.Default.Equals(Maximum, other.Maximum);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((Range<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Minimum)*397) ^ EqualityComparer<T>.Default.GetHashCode(Maximum);
            }
        }

        /// <summary>
        /// Minimum value of the range
        /// </summary>
        public T Minimum { get; set; }

        /// <summary>
        /// Maximum value of the range
        /// </summary>
        public T Maximum { get; set; }

        public int CompareTo(T other)
        {
            if (Minimum.CompareTo(other) < 0) return -1;
            if (Maximum.CompareTo(other) > 1) return 1;
            return 0;
        }

     
        public int CompareTo(Range<T> other)
        {
            if (Minimum.CompareTo(other.Minimum) < 0 && Maximum.CompareTo(other.Maximum) <0) return -1;
            if (Minimum.CompareTo(other.Minimum)> 0 && Maximum.CompareTo(other.Maximum) > 0) return 1;
            return 0; //intersects
        }

        /// <summary>
        /// Presents the Range in readable format
        /// </summary>
        /// <returns>String representation of the Range</returns>
        public override string ToString() { return String.Format("[{0} - {1}]", Minimum, Maximum); }

        /// <summary>
        /// Determines if the range is valid
        /// </summary>
        /// <returns>True if range is valid, else false</returns>
        public Boolean IsValid() { return Minimum.CompareTo(Maximum) <= 0; }

        public bool Contains(Range<T> other)
        {
            return Minimum.CompareTo(other.Minimum) <=0  && Maximum.CompareTo(other.Maximum) >=0;
        }

        //  User-defined conversion from double to Digit 
        public static implicit operator Range<T>(T num)
        {
            return new Range<T>(num);
        } 
    }



    class Program
    {
        static void Main(string[] args)
        {
           

            RangeDict<int,string> rangeDict = new RangeDict<int, string>();
            rangeDict[1, 40] = "Fail";
            rangeDict[41, 60] = "E";
            rangeDict[61, 70] = "D";
            rangeDict[71, 79] = "C";

           rangeDict[81, 90] = "B";
            rangeDict[91, 99] = "A";
            rangeDict[100] = "A+";



            Console.WriteLine(rangeDict[91]);
        /*    Console.WriteLine(rangeDict[3]);
            Console.WriteLine(rangeDict[10]);
            Console.WriteLine(rangeDict[55]);
            Console.WriteLine(rangeDict[62]);
            Console.WriteLine(rangeDict[72]);
            Console.WriteLine(rangeDict[99]);
            Console.WriteLine(rangeDict[100]);*/
            Console.ReadKey();


        }
    }
}
