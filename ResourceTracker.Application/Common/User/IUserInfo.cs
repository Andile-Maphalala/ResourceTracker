using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Common.User
{
    public interface IUserInfo
    {
        int GetUserId();

        bool IsAdmin();
        bool IsLoginAsActive();
    }
}
