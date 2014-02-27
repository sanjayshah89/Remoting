using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Common;
using DataLayer;


namespace Common
{
    public class RemoteObject : MarshalByRefObject, Common.IRemoteObject
    {
        public DataSet GetDepartment()
        {
            if (LoginInfo.Authenticated)
            {
                UserDataManager userManager = new UserDataManager();
                return userManager.GetDepartmentsDataSet();
            }
            return null;
        }


        

    }
}
