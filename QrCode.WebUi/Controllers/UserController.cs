using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using QrCode.Application.Features.Tags.IsTagVerification;
using QrCode.Application.Features.Tags.UpdateTag;
using QrCode.Application.Features.Tags.VerificationCode;
using QrCode.Application.Features.Users.CreateUser;
using QrCode.Application.Models;
using QrCode.Domain.AggregateModels.Email.Events;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.Validator;
using System.Text.Encodings.Web;

namespace QrCode.WebUi.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IMediator mediator, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> GetUserList(string searchString, int page = 1, int pageSize = 10)
        {
            var users = await _userManager.Users.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Name.ToLower().Contains(searchString.ToLower()) || u.Email.ToLower().Contains(searchString.ToLower())).ToList();
            }

            var paginatedUsers = users
                .Skip((page - 1) * pageSize)
                //.Take(pageSize)
                .ToList();
            return View(paginatedUsers);
        }
        public async Task<IActionResult> AddAdminRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return RedirectToAction("Getuserlist");
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                
                TempData["Success"] = $"{user.Email}({user.Name})"; 
                // Admin rolü başarıyla silindi, isteğe bağlı olarak bir mesaj gösterilebilir
            }
            else
            {
                // Hata durumunda bir mesaj gösterilebilir
            }

            return RedirectToAction("GetUserlist");
        }
        public async Task<IActionResult> RemoveAdminRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                return RedirectToAction("Getuserlist");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded)
            {                
                TempData["Success"] = $"{user.Email}({user.Name})";

                // Admin rolü başarıyla silindi, isteğe bağlı olarak bir mesaj gösterilebilir
            }
            else
            {
                // Hata durumunda bir mesaj gösterilebilir
            }

            return RedirectToAction("GetUserlist");
        }
        //public async Task<IActionResult> Create(string tag)
        //{
        //    if(!string.IsNullOrEmpty(tag))
        //    {
        //        TempData["Tag"] = tag;
        //        var isTagVerification = await _mediator.Send(new IsTagVerificationCommand(tag));
        //        if (isTagVerification)
        //        {
        //            return RedirectToAction("GetTagDetails", "Tag", new {tag=tag});
        //        }
        //    }
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CreateUserCommand request,CancellationToken cancellationToken)
        //{
        //    var validator = new UserCreateValidator();
        //    var validationResult = validator.Validate(request);
        //    if (!validationResult.IsValid)
        //    {
        //        foreach (var error in validationResult.Errors)
        //        {
        //            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        //        }
        //        return View(request);
        //    }
        //    var result = await _mediator.Send(request, cancellationToken);
        //    if (!result.Succeeded)
        //    {
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.TryAddModelError(error.Code, error.Description);
        //        }
        //        return View(request);
        //    }
        //    var user=await _userManager.FindByEmailAsync(request.Email);
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var url = Url.Action("ConfirmEmail", "User", new
        //    {
        //        email = request.Email,
        //        token = token
        //    });           
        //    string body = $"Please confirm your email address by clicking this link: <a href='https://www.torruna.store{url}' style='background-color: #3c8dbc; border: none;color: white; padding: 10px 41px;text-align: center;text-decoration: none;display: inline-block; font-size: 16px;margin: 4px 2px;cursor: pointer;'>Confirm Email</a>";
        //    await _mediator.Publish(new EmailSentEvent(user.Email, "Mail register activation", body));
        //    var message = new { message = $"Before logging in, click on the confirmation e-mail we sent to {user.Email}'s e-mail address." };
        //    return RedirectToAction("Login","Account",message);            
        //}        
        //public async Task<IActionResult> ConfirmEmail(string token, string email)
        //{            
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //        return View("Error");
        //    var result = await _userManager.ConfirmEmailAsync(user, token);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Login", "User");
        //}
    }
}
