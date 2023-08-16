using ArthouseRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArthouseRepository.Data
{
    public interface IArtworkRepository
    {
        Task<List<Artwork>> GetArtworks();
        Task<Artwork> GetArtwork(int ID);
        Task<List<Artwork>> GetArtworksByX(int ArtTypeID, int ArtistID, string Title);
        Task<int> AddArtwork(Artwork artworkToAdd);
        Task<int> UpdateArtwork(Artwork artworkToUpdate);
        Task<int> DeleteArtwork(Artwork artworkToDelete);
    }
}
