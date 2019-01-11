using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Modules.Objects.Company
{
    public class Employee
    {
        string userID;
        Position position;
        int salary;
        int wage;
        int wageEarned;
        public Employee(JObject obj)
        {

        }
        public JObject Deserialise()
        {
            return null;
        }
    }
}
