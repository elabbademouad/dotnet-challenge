using System;
using System.Linq;
using System.Threading.Tasks;
using Cds.DroidManagement.Api.Extensions;
using Cds.DroidManagement.Api.DroidFeature.ViewModels;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Cds.DroidManagement.Domain.DroidAggregate;
using System.Net;

namespace Cds.DroidManagement.Api.DroidFeature
{
    /// <summary>
    /// Droids Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DroidsController : ControllerBase
    {
        // INFO: readRepository retrieve one entity level (not all aggregate)
        private readonly IReadDroidRepository _readDroidRepository;
        private readonly IDroidHandler _droidHandler;

        /// <summary>
        /// DroidsController constructor
        /// </summary>
        /// <param name="readDroidRepository"></param>
        /// <param name="droidHandler"></param>
        public DroidsController(IReadDroidRepository readDroidRepository, IDroidHandler droidHandler)
        {
            _readDroidRepository = readDroidRepository ?? throw new ArgumentNullException(nameof(readDroidRepository));
            _droidHandler = droidHandler ?? throw new ArgumentNullException(nameof(droidHandler));
        }

        /// <summary>
        /// Get all droids paged
        /// </summary>
        /// <param name="pageIndex">Index of the page</param>
        /// <param name="pageSize">Size of page(Default = 10)</param>
        /// <returns>
        /// A list of droids
        /// </returns>
        /// <remarks>
        /// Get all droids paged from droidRepository.<br/>
        /// There is no filter.<br/>
        /// </remarks>
        /// <response code="200">Successfully retrieve droids</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<ActionResult<PagedList<DroidViewModel>>> GetAsync(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 10)
        {
            var nbElementSkiped = pageIndex * pageSize;
            var (droidDtos, hasNextPage) = await _readDroidRepository.GetAllPagedAsync(nbElementSkiped, pageSize);
            var droidViewModels = droidDtos.Select(d => d.ToViewModel()).ToList();

            return Ok(new PagedList<DroidViewModel>
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                HasNextPage = hasNextPage,
                Items = droidViewModels
            });
        }

        /// <summary>
        /// Get a specific droids
        /// </summary>
        /// <param name="serialNumber">Droid Serial Number.</param>
        /// <returns>
        /// The droid in body and the resource location in headers
        /// </returns>
        /// <remarks>
        /// Get a specific droid from droid repository.<br/>
        /// </remarks>
        /// <response code="200">Successfully retrieve my droid</response>
        /// <response code="404">Droid not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{serialNumber:guid}")]
        public async Task<ActionResult<DroidViewModel>> GetAsync([FromRoute] Guid serialNumber)
        {
            try
            {
                var droidDto = await _readDroidRepository.GetBySerialNumberAsync(serialNumber, Droid.AssertExists);

                var droidViewModels = droidDto.ToViewModel();
                return Ok(droidViewModels);
            }
            catch (BusinessException ex) { return ex.GetStatusCode(); }
        }

        /// <summary>
        /// Add a Droid
        /// </summary>
        /// <param name="createDroid">Creation command for a droid</param>
        /// <returns>
        /// A specific droid
        /// </returns>
        /// <remarks>
        /// Add a new Droid with the specified name if no other droid has the same name.<br/>
        /// </remarks>
        /// <response code="204">Successfully created</response>
        /// <response code="404">Droid not found</response>
        /// <response code="409">Droid with this name already exists</response>
        /// <response code="500">Internal server error</response>
        // INFO: Redifine all http codes to prevent swagger to put 200 by default
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] CreateDroid createDroid)
        {
            try
            {
                var droid = await _droidHandler.HandleAsync(createDroid);
                return CreatedAtAction(
                    nameof(GetAsync),
                    new { serialNumber = (Guid)droid.SerialNumber },
                    droid.ToViewModel());
            }
            catch (BusinessException ex) { return ex.GetStatusCode(); }
        }

        /// <summary>
        /// Update a Droid
        /// </summary>
        /// <param name="serialNumber">Droid serial number.</param>
        /// <param name="updateDroid">Droid.</param>
        /// <remarks>
        /// Update the nickname of the Droid with the specified serial number.<br/>
        /// </remarks>
        /// <response code="204">Successfully updated</response>
        /// <response code="404">Droid not found</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPut("{serialNumber:guid}")]
        public async Task<ActionResult> PutAsync([FromRoute] Guid serialNumber, [FromBody] UpdateDroid updateDroid)
        {
            try
            {
                updateDroid.WithSerialNumber(serialNumber);
                await _droidHandler.HandleAsync(updateDroid);
                return NoContent();
            }
            catch (BusinessException ex) { return ex.GetStatusCode(); }
        }

        /// <summary>
        /// Delete a Droid
        /// </summary>
        /// <param name="serialNumber">Droid serial number.</param>
        /// <remarks>
        /// Update the name of the Droid with the specified serial number.<br/>
        /// </remarks>
        /// <response code="204">Successfully updated</response>
        /// <response code="404">Droid not found</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpDelete("{serialNumber:guid}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] Guid serialNumber)
        {
            try
            {
                await _droidHandler.HandleAsync(new DeleteDroid(serialNumber));
                return NoContent();
            }
            catch (BusinessException ex) { return ex.GetStatusCode(); }
        }
    }
}
