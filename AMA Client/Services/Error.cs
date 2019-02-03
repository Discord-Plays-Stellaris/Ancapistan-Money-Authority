using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMA_Client.Services
{
    public static class Error
    {
        public static void Throw(string description)
        {
            ErrorForm form = new ErrorForm(description);
            form.Show();
        }
    }
}
