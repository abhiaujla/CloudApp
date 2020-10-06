using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT1.Implementation
{
	public static class Extensions
	{
		// Computes total time used by this processor for its tasks
		public static double Time ( this Processor processor )
		{
			return processor.AllocatedTasks.Sum ( x => x.ElapsedTime ( processor ) );
		}

		// Extension method to find max element in a sequence by a specific property
		public static TSource MaxBy<TSource, TKey> ( this IEnumerable<TSource> source,
													 Func<TSource, TKey> selector )
		{
			var comparer = Comparer<TKey>.Default;

			using ( var sourceIterator = source.GetEnumerator ( ) )
			{
				if ( !sourceIterator.MoveNext ( ) )
					throw new InvalidOperationException ( "Sequence contains no elements" );

				var min    = sourceIterator.Current;
				var minKey = selector ( min );
				while ( sourceIterator.MoveNext ( ) )
				{
					var candidate          = sourceIterator.Current;
					var candidateProjected = selector ( candidate );
					if ( comparer.Compare ( candidateProjected, minKey ) > 0 )
					{
						min    = candidate;
						minKey = candidateProjected;
					}
				}

				return min;
			}
		}

		// Convert allocation to TAFF string
		public static string ToTaffString ( this Allocation allocation )
		{
			var builder = new StringBuilder ( );
			builder.AppendLine ( $"ALLOCATION-ID={allocation.ID}" );

			foreach ( var processor in allocation.Processors )
			{
				var tasks = new int[allocation.NumberOfTasks];
				foreach ( var task in processor.AllocatedTasks )
					tasks[task.ID - 1] = 1;
				builder.AppendLine ( string.Join ( ",", tasks ) );
			}

			return builder.ToString ( );
		}

		// Convert allocations to TAFF string
		public static string ToTaffString ( this Allocations allocations, Configuration config )
		{
			var builder = new StringBuilder ( );
			builder.AppendLine ( $"CONFIG-FILE=\"{config.FilePath}\"" );
			builder.AppendLine ( );
			builder.AppendLine (
				$"ALLOCATIONS-DATA={allocations.Count},{config.NumberOfTasks},{config.NumberOfProcessors}" );
			builder.AppendLine ( );

			foreach ( var allocation in allocations )
			{
				builder.AppendLine ( allocation.ToTaffString ( ) );
				builder.AppendLine ( );
			}

			return builder.ToString ( );
		}
	}
}