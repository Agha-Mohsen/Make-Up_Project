using System.Collections.Generic;
using System.Linq;
using _0_Framework.Infrastructure;
using AccountManagement.Application.Contracts.Role;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServiceHost.Areas.Administration.Pages.Accounts.Role
{
    public class EditModel : PageModel
    {
        public EditRole Command;
        public List<SelectListItem> Permissions = new List<SelectListItem>();
        private readonly IRoleApplication _roleApplication;
        private readonly IEnumerable<IPermissionExposure> _exposure;

        public EditModel(IRoleApplication roleApplication, IEnumerable<IPermissionExposure> exposure)
        {
            _roleApplication = roleApplication;
            _exposure = exposure;
        }

        public void OnGet(long id)
        {
            Command = _roleApplication.GetDetails(id);
            var permissions = new List<PermissionDto>();

            foreach (var exposure in _exposure)
            {
                var exposedPermission = exposure.Expose();

                foreach (var (key, value) in exposedPermission)
                {
                    permissions.AddRange(value);
                    var group = new SelectListGroup { Name = key };

                    foreach (var permission in value)
                    {
                        var item = new SelectListItem(permission.Name, permission.Code.ToString())
                        {
                            Group = group
                        };

                        if (Command.MappedPermission.Any(x => x.Code == permission.Code))
                            item.Selected = true;

                        Permissions.Add(item);
                    }
                }
            }
        }

        public IActionResult OnPost(EditRole command)
        {
            _roleApplication.Edit(command);
            return RedirectToPage("Index");
        }
    }
}