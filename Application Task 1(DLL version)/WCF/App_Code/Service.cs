using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.ServiceModel.Web;
using System.Net;
using AT1;
using AT1.Implementation;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public partial class Service : IService
{
    public Solver Solve(string url)
    {
        var solver = new Solver();
        solver.Solve(url);
        return solver;
    }
}
