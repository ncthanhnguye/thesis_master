using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASTERPM.Model.Core
{
    public interface IDALContainer
    {
        IRepository Repository { get; }
        IUnitOfWork UnitOfWork { get; }
        void Close();
        void Dispose();
    }
}
