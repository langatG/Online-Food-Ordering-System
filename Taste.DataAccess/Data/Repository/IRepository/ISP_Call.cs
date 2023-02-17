using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taste.DataAccess.Data.Repository.IRepository
{
    public interface ISP_Call: IDisposable
    {
        IEnumerable<T> ReturnList<T>(string procedureName,DynamicParameters param=null);
        void ExecutewithoutReturn(string procedureName, DynamicParameters param = null);
        T ExecureReturnScaler<T>(string procedureName, DynamicParameters param = null);
    }
}
