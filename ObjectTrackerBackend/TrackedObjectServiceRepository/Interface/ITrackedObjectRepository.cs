using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackedObjectServiceRepository.Model;

namespace TrackedObjectServiceRepository.Interface
{
    public interface ITrackedObjectRepository
    {
        Task<TrackedObject> CreateAsync(TrackedObject model);
        Task UpdateAsync(string id, TrackedObject model);
        Task DeleteAsync(string id);
        Task<TrackedObject> GetByIdAsync(string id);
        Task<IEnumerable<TrackedObject>> GetAllAsync(int offset, int fetch);
    }
}
