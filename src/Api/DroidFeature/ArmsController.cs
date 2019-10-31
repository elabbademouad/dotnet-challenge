using Cds.DroidManagement.Api.DroidFeature.ViewModels;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cds.DroidManagement.Api.DroidFeature
{
    /// <summary>
    /// Arms Controller
    /// </summary>
    [Route("api/Droids/{serialNumber:guid}/[controller]")]
    [ApiController]
    public class ArmsController : ControllerBase
    {
        public ArmsController(IReadArmRepository readArmRepository, IReadDroidRepository readDroidRepository, IDroidHandler droidHandler)
        {
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        public Task<ActionResult> PostAsync([FromRoute] Guid serialNumber)
        {
            return null;
        }

        [HttpGet]
        public Task<ActionResult<IReadOnlyCollection<ArmViewModel>>> GetAsync([FromRoute] Guid serialNumber)
        {
            return null;
        }
    }
}
