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
    public class ArtworkRepository : IArtworkRepository
    {
        private string _connectionString;
        public ArtworkRepository(string connStr)
        {
            _connectionString = connStr;
        }

        /// <summary>
        /// Gets all Artworks in the databse.
        /// </summary>
        /// <returns>List of Artwork Objects</returns>
        public async Task<List<Artwork>> GetArtworks()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<Artwork> artWorks = new List<Artwork>();
                try
                {
                    await connection.OpenAsync();
                    artWorks = (List<Artwork>)await connection.QueryAsync<Artwork>("ArtworkSelectAll",
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {
                    throw;
                }
                return artWorks;
            }
        }

        /// <summary>
        /// Gets a single Artwork Object identified by the ID primary key value
        /// </summary>
        /// <param name="ID">Primary key value</param>
        /// <returns>A single Artwork Object</returns>
        /// 
        public async Task<Artwork> GetArtwork(int ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Artwork artWork = new Artwork();
                try
                {
                    await connection.OpenAsync();
                    artWork = (Artwork)await connection.QueryAsync<Artwork>("ArtworkSelectByID",
                        new { ID },
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {
                    throw;
                }

                return artWork;
            }
        }

        /// <summary>
        /// Gets a filtered collection of Artwork objects.
        /// </summary>
        /// <param name="ArtTypeID">ID of the Type Of Art used to filter.  Note that the value 0 negates the filter</param>
        /// <param name="ArtistID">ID of the Artist used to filter.  Note that the value 0 negates the filter</param>
        /// <param name="Title">Characters anywhere in the Title</param>
        /// <returns>Artwork objects</returns>
        /// 
        public async Task<List<Artwork>> GetArtworksByX(int ArtTypeID, int ArtistID, string Title)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<Artwork> artWorks = new List<Artwork>();
                try
                {
                    await connection.OpenAsync();
                    artWorks = (List<Artwork>)await connection.QueryAsync<Artwork>("ArtworkSelectByX",
                        new { ArtTypeID, ArtistID, Title },
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {
                    throw;
                }

                return artWorks;
            }
        }

        /// <summary>
        /// Insert new Artwork
        /// </summary>
        /// <param name="artworkToAdd">Artwork object to add</param>
        /// <returns>The number of rows affected (0 or 1)</returns>
        public async Task<int> AddArtwork(Artwork artworkToAdd)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = 0;
                try
                {
                    await connection.OpenAsync();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@ID", artworkToAdd.ID, direction: ParameterDirection.Output);
                    parameters.Add("@Title", artworkToAdd.Title);
                    parameters.Add("@DateFinished", artworkToAdd.DateFinished);
                    parameters.Add("@Value", artworkToAdd.Value);
                    parameters.Add("@Description", artworkToAdd.Description);
                    parameters.Add("@ArtistID", artworkToAdd.ArtistID);
                    parameters.Add("@ArtTypeID", artworkToAdd.ArtTypeID);

                    affectedRows = await connection.ExecuteAsync(
                        "ArtworkInsert",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    artworkToAdd.ID = parameters.Get<int>("@ID");//Get the new Primary Key Value

                }
                catch (Exception)
                {
                    throw;
                }
                return affectedRows;
            }
        }

        /// <summary>
        /// Update the Artwork identified by the ID
        /// </summary>
        /// <param name="artworkToUpdate"></param>
        /// <returns>The number of rows affected (0 or 1)</returns>
        public async Task<int> UpdateArtwork(Artwork artworkToUpdate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = 0;
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@ID", artworkToUpdate.ID);
                    parameters.Add("@Title", artworkToUpdate.Title);
                    parameters.Add("@DateFinished", artworkToUpdate.DateFinished);
                    parameters.Add("@Value", artworkToUpdate.Value);
                    parameters.Add("@Description", artworkToUpdate.Description);
                    parameters.Add("@ArtistID", artworkToUpdate.ArtistID);
                    parameters.Add("@ArtTypeID", artworkToUpdate.ArtTypeID);
                    parameters.Add("@Timestamp", artworkToUpdate.Timestamp);

                    await connection.OpenAsync();
                    affectedRows = connection.Execute("ArtworkUpdate",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {
                    throw;
                }
                return affectedRows;

                //Note: we could have used the simplified code below for the Execute
                //command IF we didn't have extra properties in our Artwork class
                //that are not in the database entity.  Remember that we also have the extra  
                //lookup values coming in the view so we need to specify which properties to use.
                //
                //var affectedRows = connection.Execute("ArtworkUpdate",
                //    artworkToUpdate,
                //    commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Delete the Artwork identified by ID.
        /// </summary>
        /// <param name="artworkToDelete"></param>
        /// <returns>The number of rows affected (0 or 1)</returns>
        public async Task<int> DeleteArtwork(Artwork artworkToDelete)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = 0;
                await connection.OpenAsync();
                try
                {
                    affectedRows = connection.Execute("ArtworkDelete",
                    new { artworkToDelete.ID },
                    commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {
                    throw;
                }
                return affectedRows;
            }
        }
    }
}
