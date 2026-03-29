using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Plugins;
using Org.BouncyCastle.Ocsp;
using QrCode.Application.Features.Exception.CreateException;
using QrCode.Application.Features.Tags.IsTagVerification;
using QrCode.Application.Features.Tags.UpdateTag;
using QrCode.Application.Features.Tags.VerificationCode;
using QrCode.Application.Features.Users.CreateUser;
using QrCode.Application.Models;
using QrCode.Domain.AggregateModels.Email.Events;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.Validator;
using QrCode.WebUi.Models;
using System.Security.Claims;
using static QRCoder.PayloadGenerator;

namespace QrCode.WebUi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IMediator mediator, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITempDataDictionaryFactory tempDataDictionaryFactory, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _signInManager = signInManager;
            _userManager = userManager;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse), new { returnUrl })
            };
            return Challenge(properties, "Google");
        }

        // Google'dan gelen cevabı işleme
        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
            {
                TempData["Error"] = "User email not found.";
            }

            // Google'dan gelen kullanıcı bilgilerini al
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);
            var userName = email;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Kullanıcı veritabanında bulunamazsa, yeni bir kullanıcı oluştur
                user = new ApplicationUser { UserName = userName, Name = name, Email = email,EmailConfirmed=true };
               IdentityResult userResult= await _userManager.CreateAsync(user);
            }

            // Kullanıcıyı oturum açtır
            await _signInManager.SignInAsync(user, isPersistent: false);
            var tag = TempData["Tag"];
            if(tag!=null)
            {
                var VerifyUser = await _userManager.FindByEmailAsync(email);
                await _mediator.Send(new UpdateVerificationCommand { Tag = tag.ToString(), UserId = VerifyUser.Id });
                return RedirectToAction("UpdateTagDetails", "Tag", new { tag = tag.ToString() });
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider="Facebook", string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                // External login failed
                return RedirectToAction("Login");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // External login information not available
                return RedirectToAction("Login");
            }

            var user = new ApplicationUser { UserName = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier), Name = info.Principal.FindFirstValue(ClaimTypes.Name), Email = info.Principal.FindFirstValue(ClaimTypes.Email),EmailConfirmed=true };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var tag = TempData["Tag"];
                    if (tag != null)
                    {
                        var VerifyUser = await _userManager.FindByEmailAsync(user.Email);
                        await _mediator.Send(new UpdateVerificationCommand { Tag = tag.ToString(), UserId = VerifyUser.Id });
                        return RedirectToAction("UpdateTagDetails", "Tag", new { tag = tag.ToString() });
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            // If we reached here, something went wrong
            return RedirectToAction("Login");
        }

        
        [AllowAnonymous]
        public async Task<IActionResult> Login(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            var isUser = await _userManager.GetUserAsync(User);
            if (isUser != null && TempData["VerificationCode"]==null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest.TagVerification != null)
                {
                    var verificationCode = await _mediator.Send(new GetVerificationCode(loginRequest.TagVerification.Tag.ToString()));
                    if (verificationCode == loginRequest.TagVerification.IncomingVerificationCode)
                    {
                        var VerifyUser = await _userManager.GetUserAsync(User);
                        await _mediator.Send(new UpdateVerificationCommand { Tag = loginRequest.TagVerification.Tag, UserId = VerifyUser.Id });
                        return RedirectToAction("UpdateTagDetails", "Tag", new { tag = loginRequest.TagVerification.Tag });
                    }
                    TempData["Tag"] = loginRequest.TagVerification.Tag;
                    TempData["VerificationCode"] = verificationCode;
                    TempData["Error"] = "WrongCode";
                    return RedirectToAction("Login");
                }
                var validator = new LoginRequestValidator();
                var validationResult = validator.Validate(loginRequest);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                    return View(loginRequest);
                }
                var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    TempData["Error"] = "User email not found.";
                }
                else
                {
                    var emailConfirm = await _userManager.IsEmailConfirmedAsync(user);
                    if (emailConfirm)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, true, true);
                        if (result.Succeeded)
                        {

                            //Tag için kod gönder
                            var tag = TempData["Tag"];
                            if (tag != null)
                            {
                                var isTagVerification = await _mediator.Send(new IsTagVerificationCommand(tag.ToString()));
                                if (!isTagVerification)
                                {
                                    var verificationCode = await _mediator.Send(new GetVerificationCode(tag.ToString()));
                                    TempData["VerificationCode"] = verificationCode;
                                    string body = $"Confirmation code for tag number #{tag}<br><br>Verification Code:<b style='font-size:16px'>{verificationCode}</b>";
                                    await _mediator.Publish(new EmailSentEvent(user.Email, $"Tag - #{tag} verification code", body));
                                    return RedirectToAction("Login");
                                }
                            }
                            var userRoles = await _userManager.GetRolesAsync(user);

                            // Her bir rolü kullanıcıya ekle
                            foreach (var role in userRoles)
                            {
                                if (!await _userManager.IsInRoleAsync(user, role))
                                {
                                    await _userManager.AddToRoleAsync(user, role);
                                }
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Error"] = "Email or password is incorrect.";
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Please complete the confirmation process sent to your e-mail.";
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateExceptionCommand("Account/Login", ex.StackTrace, ex.Message));
                throw new Exception();
            }
        }
        
        public async Task<IActionResult> Create(string tag)
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(HttpContext);
            tempData.Clear();
            if (!string.IsNullOrEmpty(tag))
            {
                TempData["Tag"] = tag;
                var isTagVerification = await _mediator.Send(new IsTagVerificationCommand(tag));
                if(User.Identity.IsAuthenticated && !isTagVerification)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var verificationCode = await _mediator.Send(new GetVerificationCode(tag.ToString()));
                    TempData["VerificationCode"] = verificationCode;
                    string body = $"Tag number #{tag}<br><br>Verification Code:<b style='font-size:16px'>{verificationCode}</b>";
                    await _mediator.Publish(new EmailSentEvent(user.Email, $"Tag - #{tag} verification code", body));
                    return RedirectToAction("Login");
                   
                }
                if (isTagVerification)
                {
                    return RedirectToAction("GetTagDetails", "Tag", new { tag = tag });
                }
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserCommand request, CancellationToken cancellationToken)
        {
            
            try
            {
                var validator = new UserCreateValidator();
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                    return View("Create", request);
                }
                request.UserName = request.Email;
                var result = await _mediator.Send(request, cancellationToken);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    ViewBag.DublicateUser = "You have entered a registered e-mail address";
                    return View("Create", request);
                }
                var user = await _userManager.FindByEmailAsync(request.Email);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    email = request.Email,
                    token = token
                });
                string body = $"Please confirm your email address by clicking this link: <a href='https://www.torruna.store{url}' style='background-color: #3c8dbc; border: none;color: white; padding: 10px 41px;text-align: center;text-decoration: none;display: inline-block; font-size: 16px;margin: 4px 2px;cursor: pointer;'>Confirm Email</a>";
                await _mediator.Publish(new EmailSentEvent(user.Email, "Tag - Mail register activation", body));
                var message = new { message = user.Email };
                return RedirectToAction("Login", "Account", message);
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateExceptionCommand("Account/Create", ex.StackTrace, ex.Message));
                throw new Exception();
            }
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(HttpContext);
            tempData.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // E-posta adresiyle kullanıcıyı bul
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        // Kullanıcı bulunamazsa hata mesajı göster
                        ModelState.AddModelError(string.Empty, "No account was found with this email address.");
                        return View(model);
                    }

                    // Kullanıcıya şifre sıfırlama bağlantısı gönder
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);
                    // Burada resetLink değişkenini kullanarak kullanıcıya e-posta gönderme işlemi yapılabilir
                    string body = $"Please click on the link to change your password : <a href='{resetLink}' style='background-color: #3c8dbc; border: none;color: white; padding: 10px 41px;text-align: center;text-decoration: none;display: inline-block; font-size: 16px;margin: 4px 2px;cursor: pointer;'>Forgot Password</a>";
                    await _mediator.Publish(new EmailSentEvent(user.Email, "Tag - Forgot Password", body));
                    // Başarılı mesajı göster
                    ViewBag.SuccessMessage = "The password reset link has been sent to your email address.";
                }

                // Eğer işlem başarısız olursa formu tekrar göster
                return View(model);
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateExceptionCommand("Account/ForgotPassword", ex.StackTrace, ex.Message));
                throw new Exception();
            }
        }
        // Şifre sıfırlama formunu gösterir
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordModel { Email = email, Token = token };
            return View(model);
        }

        // Şifre sıfırlama formunu işler
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // E-posta adresiyle kullanıcıyı bul
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        // Kullanıcı bulunamazsa hata mesajı göster
                        ModelState.AddModelError(string.Empty, "No account was found with this email address.");
                        return View(model);
                    }

                    // Şifreyi sıfırla
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        // Başarılı mesajı göster
                        ViewBag.SuccessMessage = "Your password has been successfully reset.";
                        return View();
                    }

                    // Şifre sıfırlama başarısızsa hata mesajları göster
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                // Eğer işlem başarısız olursa formu tekrar göster
                return View(model);
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateExceptionCommand("Account/ResetPassword", ex.StackTrace, ex.Message));
                throw new Exception();
            }
        }
    }
}
