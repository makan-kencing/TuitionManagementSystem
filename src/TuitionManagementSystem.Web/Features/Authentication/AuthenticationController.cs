namespace TuitionManagementSystem.Web.Features.Authentication;

using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Accounts.CheckEmail;
using Ardalis.Result;
using AutoMapper;
using Infrastructure.Persistence;
using Login;
using Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Security;
using Services.Email;

[Route("/[action]")]
public sealed class AuthenticationController(
    IMediator mediator,
    IMapper mapper,
    ApplicationDbContext db,
    IEmailService emailService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => this.View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromForm][Required] LoginViewModel login, [FromQuery] Uri? returnUrl)
    {
        var result = await mediator.Send(mapper.Map<LoginRequest>(login));

        if (!result.IsOk())
        {
            // Error
            this.ModelState.AddModelError("Invalid", "Username or password is incorrect.");
            return this.View(login);
        }

        if (result.Value.Status == LoginResponseStatus.TwoFactorRequired)
        {
            return this.View("TwoFactor", login);
        }
        return this.LocalRedirect(returnUrl?.OriginalString ?? "/");
    }

    [HttpPost]
    public async Task<IActionResult> TwoFactor() =>
        throw new NotImplementedException();

    [HttpPost]
    public async Task<IActionResult> Logout([FromHeader] Uri? referer)
    {
        await mediator.Send(new LogoutRequest());
        return this.LocalRedirect(referer?.LocalPath ?? "/");
    }

    private readonly ApplicationDbContext _db = db;
    private readonly IEmailService _emailService = emailService;

    // --------------------------
    // Forgot Password
    // --------------------------

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword() => this.View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.View(model);

        var emailCheckResult = await mediator.Send(new CheckEmailRequest(model.Email));

        if (!emailCheckResult.IsSuccess || !emailCheckResult.Value.Exists)
        {
            TempData["Error"] = "Email does not exist or is invalid";
            return this.View(model);
        }

        var account = await _db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == model.Email && x.DeletedAt == null);

        if (account != null)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("ForgotPasswordEmail", account.Email!);
            HttpContext.Session.SetString("ForgotPasswordOtp", otp);
            HttpContext.Session.SetString("ForgotPasswordOtpExpiry", DateTime.UtcNow.AddMinutes(5).ToString());

            var mail = new MailMessage
            {
                To = { new MailAddress(account.Email!) },
                Subject = "Your OTP for password reset",
                Body = $"<p>Your OTP is: <b>{otp}</b>. It expires in 5 minutes.</p>",
                IsBodyHtml = true
            };

            await _emailService.SendAsync(mail);
        }

        return RedirectToAction("EnterOtp");
    }

    // --------------------------
    //  OTP Verification
    // --------------------------

    [HttpGet]
    [AllowAnonymous]
    public IActionResult EnterOtp() => this.View();

    [HttpPost]
    [AllowAnonymous]
    public IActionResult EnterOtp(VerifyOtpViewModel model)
    {
        var otp = HttpContext.Session.GetString("ForgotPasswordOtp");
        var expiry = HttpContext.Session.GetString("ForgotPasswordOtpExpiry");
        var email = HttpContext.Session.GetString("ForgotPasswordEmail");

        if (otp == null || expiry == null || email == null)
        {
            ModelState.AddModelError("", "Session expired. Please try again.");
            return View(model);
        }

        if (DateTime.UtcNow > DateTime.Parse(expiry))
        {
            ModelState.AddModelError("", "OTP expired. Please resend.");
            return View(model);
        }

        if (model.Otp != otp)
        {
            ModelState.AddModelError(nameof(model.Otp), "Incorrect OTP. Please try again.");
            return View(model);
        }

        return RedirectToAction("NewPassword");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResendOtp()
    {
        var email = HttpContext.Session.GetString("ForgotPasswordEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("ForgotPassword");
        }

        var otp = new Random().Next(100000, 999999).ToString();
        HttpContext.Session.SetString("ForgotPasswordOtp", otp);
        HttpContext.Session.SetString("ForgotPasswordOtpExpiry", DateTime.UtcNow.AddMinutes(5).ToString());

        var mail = new MailMessage
        {
            To = { new MailAddress(email) },
            Subject = "Your OTP for password reset",
            Body = $"<p>Your OTP is: <b>{otp}</b>. It expires in 5 minutes.</p>",
            IsBodyHtml = true
        };

        await _emailService.SendAsync(mail);

        TempData["Message"] = "OTP resent to your email.";
        return RedirectToAction("EnterOtp");
    }

    // --------------------------
    //  Set New Password
    // --------------------------

    [HttpGet]
    [AllowAnonymous]
    public IActionResult NewPassword() => this.View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> NewPassword(NewPasswordViewModel model)
    {
        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match.");
            return View(model);
        }

        var email = HttpContext.Session.GetString("ForgotPasswordEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("ForgotPassword");
        }

        var account = await _db.Accounts.FirstOrDefaultAsync(x => x.Email == email && x.DeletedAt == null);
        if (account == null) return BadRequest();

        var hasher = new PasswordHasher<Account>();
        account.HashedPassword = hasher.HashPassword(account, model.NewPassword);
        account.LastChanged = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        // Clear session
        HttpContext.Session.Remove("ForgotPasswordEmail");
        HttpContext.Session.Remove("ForgotPasswordOtp");
        HttpContext.Session.Remove("ForgotPasswordOtpExpiry");

        return RedirectToAction("Login", "Authentication");
    }
}
