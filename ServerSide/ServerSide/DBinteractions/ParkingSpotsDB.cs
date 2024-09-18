using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServerSide.Models;

namespace ServerSide.DBinteractions
{
    public class ParkingSpotsDB
    {
        private readonly string _sqlConnectionStr;

        public ParkingSpotsDB(string connectionString)
        {
            _sqlConnectionStr = connectionString;
        }

        private static readonly string insertParkingSpotQuery = "INSERT INTO ParkingSpots (Id, ParkingLotId, SpotNumber, SpotType, ReservedByUserId, IsAvailable) VALUES (@Id, @ParkingLotId, @SpotNumber, @SpotType, @ReservedByUserId, @IsAvailable)";
        private static readonly string getParkingSpotsByLotIdQuery = "SELECT * FROM ParkingSpots WHERE ParkingLotId = @ParkingLotId";
        private static readonly string getAllParkingSpotsQuery = "SELECT * FROM ParkingSpots";
        private static readonly string updateParkingSpotQuery = "UPDATE ParkingSpots SET SpotType = @SpotType, ReservedByUserId = @ReservedByUserId, IsAvailable = @IsAvailable WHERE Id = @Id";
        private static readonly string deleteParkingSpotQuery = "DELETE FROM ParkingSpots WHERE Id = @Id";

        // Get all parking spots
        public List<ParkingSpot> GetAllParkingSpots()
        {
            List<ParkingSpot> spots = new List<ParkingSpot>();

            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(getAllParkingSpotsQuery, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ParkingSpot spot = new ParkingSpot
                    {
                        Id = reader["Id"].ToString(),
                        ParkingLotId = reader["ParkingLotId"].ToString(),
                        SpotNumber = Convert.ToInt32(reader["SpotNumber"]),
                        SpotType = reader["SpotType"].ToString(),
                        ReservedByUserId = reader["ReservedByUserId"] != DBNull.Value ? reader["ReservedByUserId"].ToString() : null,
                        IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                    };
                    spots.Add(spot);
                }
                reader.Close();
            }

            return spots;
        }

        // Add a new parking spot
        public void AddParkingSpot(ParkingSpot spot)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertParkingSpotQuery, connection);
                command.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@ParkingLotId", spot.ParkingLotId);
                command.Parameters.AddWithValue("@SpotNumber", spot.SpotNumber);
                command.Parameters.AddWithValue("@SpotType", spot.SpotType);
                command.Parameters.AddWithValue("@ReservedByUserId", spot.ReservedByUserId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsAvailable", spot.IsAvailable);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Remove a parking spot
        public bool RemoveParkingSpot(string id)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteParkingSpotQuery, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // Get parking spots by lot ID
        public List<ParkingSpot> GetParkingSpotsByLotId(string lotId)
        {
            List<ParkingSpot> spots = new List<ParkingSpot>();

            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(getParkingSpotsByLotIdQuery, connection);
                command.Parameters.AddWithValue("@ParkingLotId", lotId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ParkingSpot spot = new ParkingSpot
                    {
                        Id = reader["Id"].ToString(),
                        ParkingLotId = reader["ParkingLotId"].ToString(),
                        SpotNumber = Convert.ToInt32(reader["SpotNumber"]),
                        SpotType = reader["SpotType"].ToString(),
                        ReservedByUserId = reader["ReservedByUserId"] != DBNull.Value ? reader["ReservedByUserId"].ToString() : null,
                        IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                    };
                    spots.Add(spot);
                }
                reader.Close();
            }
            return spots;
        }

        // Update a parking spot
        public bool UpdateParkingSpot(string id, ParkingSpot updatedSpot)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateParkingSpotQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@SpotType", updatedSpot.SpotType);
                command.Parameters.AddWithValue("@ReservedByUserId", updatedSpot.ReservedByUserId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsAvailable", updatedSpot.IsAvailable);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
