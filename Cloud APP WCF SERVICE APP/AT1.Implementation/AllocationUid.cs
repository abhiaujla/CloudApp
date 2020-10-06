using System;
using System.Linq;

namespace AT1.Implementation
{
	/*
	 * 1, 0, 0, 0	P1
	 * 0, 1, 1, 0	P2
	 * 0, 0, 0, 1	P3
	 *
	 * 1, 2, 2, 3	hash
	 * 3, 2, 2, 1	hash3
	 * 4, 2, 1, 3	hash
	 * 1, 2, 2, 3	hash2
	 */

	// A unique identifier for a node, used to check is a node has been visited
	public class AllocationUid : IEquatable<AllocationUid>
	{
		public int[] Tasks { get; }		// task array
		public int   Value { get; }		// hash

		public AllocationUid ( Allocation allocation )
		{
			Tasks = new int[allocation.Tasks.Count];

			foreach ( var processor in allocation.Processors )
			foreach ( var task in processor.AllocatedTasks )
				Tasks[task.ID - 1] = processor.ID;		// store all tasks in a single array

			Value = Tasks.Aggregate ( 397,				// compute hash of task array
									  ( current, item ) => current * 31 + item );
		}

		public bool Equals ( AllocationUid other )
		{
			if ( other is null ) return false;
			if ( ReferenceEquals ( this, other ) ) return true;

			return Value == other.Value &&
				   Tasks.SequenceEqual ( other.Tasks );	// 2 UIDs are equal iff they have the same sequence of tasks (order matters)
		}

		public override int GetHashCode ( )
		{
			return Value;
		}
	}
}