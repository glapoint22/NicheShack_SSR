using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Website.Classes;
using Website.Interfaces;
using Website.Models;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Customer> userManager;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;

        public AccountController(UserManager<Customer> userManager, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
        }



        // ..................................................................................Register.....................................................................
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(Account account)
        {
            // Add the new customer to the database
            IdentityResult result = await userManager.CreateAsync(account.CreateCustomer(), account.Password);

            // The new customer was successfully added to the database 
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                // There was a problem adding the customer to the database. Return with errors
                foreach (IdentityError error in result.Errors)
                {
                    if (error.Code == "DuplicateEmail")
                    {
                        error.Description = "The email address, \"" + account.Email.ToLower() + "\", already exists with another Niche Shack account. Please use another email address.";
                    }
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return Conflict(ModelState);
            }
        }





        // ..................................................................................Sign In.....................................................................
        [HttpPost]
        [Route("SignIn")]
        public async Task<ActionResult> SignIn(SignIn signIn)
        {
            // Get the customer from the database based on the email address
            Customer customer = await userManager.FindByEmailAsync(signIn.Email);


            // If the customer is in the database and the password is valid, create claims for the access token
            if (customer != null && await userManager.CheckPasswordAsync(customer, signIn.Password))
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim("acc", "customer"),
                    new Claim(ClaimTypes.NameIdentifier, customer.Id),
                    new Claim(JwtRegisteredClaimNames.Iss, configuration["TokenValidation:Site"]),
                    new Claim(JwtRegisteredClaimNames.Aud, configuration["TokenValidation:Site"]),
                    new Claim(ClaimTypes.IsPersistent, signIn.IsPersistent.ToString())
                };

                // Return with the token data
                return Ok(await GenerateTokenData<TokenDataDetail>(customer, claims));
            }

            return Unauthorized();
        }




        // ..................................................................................Update Customer Name.....................................................................
        [HttpPut]
        [Route("UpdateName")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdateCustomerName(UpdatedCustomerName updatedCustomerName)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // If the customer is found, update his/her name
            if (customer != null)
            {
                customer.FirstName = updatedCustomerName.FirstName;
                customer.LastName = updatedCustomerName.LastName;

                // Update the name in the database
                IdentityResult result = await userManager.UpdateAsync(customer);

                // If succeeded, return with the new updated name
                if (result.Succeeded)
                {
                    return Ok(new CustomerDTO
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = customer.Email
                    });
                }
            }


            return BadRequest();
        }








        // ..................................................................................Update Email.....................................................................
        [HttpPut]
        [Route("UpdateEmail")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdateEmail(UpdatedEmail updatedEmail)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            // If the customer is found...
            if (customer != null)
            {
                // Update the new email in the database
                IdentityResult result = await userManager.SetEmailAsync(customer, updatedEmail.Email);


                // If the update was successful, return the customer data with the new email
                if (result.Succeeded)
                {
                    return Ok(new CustomerDTO
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = updatedEmail.Email
                    });
                }
                else
                {
                    // The update was not successful. Return with errors
                    foreach (IdentityError error in result.Errors)
                    {
                        if (error.Code == "DuplicateEmail")
                        {
                            error.Description = "The email address, \"" + updatedEmail.Email.ToLower() + "\", already exists with another Niche Shack account. Please use another email address.";
                        }
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Conflict(ModelState);
                }
            }

            return BadRequest();
        }








        // ..................................................................................Update Password.....................................................................
        [HttpPut]
        [Route("UpdatePassword")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdatePassword(UpdatedPassword updatedPassword)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // If the customer is found...
            if (customer != null)
            {
                // Update the password in the database
                IdentityResult result = await userManager.ChangePasswordAsync(customer, updatedPassword.CurrentPassword, updatedPassword.NewPassword);


                // If the password was successfully updated, return ok
                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return Conflict();
        }





        // ..................................................................................Refresh.....................................................................
        [HttpGet]
        [Route("Refresh")]
        public async Task<ActionResult> Refresh(string refresh)
        {
            string accessToken = GetAccessTokenFromHeader();

            if (accessToken != null)
            {
                ClaimsPrincipal principal = GetPrincipalFromToken(accessToken);


                if (principal != null && refresh != null)
                {
                    string customerId = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                    if (customerId != null)
                    {
                        RefreshToken refreshToken = await unitOfWork.RefreshTokens.Get(x => x.Id == refresh && x.CustomerId == customerId);
                        if (refreshToken != null)
                        {
                            // Remove the refresh token from the database
                            unitOfWork.RefreshTokens.Remove(refreshToken);
                            await unitOfWork.Save();

                            if (DateTime.Compare(DateTime.UtcNow, refreshToken.Expiration) < 0)
                            {
                                Customer customer = await userManager.FindByIdAsync(customerId);

                                // Generate a new token and refresh token
                                return Ok(await GenerateTokenData<TokenData>(customer, principal.Claims));
                            }
                        }
                    }
                }
            }


            return Ok();
        }






        // ..................................................................................Sign Out.....................................................................
        [HttpGet]
        [Route("SignOut")]
        public async Task<ActionResult> SignOut(string refresh)
        {
            if (refresh != null)
            {
                RefreshToken refreshToken = await unitOfWork.RefreshTokens.Get(x => x.Id == refresh);

                if (refreshToken != null)
                {
                    unitOfWork.RefreshTokens.Remove(refreshToken);
                    await unitOfWork.Save();
                }

            }

            return NoContent();
        }









        // ..................................................................................Generate Token Data.....................................................................
        private async Task<T> GenerateTokenData<T>(Customer customer, IEnumerable<Claim> claims)
        {
            ITokenData<T> tokenData;

            // Generate the access token
            JwtSecurityToken accessToken = GenerateAccessToken(claims);

            // Generate the refresh token
            RefreshToken refreshToken = await GenerateRefreshToken(customer.Id);


            // If type of T is TokenData
            // This will return access token and refresh token info
            if (typeof(T) == typeof(TokenData))
            {
                tokenData = (ITokenData<T>)new TokenData();
            }
            else
            {
                // Type of T is TokenDataDetail
                // This will return access token, refresh token, and customer info
                tokenData = (ITokenData<T>)new TokenDataDetail();
            }


            return tokenData.GetTokenData(accessToken, refreshToken, customer);
        }








        // ..................................................................................Generate Access Token.....................................................................
        private JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims)
        {
            return new JwtSecurityToken(
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["TokenValidation:AccessExpiresInMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenValidation:SigningKey"])), SecurityAlgorithms.HmacSha256),
                claims: claims);
        }










        // ..................................................................................Generate Refresh Token.....................................................................
        private async Task<RefreshToken> GenerateRefreshToken(string customerId)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                RefreshToken refreshToken = new RefreshToken()
                {
                    Id = Convert.ToBase64String(randomNumber),
                    CustomerId = customerId,
                    Expiration = DateTime.UtcNow.AddDays(Convert.ToInt32(configuration["TokenValidation:RefreshExpiresInDays"]))
                };

                // Add to database
                unitOfWork.RefreshTokens.Add(refreshToken);

                await unitOfWork.Save();

                return refreshToken;
            }
        }












        // ..................................................................................Get Principal From Token....................................................................
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["TokenValidation:Site"],
                ValidIssuer = configuration["TokenValidation:Site"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenValidation:SigningKey"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal;

            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception)
            {

                return null;
            }


            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }



        // ..................................................................................Get Token From Header.....................................................................
        private string GetAccessTokenFromHeader()
        {
            StringValues value;
            Request.Headers.TryGetValue("Authorization", out value);

            if (value.Count == 0) return null;

            Match result = Regex.Match(value, @"(?:Bearer\s)(.+)");
            return result.Groups[1].Value;
        }
    }
}