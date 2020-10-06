using AT1.Implementation;

namespace WcfServiceLibrary1
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in both code and config file together.
	public class Service : IService
	{
		public string Solve ( string url )
		{
			var solver = new Solver ( );
			return solver.Solve ( url );
		}
	}
}
