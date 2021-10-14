using System;
using System.Collections;
using System.Collections.Generic;

namespace AARPG.API.DataStructures{
	/// <summary>
	/// An object representing a heap of objects
	/// </summary>
	public class Heap<T> : IDisposable, IEnumerable, IEnumerable<T>{
		public struct HeapIndex{
			public readonly int index;
			public readonly int length;

			internal HeapIndex(int index, int length){
				this.index = index;
				this.length = length;
			}
		}

		private T[] heap;
		//Having "free entry" be "false" and "used entry" be "true" simplifies the logic when growing the heap, since BitArray defaults new bits to "false"
		//BitArray also handles determining how many bits are needed for a given capacity
		private BitArray used;
		private int version;

		public Heap(){
			heap = Array.Empty<T>();
			used = new BitArray(0);
			version = 0;
		}

		public Heap(int capacity){
			heap = new T[capacity];
			used = new BitArray(capacity, false);
			version = 0;
		}

		public Heap(T[] entries){
			heap = (T[])entries.Clone();
			used = new BitArray(entries.Length, true);
			version = 0;
			Count = entries.Length;
		}

		public ref T this[int index]{
			get{
				if(index < 0 || index > heap.Length)
					throw new ArgumentOutOfRangeException(nameof(index), "Index was outside the range of the heap");

				if(!used[index])
					throw new ArgumentException($"Heap index {index} was freed and cannot be used");

				return ref heap[index];
			}
		}

		/// <summary>
		/// How many used entries the heap contains
		/// </summary>
		public int Count{ get; private set; }

		/// <summary>
		/// Inserts <paramref name="entries"/> into the heap and expands the heap if there isn't enough space
		/// </summary>
		/// <returns>The starting index of <paramref name="entries"/> in the heap, or <c>-1</c> if the array was empty</returns>
		public HeapIndex AllocateEntries(T[] entries){
			if(entries is null)
				throw new ArgumentNullException(nameof(entries));

			if(entries.Length == 0)
				return new HeapIndex(-1, 0);

			Count += entries.Length;

			if(FindFreeRange(entries.Length, out int start)){
				//Insert the entries at the given index
				InsertEntries(entries, start);
				return new HeapIndex(start, entries.Length);
			}

			//Couldn't find a free range.  Need to allocate more
			int oldLength = heap.Length;
			EnsureCapacity(heap.Length + entries.Length);
			InsertEntries(entries, oldLength);
			return new HeapIndex(start, entries.Length);
		}

		/// <summary>
		/// Gets a slice of entries within the heap
		/// </summary>
		/// <param name="index">The starting index and amount of the entries to retrieve</param>
		/// <remarks>This method can return entries which are considered "freed"</remarks>
		public T[] GetEntries(HeapIndex index){
			EnsureWithinRange(index.index, index.length);

			if(index.length == 0)
				return Array.Empty<T>();

			return heap[index.index..(index.index + index.length)];
		}

		public void FreeEntries(HeapIndex index){
			EnsureWithinRange(index.index, index.length);

			version++;

			if(index.length == 0)
				return;

			Array.Fill(heap, default, index.index, index.length);
			UpdateUsed(index.index, index.length, set: false);

			Count -= index.length;
		}

		private bool FindFreeRange(int length, out int start){
			start = -1;
			int range = 0;
			int iter = 0;
			foreach(bool bit in used){
				if(!bit){
					range++;

					if(start == -1)
						start = iter;
				}else{
					//Couldn't find enough space; reset the counter
					range = 0;
					start = -1;
				}

				//Found enough space
				if(range == length)
					return true;

				iter++;

				//"used" could contain more bits than there are entries
				if(iter >= heap.Length)
					break;
			}

			//Couldn't find a large enough range
			start = -1;
			return false;
		}

		private void EnsureCapacity(int capacity){
			if(capacity > heap.Length){
				Array.Resize(ref heap, capacity);
				used.Length = capacity;
				version++;
			}
		}

		private void EnsureWithinRange(int start, int length){
			if(start < 0 || start + length > heap.Length)
				throw new IndexOutOfRangeException("Given start and length values went outside the range of the heap");

			if(length < 0)
				throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive");
		}

		private void InsertEntries(T[] newentries, int start){
			Array.Copy(newentries, 0, heap, start, newentries.Length);
			UpdateUsed(start, newentries.Length, set: true);
			version++;
		}

		private const int BitsPerInt8 = 8;

		private void UpdateUsed(int start, int length, bool set){
			EnsureWithinRange(start, length);

			if(length == 0)
				return;

			var mask = new BitArray(CreateArrayForBits(start, length));

			if(set)
				used.Or(mask);
			else
				used.And(mask.Not());
		}

		private byte[] CreateArrayForBits(int start, int length){
			byte[] arr = new byte[GetArrayLength(used.Length, BitsPerInt8)];

			for(int i = start; i < start + length; i++)
				arr[i / BitsPerInt8] |= (byte)(1 << (i % BitsPerInt8));

			return arr;
		}

		private static int GetArrayLength(int length, int div)
			=> length > 0 ? ((length - 1) / div) + 1 : 0;

		#region Implement IDisposable
		private bool disposed;

		public void Dispose(){
			InnerDispose();
			GC.SuppressFinalize(this);
		}

		private void InnerDispose(){
			if(!disposed){
				disposed = true;

				heap = null;
				used = null;

				version = -1;
			}
		}

		~Heap() => InnerDispose();
		#endregion

		#region Implement IEnumerable
		public IEnumerator GetEnumerator()
			=> new HeapEnumerator(this);

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
			=> new HeapEnumerator(this);
		#endregion

		/// <summary>
		/// An enumerator for <seealso cref="Heap{T}"/>.  Any freed entries in the heap are skipped over
		/// </summary>
		public class HeapEnumerator : IEnumerator, IEnumerator<T>{
			private Heap<T> heap;
			private int index;
			private readonly int version;
			private T current;

			internal HeapEnumerator(Heap<T> heap){
				this.heap = heap;
				index = -1;
				version = heap.version;
			}

			public int CurrentIndex => index;

			public object Current{
				get{
					if(version != heap.version)
						throw new InvalidOperationException("Underlying collection has been modified");

					if(index == -1)
						throw new InvalidOperationException("Enumerator has not started");

					if(index >= heap.heap.Length)
						throw new InvalidOperationException("Enumerator has ended");

					return current;
				}
			}

			T IEnumerator<T>.Current{
				get{
					if(version != heap.version)
						throw new InvalidOperationException("Underlying collection has been modified");

					if(index == -1)
						throw new InvalidOperationException("Enumerator has not started");

					if(index >= heap.heap.Length)
						throw new InvalidOperationException("Enumerator has ended");

					return current;
				}
			}

			public bool MoveNext(){
				if(version != heap.version)
					throw new InvalidOperationException("Underlying collection has been modified");

				if(index < heap.heap.Length - 1){
					//Find the next non-free vertex
					while(++index < heap.heap.Length && !heap.used[index]);

					if(index < heap.heap.Length){
						current = heap.heap[index];
						return true;
					}

					current = default;
				}

				//Heap was empty or the enumerator has reached the end
				index = heap.heap.Length;
				return false;
			}

			public void Reset(){
				if(version != heap.version)
					throw new InvalidOperationException("Underlying collection has been modified");

				index = -1;
			}

			#region Implement IDisposable
			private bool disposed;

			public void Dispose(){
				InnerDispose();
				GC.SuppressFinalize(this);
			}

			private void InnerDispose(){
				if(!disposed){
					disposed = true;

					heap = null;
				}
			}

			~HeapEnumerator() => InnerDispose();
			#endregion
		}
	}
}
