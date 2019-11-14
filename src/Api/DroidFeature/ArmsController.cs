using Cds.DroidManagement.Api.DroidFeature.ViewModels;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Cds.DroidManagement.Api.Extensions;

namespace Cds.DroidManagement.Api.DroidFeature
{
    /// <summary>
    /// Arms Controller
    /// </summary>
    [Route("api/Droids/{serialNumber:guid}/[controller]")]
    [ApiController]
    public class ArmsController : ControllerBase
    {
        /// <summary>
        /// DroitHandler
        /// </summary>
        private readonly IDroidHandler _droidHandler;

        /// <summary>
        /// Read only ArmRepository
        /// </summary>
        private readonly IReadArmRepository _readArmRepository;

        /// <summary>
        /// Read only DroidRepository
        /// </summary>
        private readonly IReadDroidRepository _readDroidRepository;

        /// <summary>
        /// ArmsController constructor
        /// </summary>
        /// <param name="readArmRepository"></param>
        /// <param name="readDroidRepository"></param>
        /// <param name="droidHandler"></param>
        public ArmsController(IReadArmRepository readArmRepository, IReadDroidRepository readDroidRepository, IDroidHandler droidHandler)
        {
            _droidHandler = droidHandler;
            _readArmRepository = readArmRepository;
            _readDroidRepository = readDroidRepository;
        }

        /// <summary>
        /// Add a Arm
        /// </summary>
        /// <param name="serialNumber">Droid serial number</param>
        /// <response code="204">Successfully created</response>
        /// <response code="404">Droid not found</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromRoute] Guid serialNumber)
        {
            try
            {
                CreateArm arm = new CreateArm(serialNumber);
                await _droidHandler.HandleAsync(arm);
                return NoContent();
            }
            catch (BusinessException ex) { return ex.GetStatusCode(); }
        }

        /// <summary>
        /// Get Arms with specific Droit
        /// </summary>
        /// <param name="serialNumber">Droit serial number</param>
        /// <returns></returns>
        /// <response code="200">Successfully retrieve Arms with specific Droit/response>
        /// <response code="404">Droid not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<ArmViewModel>>> GetAsync([FromRoute] Guid serialNumber)
        {
            try
            {
                var droitDto = await _readDroidRepository.GetBySerialNumberAsync(serialNumber, _ => { });
                var armsDto = await _readArmRepository.GetDroidArmsAsync(droitDto.DroidId);
                List<ArmViewModel> armsViewModel = new List<ArmViewModel>();
                foreach (var armDto in armsDto)
                {
                    armsViewModel.Add(armDto.ToViewModel());
                }
                return Ok(armsViewModel);

            }
            catch (BusinessException ex) { return ex.GetStatusCode(); }
        }

    }
}
