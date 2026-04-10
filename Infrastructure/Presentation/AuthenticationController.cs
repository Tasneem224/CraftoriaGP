using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.IdentityModule;

namespace Presentation
{
    public class AuthenticationController(IServiceManager _serviceManager, IAuthenticationService _authenticationService) : BaseApiController
    {
        [HttpPost("Register")]

        public async Task<IActionResult> Registeration(RegisterDto _customerRegisterDto)
                {

            
                        var user = await _serviceManager.AuthenticationService.RegisterAsync(_customerRegisterDto);
                        return SendSuccessResponse(user, "Registration successful");
                }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return SendErrorResponse("Validation failed", ModelState, 422);
            }

            var user = await _serviceManager.AuthenticationService.LoginAsync(loginDTO);

            return SendSuccessResponse(user, "Login  successful");

        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return SendErrorResponse("Validation failed", ModelState, 422);

            var result = await _authenticationService.ForgotPasswordAsync(dto.Email);

            return SendSuccessResponse(result, "OTP sent successfully");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
                return SendErrorResponse("Validation failed", ModelState, 422);

            var isValid = await _authenticationService.VerifyOtpAsync(dto);

                if (!isValid)
                {
                return SendErrorResponse("The Otp Code is wrong or has expired", null, 400);
            }


            var responseData = new
            {
                Email = dto.Email,
                Code = dto.OtpCode
            };

            return SendSuccessResponse(responseData, "Code Verified Successfully");


        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            return SendErrorResponse("Validation failed", ModelState, 422);
            var result = await _authenticationService.ResetPasswordAsync(dto);

            // return Ok(new { message = result });
            return SendSuccessResponse(result, "Password reset successfully");
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDTO verifyEmailDTO)
        {
           var verify= await _authenticationService.VerifyEmailAsync(verifyEmailDTO);
            return SendSuccessResponse(verify, $"OTP is sent successfully to {verifyEmailDTO.Email} ,Check Your Email");
        }
        [HttpPost("CheckEmailOtp")]
        public async Task<IActionResult> CheckEmailOtp(VerifyOtpDto verifyOtpDto)
        {
            var check=await _authenticationService.CheckEmailOTPAsync(verifyOtpDto);
            return  SendSuccessResponse(check, $"Email Is verified successfully ");

        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin( GoogleLoginDto model)
        {
            // Validation
            if (!ModelState.IsValid)
                return SendErrorResponse("Validation failed", ModelState, 422);

            // Call Service
            var result = await _authenticationService.GoogleLoginAsync(model);

            // Return Uniform Response
            return SendSuccessResponse(result, "Google Login successful");
        }

        [HttpPost("Expert-With-google")]
        public async Task<IActionResult>ExpertWithGoolge(ExpertWithGoolgeDto infos)
        {
            // Validation
            if (!ModelState.IsValid)
                return SendErrorResponse("Validation failed", ModelState, 422);

            await _authenticationService.ExpertWithGoolgeService(infos);
            return Ok("Expert information saved successfully!😊😊😊😊");


        }


    }

}
