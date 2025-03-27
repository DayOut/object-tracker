using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using TrackedObjectServiceRepository.Interface;
using TrackedObjectServiceRepository.Model;
using UserServiceRepository.Interface;

namespace ObjectTrackerBackend.Controllers
{
    [ApiController]
    [Route("/api/TrackedObject")]
    public class TrackedObjectController : Controller
    {
        private readonly ITrackedObjectRepository _TrackedObjectRepository;

        public TrackedObjectController(ITrackedObjectRepository TrackedObjectRepository)
        {
            _TrackedObjectRepository = TrackedObjectRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TrackedObject TrackedObject)
        {
            var response = await _TrackedObjectRepository.CreateAsync(TrackedObject);
            return CreatedAtRoute(nameof(GetTrackedObjectByIdAsync), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([Required] string id, TrackedObject TrackedObject)
        {
            await _TrackedObjectRepository.UpdateAsync(id, TrackedObject);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _TrackedObjectRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}", Name = nameof(GetTrackedObjectByIdAsync))]
        public async Task<IActionResult> GetTrackedObjectByIdAsync([Required] string id)
            => Ok(await _TrackedObjectRepository.GetByIdAsync(id));

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int offset = 0, [FromQuery] int fetch = 100)
            => Ok(await _TrackedObjectRepository.GetAllAsync(offset, fetch));
    }
}