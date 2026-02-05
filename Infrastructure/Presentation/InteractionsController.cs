using DomainLayer.Models.Interaction;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class InteractionsController:BaseApiController
    {
        private readonly IInteractionService _interactionService;

        public InteractionsController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromBody] InteractionDto dto)
        {
            var result = await _interactionService.AddOrUpdateAsync(dto);
            return SendSuccessResponse(result,"review is added successfully");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(
        [FromQuery] string userId,
        [FromQuery] string targetId,
        [FromQuery] InteractionTargetType2 targetType)
        {
            await _interactionService.DeleteAsync(
                userId,
                targetId,
                targetType);

            return SendSuccessResponse("review was deleted sccessfully");
        }
        [HttpGet("target/{targetId}")]
        public async Task<IActionResult> GetAllForTarget(string targetId,[FromQuery] InteractionTargetType2 targetType)
        {
            var results = await _interactionService.GetAllAsync(
                targetId,
                targetType);

            return SendSuccessResponse(results);
        }

        [HttpGet("count/{targetId}")]
        public async Task<IActionResult> GetReviewCount(string targetId,[FromQuery] InteractionTargetType2 targetType)
        {
            var count = await _interactionService.GetReviewCountAsync(
                targetId,
                targetType);

            return SendSuccessResponse(new
            {
                TargetId = targetId,
                ReviewCount = count
            });
        }
    }
}
