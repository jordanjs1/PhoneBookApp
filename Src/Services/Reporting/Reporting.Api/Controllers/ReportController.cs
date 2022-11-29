using Microsoft.AspNetCore.Mvc;
using Reporting.Api.Infrastructure.Repositories;
using Reporting.Api.IntegrationEvents;
using Reporting.Api.IntegrationEvents.Events;
using Reporting.Api.Models;
using System.Net;

namespace Reporting.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _repository;
        private readonly IReportingIntegrationEventService _reportingIntegrationEventService;

        public ReportController(IReportRepository repository, IReportingIntegrationEventService reportingIntegrationEventService)
        {
            _repository = repository;
            _reportingIntegrationEventService = reportingIntegrationEventService ?? throw new ArgumentNullException(nameof(reportingIntegrationEventService));
        }

        [HttpGet]
        [Route("GetAllReports")]
        [ProducesResponseType(typeof(IEnumerable<Report>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Report>>> GetAllReportsAsync()
        {
            var result = await _repository.GetAllReportsAsync();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetReportById")]
        [ProducesResponseType(typeof(Report), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Report>> GetReportByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("The given GUID is empty.");

            var parseResult = Guid.TryParse(id, out var guid);
            if (!parseResult)
                return BadRequest("The given GUID is not in a valid format.");

            var result = await _repository.GetReportByIdAsync(guid);

            if (result == default(Report))
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("GenerateReport")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Guid>> GenerateReportAsync()
        {
            var newReportId = await _repository.CreateNewReportAsync();
            var reportRequestedEvent = new ReportRequestedIntegrationEvent
            {
                ReportId = newReportId
            };

            await _reportingIntegrationEventService.PublishThroughEventBusAsync(reportRequestedEvent);

            return Ok(newReportId);
        }
    }
}
