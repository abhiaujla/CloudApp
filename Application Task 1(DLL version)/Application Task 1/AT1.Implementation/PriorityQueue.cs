using System;
using System.Collections.Generic;

namespace AT1.Implementation
{
	// Priority Queue implemented using a standard min heap
	public class PriorityQueue<T>
		where T : IComparable<T>
	{
		private readonly List<T> _list;

		public int Count => _list.Count;

		public PriorityQueue ( )
		{
			_list = new List<T> ( );
		}

		public void Push ( T x )
		{
			_list.Add ( x );
			var i = Count - 1;

			while ( i > 0 )
			{
				var p = ( i - 1 ) / 2;
				if ( _list[p].CompareTo ( x ) <= 0 )
					break;

				_list[i] = _list[p];
				i        = p;
			}

			if ( Count > 0 ) _list[i] = x;
		}

		public T Pop ( )
		{
			var target = Peek ( );
			var root   = _list[Count - 1];
			_list.RemoveAt ( Count - 1 );

			var i = 0;
			while ( i * 2 + 1 < Count )
			{
				var a = i * 2 + 1;
				var b = i * 2 + 2;
				var c = b < Count && _list[b].CompareTo ( _list[a] ) < 0 ? b : a;

				if ( _list[c].CompareTo ( root ) >= 0 )
					break;

				_list[i] = _list[c];
				i        = c;
			}

			if ( Count > 0 ) _list[i] = root;
			return target;
		}

		public T Peek ( )
		{
			if ( Count == 0 )
				throw new InvalidOperationException ( "Queue is empty." );

			return _list[0];
		}

		public bool Empty ( )
		{
			return _list.Count == 0;
		}

		public void Clear ( )
		{
			_list.Clear ( );
		}
	}
}