using Newtonsoft.Json.Linq;

namespace VIR.Modules.Objects.Company
{
    public class Employee
    {
        public string userID;
        public Position position;
        public int salary;
        public int wage;
        public int wageEarned;
        public Employee()
        {
        }
        public Employee(JObject obj)
        {
            userID = (string) obj["id"];
            salary = (int) obj["salary"];
            wage = (int) obj["wage"];
            wageEarned = (int)obj["wageEarned"];
            position = new Position((JObject) obj["position"]);
        }
        public JObject Deserialise()
        {
            JObject obj = new JObject();
            obj["id"] = userID;
            obj["salary"] = salary;
            obj["wage"] = wage;
            obj["wageEarned"] = wageEarned;
            obj["position"] = position.Deserialise();
            return obj;
        }
    }
}
