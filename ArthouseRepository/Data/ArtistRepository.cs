using ArthouseRepository.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ArthouseRepository.Data
{
    public class ArtistRepository : IArtistRepository
    {
        private string _connectionString;
        public ArtistRepository(string connStr)
        {
            _connectionString = connStr;
        }
        /// <summary>
        /// Gets an ordered list suitable for use in a ComboBox
        /// </summary>
        /// <returns>An ordered list of Artist objects using the Lookup Class</returns>
        public async Task<List<Lookup>> GetArtists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<Lookup> artists = new List<Lookup>();
                try
                {
                    await connection.OpenAsync();
                    artists = (List<Lookup>)await connection.QueryAsync<Lookup>("ArtistList", commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {
                    throw;
                }
                return artists;
            }

        }
    }
}
