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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditEvent(int id, [FromBody] EditEventDto dto)
        {
            // 1) Fetch existing event
            Event existing = await _eventRepo.GetByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("EditEvent: Event {EventId} not found.", id);
                return NotFound(new { message = "Event not found." });
            }

            // Keep a copy of the old event for comparison
            var oldEvent = new Event
            {
                Id = existing.Id,
                Title = existing.Title,
                Description = existing.Description,
                Adress = existing.Adress,
                StartDate = existing.StartDate,
                EndDate = existing.EndDate,
                CGI = existing.CGI,
                Cost = existing.Cost,
                Capacity = existing.Capacity,
                Status = existing.Status
            };

            // 2) Apply changes via domain service
            existing.Title = dto.Title;
            existing.Description = dto.Description ?? existing.Description;
            existing.Adress = dto.Location ?? existing.Adress;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.Status = dto.Status;

            await _updateEventService.UpdateEventAsync(existing);

            // 3) Compose notification (Edited)
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "User";

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    await _emailSender.SendEventEditedEmailAsync(
                        toEmail: userEmail,
                        toName: userName,
                        oldEvent: oldEvent,
                        newEvent: existing);

                    _logger.LogInformation("EditEvent: Edited notification sent to {Email} for event '{EventTitle}'.",
                        userEmail, existing.Title);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "EditEvent: Failed to send edited notification to {Email}.", userEmail);
                }
            }
            else
            {
                _logger.LogWarning("EditEvent: No email claim found for current user; notification skipped.");
            }

            return Ok(existing);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            // 1) Fetch the event to preserve its details for the email
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

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    await _emailSender.SendEventDeletedEmailAsync(
                        toEmail: userEmail,
                        toName: userName,
                        ev: existing);

                    _logger.LogInformation("DeleteEvent: Deleted notification sent to {Email} for event '{EventTitle}'.",
                        userEmail, existing.Title);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "DeleteEvent: Failed to send deleted notification to {Email}.", userEmail);
                }
            }
            else
            {
                _logger.LogWarning("DeleteEvent: No email claim found for current user; notification skipped.");
            }

            return NoContent();
        }
    }

    public sealed class EditEventMapper
    {
        public static EditEventDto ToDto(Event ev) => new EditEventDto
        {
            Title = ev.Title,
            Description = ev.Description,
            Location = ev.Adress,
            StartDate = ev.StartDate,
            EndDate = ev.EndDate,
            Status = ev.Status
        };
    }

    public sealed class EditEventDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventStatus Status { get; set; }
    }
}