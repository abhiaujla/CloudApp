using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

// ReSharper disable InconsistentNaming

namespace AT1.Implementation
{
	public class Solver
	{
		public Allocations   AT1Allocations;
		public Configuration AT1Configuration;

		public string Solve ( string url )
		{
			var filename =
				url.Substring ( url.LastIndexOf ( '/' ) + 1 ); // everything after the last "/" is the filename

			using ( var configurationWebClient = new WebClient ( ) )
			using ( var configurationStream = configurationWebClient.OpenRead ( url ) )
			using ( var configurationFile = new StreamReader ( configurationStream ) )
			{
				Configuration.TryParse ( configurationFile,
										 filename,
										 out AT1Configuration,
										 out var configurationErrors );
			}

			var cutoffTime = AT1Configuration.MaximumProgramDuration; // the max time a solution must take

			var bestAllocations = BestFirstSearch ( cutoffTime );
			AT1Allocations = new Allocations ( AT1Configuration ); // convert List<Allocation> into Allocations object
			AT1Allocations.AddRange ( bestAllocations );
			var taff = AT1Allocations.ToTaffString (
				AT1Configuration ); // need to save as taff and back to Allocations to make some functions work
			Allocations.TryParse ( taff, AT1Configuration, out AT1Allocations, out var errors2 );

			return taff;
		}

		// Greedy search
		private List<Allocation> BestFirstSearch ( double cutoffTime )
		{
			var initialAllocation = GetInitialAllocation ( ); // BFS search needs a node to start with
			var startNode         = new AllocationNode ( initialAllocation );
			var queue             = new PriorityQueue<AllocationNode> ( ); // priority queue of nodes to search next
			queue.Push ( startNode );

			var closed          = new HashSet<AllocationUid> ( ); // set of visited nodes
			var bestAllocations = new List<Allocation> ( );
			var bestEnergy      = double.PositiveInfinity;
			var bestTime        = initialAllocation.Time ( );
			var start           = DateTime.Now;

			while ( !queue.Empty ( ) )
			{
				var elapsed = DateTime.Now - start;
				if ( elapsed.TotalSeconds > 5 ) // change this timer if needed (in seconds)
					break;

				var current = queue.Pop ( );
				closed.Add ( current.Uid ); // mark node visited

				var successors = current.GetSuccessors ( )                          // get child nodes
										.Where ( x => !closed.Contains ( x.Uid ) ); // avoid visited nodes

				foreach ( var successor in successors )
				{
					closed.Add ( successor.Uid );
					if ( successor.Time > current.Time && // ignore nodes that was worse on both time and energy
						 successor.Energy > current.Energy )
						continue;

					bestTime = Math.Min ( bestTime, successor.Time );
					if ( successor.Time < cutoffTime ) // check if node should be in the solution
					{
						if ( successor.Energy < bestEnergy ) // check if node is better than existing solution
						{
							bestEnergy = successor.Energy;
							bestAllocations.Clear ( );
						}

						if ( Math.Abs ( successor.Energy - bestEnergy ) < 0.00001 )
							bestAllocations.Add ( successor.Allocation ); // add node to solution
					}

					queue.Push ( successor ); // queue the node to be visited later
				}
			}

			for ( var i = 0; i < bestAllocations.Count; i++ )
			{
				var allocation = bestAllocations[i];
				allocation.ID = i + 1; // fix allocation IDs of solution
			}

			closed.Clear ( );
			queue.Clear ( );

			return bestAllocations;
		}

		private Allocation GetInitialAllocation ( )
		{
			var initialAllocation = new Allocation (
				Array.Empty<string> ( ),
				1,
				AT1Configuration.NumberOfTasks,
				AT1Configuration.NumberOfProcessors,
				new List<string> ( )
			);
			initialAllocation.AssignTasks ( AT1Configuration );

			var tasks = initialAllocation.Tasks;
			var processors = initialAllocation.Processors
											  .OrderByDescending (
												  x => x
													  .Frequency ) // assign tasks to high frequency processors first to minimize time
											  .ToList ( );

			foreach ( var task in tasks )
			{
				var fastestProcessor = processors // find the fastest processor than can handle this task
									   .OrderBy ( x => x.Time ( ) )
									   .First ( x => task.RAM <= x.RAM );
				fastestProcessor.AllocatedTasks.Add ( task );
			}

			return initialAllocation;
		}
	}
}