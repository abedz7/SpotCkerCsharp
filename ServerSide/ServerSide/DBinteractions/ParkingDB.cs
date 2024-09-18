using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ServerSide.Models;

namespace ServerSide.DBinteractions
{
    public class ParkingDB
    {
        private readonly string _sqlConnectionStr;

        // Inject IConfiguration to access appsettings.json
        public ParkingDB(IConfiguration configuration)
        {
            _sqlConnectionStr = configuration.GetConnectionString("DefaultConnection");
        }

        // SQL Queries
        private static readonly string insertParkingQuery = "INSERT INTO Parkings (Id, SpotId, UserId, StartDate, EndDate, StartTime, EndTime) VALUES (@Id, @SpotId, @UserId, @StartDate, @EndDate, @StartTime, @EndTime)";
        private static readonly string getParkingByIdQuery = "SELECT * FROM Parkings WHERE Id = @Id";
        private static readonly string updateParkingQuery = "UPDATE Parkings SET SpotId = @SpotId, UserId = @UserId, StartDate = @StartDate, EndDate = @EndDate, StartTime = @StartTime, EndTime = @EndTime WHERE Id = @Id";
        private static readonly string deleteParkingQuery = "DELETE FROM Parkings WHERE Id = @Id";
        private static readonly string getParkingsByUserIdQuery = "SELECT * FROM Parkings WHERE UserId = @UserId";

        // Add a new parking record
        public void AddParking(Parking parking)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertParkingQuery, connection);
                command.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@SpotId", parking.SpotId);
                command.Parameters.AddWithValue("@UserId", parking.UserId);
                command.Parameters.AddWithValue("@StartDate", parking.StartDate);
                command.Parameters.AddWithValue("@EndDate", parking.EndDate);
                command.Parameters.AddWithValue("@StartTime", TimeSpan.Parse(parking.StartTime)); 
                command.Parameters.AddWithValue("@EndTime", TimeSpan.Parse(parking.EndTime)); 

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Get parking record by ID
        public Parking GetParkingById(string id)
        {
            Parking parking = null;

            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(getParkingByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    parking = new Parking
                    {
                        Id = reader["Id"].ToString(),
                        SpotId = reader["SpotId"].ToString(),
                        UserId = reader["UserId"].ToString(),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        StartTime = ((TimeSpan)reader["StartTime"]).ToString(@"hh\:mm"), 
                        EndTime = ((TimeSpan)reader["EndTime"]).ToString(@"hh\:mm") 
                    };
                }
                reader.Close();
            }

            return parking;
        }

        // Update a parking record by ID
        public int UpdateParking(Parking parking)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateParkingQuery, connection);
                command.Parameters.AddWithValue("@Id", parking.Id);
                command.Parameters.AddWithValue("@SpotId", parking.SpotId);
                command.Parameters.AddWithValue("@UserId", parking.UserId);
                command.Parameters.AddWithValue("@StartDate", parking.StartDate);
                command.Parameters.AddWithValue("@EndDate", parking.EndDate);
                command.Parameters.AddWithValue("@StartTime", TimeSpan.Parse(parking.StartTime)); 
                command.Parameters.AddWithValue("@EndTime", TimeSpan.Parse(parking.EndTime)); 

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        // Remove a parking record by ID
        public bool DeleteParking(string id)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteParkingQuery, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // Get all parking records by User ID
        public List<Parking> GetParkingsByUserId(string userId)
        {
            List<Parking> parkings = new List<Parking>();

            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(getParkingsByUserIdQuery, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Parking parking = new Parking
                    {
                        Id = reader["Id"].ToString(),
                        SpotId = reader["SpotId"].ToString(),
                        UserId = reader["UserId"].ToString(),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        StartTime = ((TimeSpan)reader["StartTime"]).ToString(@"hh\:mm"),
                        EndTime = ((TimeSpan)reader["EndTime"]).ToString(@"hh\:mm")
                    };
                    parkings.Add(parking);
                }
                reader.Close();
            }

            return parkings;
        }
    }
}
