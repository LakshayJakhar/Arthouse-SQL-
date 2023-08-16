using ArthouseRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArthouseRepository.Data
{
    public interface IArtTypeRepository
    {
        Task<List<Lookup>> GetArtTypes();
    }
}
