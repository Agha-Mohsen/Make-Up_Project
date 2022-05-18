using System.Collections.Generic;
using _0_Framework.Application;

namespace AccountManagement.Application.Contracts.Account
{
    public interface IAccountApplication
    {
        void Logout();
        EditAccount GetDetails(long id);
        List<AccountViewModel> GetAccounts();
        OperationResult Login(Login command);
        OperationResult Edit(EditAccount command);
        OperationResult Register(RegisterAccount command);
        OperationResult ChangePassword(ChangePassword command);
        List<AccountViewModel> Search(AccountSearchModel searchModel);
    }
}