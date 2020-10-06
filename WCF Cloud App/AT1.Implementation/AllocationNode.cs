using System;
using System.Collections.Generic;
using System.Linq;

namespace AT1.Implementation
{
	// Node representing a specific allocation
	public class AllocationNode : IComparable<AllocationNode>
	{
		public Allocation    Allocation { get; }
		public double        Energy     { get; }	// Total energy for this allocation
		public double        Time       { get; }	// Total time for this allocation
		public AllocationUid Uid        { get; }

		public AllocationNode ( Allocation allocation )
		{
			Allocation = allocation;
			Energy     = allocation.Energy ( );
			Time       = allocation.Time ( );
			Uid        = new AllocationUid ( allocation );
		}

		// Returns all valid child nodes
		// A valid child node is an allocation where 1 task has been moved from the slowest processor into another one
		public IEnumerable<AllocationNode> GetSuccessors ( )
		{
			var tasks = Allocation.Tasks;
			var processors = Allocation.Processors		// create a copy of the list because we don't want to modify the original list
									   .Select ( x => new Processor ( x ) )
									   .ToList ( );

			var slowestProcessor = processors.MaxBy ( x => x.Time ( ) );

			foreach ( var processor in processors )
			{
				if ( processor.ID == slowestProcessor.ID )	// skip over the slowest processor, can't move a task from slowest back to slowest
					continue;

				var validTasks = slowestProcessor.AllocatedTasks	// find all the tasks that this processor can run
											   .Where ( x => x.RAM <= processor.RAM )
											   .ToList ( );
				foreach ( var task in validTasks )
				{
					// transfer task from slowest processor into this one
					slowestProcessor.AllocatedTasks.Remove ( task );
					processor.AllocatedTasks.Add ( task );

					// new allocation after the task transfer
					var allocation = new Allocation ( Allocation.OriginalItem,
													  Allocation.ID + 1,
													  Allocation.NumberOfTasks,
													  Allocation.NumberOfProcessors,
													  Allocation.ProcessorAllocations );
					allocation.Tasks = tasks;
					allocation.Processors = processors
											.Select ( x => new Processor ( x ) )
											.ToList ( );
					// undo the task transfer so we can repeat this for the next task/processor
					slowestProcessor.AllocatedTasks.Add ( task );
					processor.AllocatedTasks.Remove ( task );

					var successor = new AllocationNode ( allocation );

					yield return successor;		// return the child node
				}
			}
		}

		public int CompareTo ( AllocationNode other )
		{
			if ( ReferenceEquals ( this, other ) ) return 0;
			if ( other is null ) return 1;
			// compares nodes first by time and then by energy
			var timeComparison = Time.CompareTo ( other.Time );
			if ( timeComparison != 0 ) return timeComparison;
			return Energy.CompareTo ( other.Energy );
		}
	}
}