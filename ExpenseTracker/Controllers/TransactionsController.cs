using ExpenseTracker.Data;
using ExpenseTracker.Data.Services;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    [EnableCors("AllowAll")]
    public class TransactionsController(ITransactionServices tranService) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto tran)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("User Id not found");
            }
            if (!int.TryParse(userId, out int id)) {
                BadRequest("Parsing not possible");
            }
            await tranService.CreateTransaction(tran, id);
            return Ok(new { message = "Created Successfully" });
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAllTransactions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("User Id not found");
            }
            if (!int.TryParse(userId, out int id))
            {
                BadRequest("Parsing not possible");
            }
            var AllTransactions = await tranService.GetAllTransaction(id);

            return Ok(AllTransactions);
        }
        [HttpGet("details/{id}")]
        public async Task<ActionResult> GetTransactionById(int id)
        {
            var tran = await tranService.GetTransactionById(id);
            if (tran == null)
            {
                return BadRequest("Transaction Not Found");
            }
            return Ok(tran);
        
        }
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateTransaction(int id, [FromBody] TransactionDto transaction)
        {
            if (transaction == null) {
                return BadRequest("Enter valid transaction details");
            }
            var tran = await tranService.UpdateTransaction(id, transaction);

            if (tran == null)
            {
                return BadRequest("Updated transaction not found");
            }
           
            return Ok(tran);

        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            var tran = await tranService.DeleteTransaction(id);

            if (tran == null)
            {
                return BadRequest("Transaction not found");

            }

            return Ok(new { message = "Deleted Successfully" });
        }
        }
}
