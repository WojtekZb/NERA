using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Logic.Services;
using Logic.SimpleMailTransferProtocol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ICreateEventRepo _eventRepo;
        private readonly UpdateEventService _updateEventService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EventController> _logger;

        public EventController(
            ICreateEventRepo eventRepo,
            UpdateEventService updateEventService,
            IEmailSender emailSender,
            ILogger<EventController> logger)
        {
            _eventRepo = eventRepo;
            _updateEventService = updateEventService;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditEvent(Guid id, [FromBody] EditEventDto dto)
        {
            // 1) Fetch existing event
            var existing = await _eventRepo.GetByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("EditEvent: Event {EventId} not found.", id);
                return NotFound(new { message = "Event not found." });
            }

            // 2) Apply changes via your domain service (keeps business rules centralized)
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Location = dto.Location;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.Status = dto.Status;

            await _updateEventService.UpdateEventAsync(existing);

            // 3) Compose notification (Edited)
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "User";
            var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm zzz", CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    await _emailSender.SendEventNotificationEmailAsync(
                        toEmail: userEmail,
                        toName: userName,
                        eventName: existing.Title,
                        action: "Edited");

                    _logger.LogInformation("EditEvent: Notification sent to {Email} for event '{EventTitle}'.",
                        userEmail, existing.Title);
                }
                catch (Exception ex)
                {
                    // Graceful handling: do not fail the request if email fails
                    _logger.LogError(ex, "EditEvent: Failed to send notification to {Email}.", userEmail);
                }
            }
            else
            {
                _logger.LogWarning("EditEvent: No email claim found for current user; notification skipped.");
            }

            return Ok(existing);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            // 1) Fetch the event to preserve its name for the email (because after delete it's gone)
            var existing = await _eventRepo.GetByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("DeleteEvent: Event {EventId} not found.", id);
                return NotFound(new { message = "Event not found." });
            }

            // 2) Delete via repository
            await _eventRepo.DeleteEventAsync(id);

            // 3) Compose notification (Deleted)
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "User";
            var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm zzz", CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    await _emailSender.SendEventNotificationEmailAsync(
                        toEmail: userEmail,
                        toName: userName,
                        eventName: existing.Title,
                        action: "Deleted");

                    _logger.LogInformation("DeleteEvent: Notification sent to {Email} for event '{EventTitle}'.",
                        userEmail, existing.Title);
                }
                catch (Exception ex)
                {
                    // Graceful handling: do not fail the request if email fails
                    _logger.LogError(ex, "DeleteEvent: Failed to send notification to {Email}.", userEmail);
                }
            }
            else
            {
                _logger.LogWarning("DeleteEvent: No email claim found for current user; notification skipped.");
            }

            return NoContent();
        }
    }

    public sealed class EditEventDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public EventStatus Status { get; set; } 
    }
}
