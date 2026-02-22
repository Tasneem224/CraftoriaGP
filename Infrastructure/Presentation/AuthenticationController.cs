using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.IdentityModule;
using Shared.Resources;

namespace Presentation
{
    public class AuthenticationController(IServiceManager _serviceManager, IAuthenticationService _authenticationService, IStringLocalizer<SharedResources> _stringLocalizer) : BaseApiController
    {
        [HttpPost("Register")]
                public async Task<IActionResult> Registeration(RegisterDto _customerRegisterDto)
                {

            
                        var user = await _serviceManager.AuthenticationService.RegisterAsync(_customerRegisterDto);
                        return SendSuccessResponse(user, _stringLocalizer[SharedResourcesKeys.RegistrationSuccessful]);
                }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return SendErrorResponse(_stringLocalizer[SharedResourcesKeys.ValidationFailed], ModelState, 422);
            }

            var user = await _serviceManager.AuthenticationService.LoginAsync(loginDTO);

            return SendSuccessResponse(user, _stringLocalizer[SharedResourcesKeys.LoginSuccessful]);

        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return SendErrorResponse(_stringLocalizer[SharedResourcesKeys.ValidationFailed], ModelState, 422);

            var result = await _authenticationService.ForgotPasswordAsync(dto.Email);

            return SendSuccessResponse(result, _stringLocalizer[SharedResourcesKeys.SendOtpSuccessfully]);
        }

    

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
                return SendErrorResponse(_stringLocalizer[SharedResourcesKeys.ValidationFailed], ModelState, 422);

            var isValid = await _authenticationService.VerifyOtpAsync(dto);

                if (!isValid)
                {
                return SendErrorResponse(_stringLocalizer[SharedResourcesKeys.ExpiredOtp], null, 400);
            }


            var responseData = new
            {
                Email = dto.Email,
                Code = dto.OtpCode
            };

            return SendSuccessResponse(responseData, _stringLocalizer[SharedResourcesKeys.CodeVerifiedSuccessfully]);


        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            return SendErrorResponse(_stringLocalizer[SharedResourcesKeys.ValidationFailed], ModelState, 422);
            var result = await _authenticationService.ResetPasswordAsync(dto);

            // return Ok(new { message = result });
            return SendSuccessResponse(result, _stringLocalizer[SharedResourcesKeys.PasswordResetSuccessfully]);
        }


        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDTO verifyEmailDTO)
        {
           var verify= await _authenticationService.VerifyEmailAsync(verifyEmailDTO);
            //return SendSuccessResponse(verify, $"OTP is sent successfully to {verifyEmailDTO.Email} ,Check Your Email");
            return SendSuccessResponse(verify, _stringLocalizer[SharedResourcesKeys.OtpSentSuccessfullyToEmail, verifyEmailDTO.Email]);
        }
        [HttpPost("CheckEmailOtp")]
        public async Task<IActionResult> CheckEmailOtp(VerifyOtpDto verifyOtpDto)
        {
            var check=await _authenticationService.CheckEmailOTPAsync(verifyOtpDto);
            return  SendSuccessResponse(check, _stringLocalizer[SharedResourcesKeys.EmailVerifiedSuccessfully]);

        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin( GoogleLoginDto model)
        {
            // Validation
            if (!ModelState.IsValid)
                return SendErrorResponse(_stringLocalizer[SharedResourcesKeys.ValidationFailed], ModelState, 422);

            // Call Service
            var result = await _authenticationService.GoogleLoginAsync(model);

            // Return Uniform Response
            return SendSuccessResponse(result, _stringLocalizer[SharedResourcesKeys.GoogleLoginSuccessful]);
        }

        [HttpPost("Expert-With-google")]
        public async Task<IActionResult>ExpertWithGoolge(ExpertWithGoolgeDto infos)
        {
            // Validation
            if (!ModelState.IsValid)
                return SendErrorResponse(_stringLocalizer[SharedResourcesKeys.ValidationFailed], ModelState, 422);

            await _authenticationService.ExpertWithGoolgeService(infos);
            return Ok(_stringLocalizer[SharedResourcesKeys.ExpertInfoSavedSuccessfully]);


        }


    }

}
