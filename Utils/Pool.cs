﻿using System;
using System.Reflection;
using System.Diagnostics;
using System.Reflection.Emit;

namespace GlyphEngine.Utils
{
	/// <summary>
	/// A collection that maintains a set of class instances to allow for recycling
	/// instances and minimizing the effects of garbage collection.
	/// </summary>
	/// <typeparam name="T">The type of object to store in the Pool. Pools can only hold class types.</typeparam>
	public class Pool<T> where T : class
    {
        #region Members

        // the amount to enlarge the items array if New is called and there are no free items
		private const int ResizeAmount = 20;

		// the actual items of the pool
		private T[] _items;

		// used for checking if a given object is still valid
		private readonly Predicate<T> _validate;

		// used for allocating instances of the object
		private readonly CreatorFunc _constructor;

        #endregion Members

        #region Properties

        /// <summary>
		/// Gets or sets a delegate used for initializing objects before returning them from the New method.
		/// </summary>
		public Action<T> Initialize { get; set; }

		/// <summary>
		/// Gets or sets a delegate that is run when an object is moved from being valid to invalid
		/// in the CleanUp method.
		/// </summary>
		public Action<T> Deinitialize { get; set; }

        /// <summary>
        /// A delegate that returns a new object instance for the Pool.
        /// </summary>
        /// <returns>A new object instance.</returns>
        public delegate T CreatorFunc();

		/// <summary>
		/// Gets the number of valid objects in the pool.
		/// </summary>
		public int ValidCount { get { return _items.Length - InvalidCount; } }

		/// <summary>
		/// Gets the number of invalid objects in the pool.
		/// </summary>
		public int InvalidCount { get; private set; }

		/// <summary>
		/// Returns a valid object at the given index. The index must fall in the range of [0, ValidCount].
		/// </summary>
		/// <param name="index">The index of the valid object to get</param>
		/// <returns>A valid object found at the index</returns>
		public T this[int index]
		{
			get
			{
				index += InvalidCount;

				if (index < InvalidCount || index >= _items.Length)
					throw new IndexOutOfRangeException("The index must be less than or equal to ValidCount");

				return _items[index];
			}
		}

        #endregion Properties

        #region cTors

        /// <summary>
		/// Creates a new pool.
		/// </summary>
		/// <param name="validateFunc">A predicate used to determine if a given object is still valid.</param>
		public Pool(Predicate<T> validateFunc) : this(0, validateFunc) { }

		/// <summary>
		/// Creates a new pool with a specific starting size.
		/// </summary>
		/// <param name="initialSize">The initial size of the pool.</param>
		/// <param name="validateFunc">A predicate used to determine if a given object is still valid.</param>
		public Pool(int initialSize, Predicate<T> validateFunc) : this(initialSize, validateFunc, null) { }

		/// <summary>
		/// Creates a new pool with a specific starting size.
		/// </summary>
		/// <param name="initialSize">The initial size of the pool.</param>
		/// <param name="validateFunc">A predicate used to determine if a given object is still valid.</param>
		/// <param name="allocateFunc">A function used to allocate an instance for the pool.</param>
		public Pool(int initialSize, Predicate<T> validateFunc, CreatorFunc allocateFunc)
		{
			// validate some parameters
			if (initialSize < 0)
				throw new ArgumentException("initialSize must be non-negative");
			if (validateFunc == null)
				throw new ArgumentNullException("validateFunc");

			if (initialSize == 0)
				initialSize = 10;

			_items = new T[initialSize];
			_validate = validateFunc;
			this.InvalidCount = _items.Length;

			// default to using a parameterless constructor if no allocateFunc was given
			_constructor = allocateFunc;

			// if we are using the ConstructorAllocate method, make sure we have a valid parameterless constructor
			if (null == _constructor)
			{
				// we want to find any parameterless constructor, public or not
				var constructor = typeof(T).GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
					null, 
					new Type[] { },
					null);

				if (constructor == null)
					throw new InvalidOperationException(typeof(T) + " does not have a parameterless constructor.");

                var ot = typeof(T);
                DynamicMethod dm = new DynamicMethod(ot.Name + "Ctor", ot, new Type[0], ot.Module, true);
                ILGenerator lgen = dm.GetILGenerator();
                lgen.Emit(OpCodes.Newobj, constructor);
                lgen.Emit(OpCodes.Ret);
                _constructor = (CreatorFunc)dm.CreateDelegate(typeof(CreatorFunc));
			}
		}

        #endregion cTors

        #region Methods

        /// <summary>
		/// Cleans up the pool by checking each valid object to ensure it is still actually valid.
		/// </summary>
		public void CleanUp()
		{
			for (int i = InvalidCount; i < _items.Length; i++)
			{
				T obj = _items[i];

				// if it's still valid, keep going
				if (_validate(obj)) 
					continue;

				// otherwise if we're not at the start of the invalid objects, we have to move
				// the object to the invalid object section of the array
				if (i != InvalidCount)
				{
					_items[i] = _items[InvalidCount];
					_items[InvalidCount] = obj;
				}

				// clean the object if desired
				if (Deinitialize != null)
					Deinitialize(obj);

				InvalidCount++;
			}
		}

		/// <summary>
		/// Returns a new object from the Pool.
		/// </summary>
		/// <returns>The next object in the pool if available, null if all instances are valid.</returns>
		public T New()
		{
            if (0 == InvalidCount)
                this.CleanUp();

			// if we're out of invalid instances, resize to fit some more
			if (InvalidCount == 0)
			{
#if DEBUG
                Trace.WriteLine("Resizing pool. Old size: " + _items.Length + ". New size: " + (_items.Length + ResizeAmount));
#endif
				// create a new array with some more slots and copy over the existing items
				T[] newItems = new T[_items.Length + ResizeAmount];
				for (int i = _items.Length - 1; i != -1; i--)
					newItems[i + ResizeAmount] = _items[i];
				_items = newItems;

				// move the invalid count based on our resize amount
				InvalidCount += ResizeAmount;
			}

			// decrement the counter
			InvalidCount--;

			// get the next item in the list
			T obj = _items[InvalidCount];

			// if the item is null, we need to allocate a new instance
            if (null == obj)
            {
                obj = _constructor();

                if (null == obj)
                    throw new InvalidOperationException("The pool's allocate method returned a null object reference.");

                _items[InvalidCount] = obj;
            }

			// initialize the object if a delegate was provided
			if (null != Initialize)
				Initialize(obj);

			return obj;
		}

        #endregion Methods

	}
}