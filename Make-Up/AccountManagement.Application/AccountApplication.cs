using System.Collections.Generic;
using _0_Framework.Application;
using AccountManagement.Application.Contracts.Account;
using AccountManagement.Domain.AccountAgg;

namespace AccountManagement.Application
{
    public class AccountApplication : IAccountApplication
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IFileUploader _fileUploader;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthHelper _authHelper;

        public AccountApplication(IAccountRepository accountRepository, IFileUploader fileUploader,
            IPasswordHasher passwordHasher, IAuthHelper authHelper)
        {
            _accountRepository = accountRepository;
            _fileUploader = fileUploader;
            _passwordHasher = passwordHasher;
            _authHelper = authHelper;
        }

        public OperationResult Create(CreateAccount command)
        {
            var operation = new OperationResult();

            if (_accountRepository.Exists(x => x.Username == command.Username || x.Mobile == command.Mobile))
                return operation.Failed(ApplicationMessages.DuplicatedRecord);

            var password = _passwordHasher.Hash(command.Password);

            var path = $"پنل های کاربری//{command.Username}";
            var pictureName = _fileUploader.Upload(command.ProfilePhoto, path);

            var account = new Account(command.Fullname, command.Username, password,
                command.Mobile, command.RoleId, pictureName);

            _accountRepository.Create(account);
            _accountRepository.SaveChanges();

            return operation.Succeeded();
        }

        public OperationResult Edit(EditAccount command)
        {
            var operation = new OperationResult();

            var account = _accountRepository.Get(command.Id);

            if (account == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);
            if (_accountRepository.Exists(x =>
                    (x.Username == command.Username || x.Mobile == command.Mobile) && x.Id != command.Id))
                return operation.Failed(ApplicationMessages.DuplicatedRecord);

            var path = $"پنل های کاربری//{command.Username}";
            var pictureName = _fileUploader.Upload(command.ProfilePhoto, path);

            account.Edit(command.Fullname, command.Username, command.Mobile, command.RoleId, pictureName);

            _accountRepository.SaveChanges();

            return operation.Succeeded();
        }

        public OperationResult ChangePassword(ChangePassword command)
        {
            var operation = new OperationResult();

            var account = _accountRepository.Get(command.Id);

            if (account == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);
            if (command.Password != command.RePassword)
                return operation.Failed(ApplicationMessages.PasswordNotMatch);

            var password = _passwordHasher.Hash(command.Password);

            account.ChangePassword(password);
            _accountRepository.SaveChanges();

            return operation.Succeeded();
        }

        public OperationResult Login(Login command)
        {
            var operation = new OperationResult();

            var account = _accountRepository.GetBy(command.Username);
            if (account == null)
                return operation.Failed(ApplicationMessages.WrongUserPass);

            (bool Verified, bool NeedsUpgrade) result = _passwordHasher.Check(account.Password, command.Password);

            if (!result.Verified)
                operation.Failed(ApplicationMessages.WrongUserPass);

            var authHelper = new AuthViewModel(account.Id, account.RoleId, account.Fullname, account.Username);
            _authHelper.Signin(authHelper);

            return operation.Succeeded();
        }

        public EditAccount GetDetails(long id)
        {
            return _accountRepository.GetDetails(id);
        }

        public List<AccountViewModel> Search(AccountSearchModel searchModel)
        {
            return _accountRepository.Search(searchModel);
        }

        public void Logout()
        {
            _authHelper.SignOut();
        }
    }
}