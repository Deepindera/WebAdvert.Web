using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;

        }
        public  IActionResult Signup()
        {
            var model = new SignUpModel();
            return View(model);
        }

        public IActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    ModelState.AddModelError("Invalid Login", "We cannot login with that username and password");

                }

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);

                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email alreadt exists");

                }
                else
                {
                    user.Attributes.Add("name", model.Email);
                    var createdUser = await _userManager.CreateAsync(user, model.Password);

                    if (createdUser.Succeeded)
                    {
                        return RedirectToAction("Confirm");
                    }
                    else
                    {
                        foreach (var error in createdUser.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                    }
                }
            }

            return View(model);
        }


        public  IActionResult Confirm()
        {
            var model = new ConfirmModel();
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

                if (user == null || user.Status == null)
                {
                    ModelState.AddModelError("UserNotExists", "Invalid Confirmation");
                }
                else
                {

                    var confirmResult = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);

                    if (confirmResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in confirmResult.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                    }
                }

            }

            return View(model);
        }
    }
}