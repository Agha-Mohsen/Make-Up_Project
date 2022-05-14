using System.Collections.Generic;

namespace _0_Framework.Application
{
    public interface IAuthHelper
    {
        void SignOut();
        void Signin(AuthViewModel account);
        bool IsAuthenticated();
        string CurrentAccountRole();
        AuthViewModel CurrentAccountInfo();
        List<int> GetPermissions(); 
    }
}
