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
    public class ArtTypeRepository : IArtTypeRepository
    {
        private string _connectionString;
        public ArtTypeRepository(string connStr)
        {
            _connectionString = connStr;
        }
        /// <summary>
        /// Gets an ordered list suitable for use in a ComboBox
        /// </summary>
        /// <returns>An ordered list of ArtType objects using the Lookup Class</returns>
        public async Task<List<Lookup>> GetArtTypes()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<Lookup> artTypes = new List<Lookup>();
                try
                {
                    await connection.OpenAsync();
                    artTypes = (List<Lookup>)await connection.QueryAsync<Lookup>("ArtTypeList",
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {
                    throw;
                }
                return artTypes;
            }
        }
    }
}
