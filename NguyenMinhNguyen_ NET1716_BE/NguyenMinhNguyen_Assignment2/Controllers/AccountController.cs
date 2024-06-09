using DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using NguyenMinhNguyen_Assignment2.MessageStatusResponse;
using Service.Interface;

namespace NguyenMinhNguyen_Assignment2.Controllers
{
    [Route("api/account/")]
    [ApiController]
    public class AccountController : ODataController
    {
        private readonly ISystemAccountService _systemAccountService;

        public AccountController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [Authorize(policy: "Admin")]
        [EnableQuery]
        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            var accounts = await _systemAccountService.GetAllAccount();
            return Ok(accounts);
        }

        [Authorize(policy: "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AccountCreate accountCreate)
        {
            var account = await _systemAccountService.EmailExisted(accountCreate.AccountEmail);
            if (account != null)
            {
                return BadRequest(new ApiResponseStatus(404, "Email is existed!"));
            }
            await _systemAccountService.CreateAccount(accountCreate);
            return Ok();
        }

        [Authorize(policy: "Admin")]
        [HttpDelete("delete/{accountID}")]
        public async Task<IActionResult> Delete([FromRoute] int accountID)
        {
            bool check = await _systemAccountService.DeleteAccounts(accountID);
            if (!check)
            {
                return BadRequest(new ApiResponseStatus(404, "The item is already stored in a news article cannot delete."));
            }
            return Ok(check);
        }

        [Authorize(policy: "Admin")]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] AccountUpdate accountUpdate)
        {
            var accountEmail =  _systemAccountService.GetAccountProfile(accountUpdate.AccountId).Result.AccountEmail;
            if (accountUpdate.AccountEmail == accountEmail)
            {
                await _systemAccountService.UpdateAccount(accountUpdate);
                return Ok();
            }
            else
            {
                var account = await _systemAccountService.EmailExisted(accountUpdate.AccountEmail);
                if (account != null)
                {
                    return BadRequest(new ApiResponseStatus(404, "Email is existed!"));
                }
                else
                {
                    await _systemAccountService.UpdateAccount(accountUpdate);
                    return Ok();
                }
            }
        }

        [Authorize(policy: "Staff")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetDProfile(int accountID)
        {
            var account = await _systemAccountService.GetAccountProfile(accountID);
            return Ok(account);
        }
    }
}
