using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using AMA_Client.Services;

namespace AMA_Client.Objects
{
    class LoggedInClient
    {
        public string id; // MAC adress of the device
        public string userID;

        public LoggedInClient(string _userID)
        {
            id = NetworkService.MAC;
            userID = _userID;
            db.SetJObjectAsync(db.SerializeObject<LoggedInClient>(this), "clients");
        }
    }
}
