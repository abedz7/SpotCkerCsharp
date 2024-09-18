using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServerSide.Models;

namespace ServerSide.DBinteractions
{
    public class ParkingLotsDB
    {
        private readonly string _sqlConnectionStr;

        public ParkingLotsDB(string connectionString)
        {
            _sqlConnectionStr = connectionString;
        }

        private static readonly string allParkingLotsQuery = "SELECT * FROM [ParkingLots]";
        private static readonly string parkingLotByIdQuery = "SELECT * FROM [ParkingLots] WHERE Id = @Id";
        private static readonly string insertParkingLotQuery = "INSERT INTO [ParkingLots] (Id, Name, Longitude, Latitude) VALUES (@Id, @Name, @Longitude, @Latitude)";
        private static readonly string updateParkingLotQuery = "UPDATE [ParkingLots] SET Name = @Name, Longitude = @Longitude, Latitude = @Latitude WHERE Id = @Id";
        private static readonly string deleteParkingLotQuery = "DELETE FROM [ParkingLots] WHERE Id = @Id";
        private static readonly string parkingLotByNameQuery = "SELECT * FROM [ParkingLots] WHERE Name = @Name";

        // Get all parking lots
        public List<ParkingLot> GetAllParkingLots()
        {
            List<ParkingLot> parkingLots = new List<ParkingLot>();

            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(allParkingLotsQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ParkingLot parkingLot = new ParkingLot
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Longitude = Convert.ToDecimal(reader["Longitude"]),
                        Latitude = Convert.ToDecimal(reader["Latitude"])
                    };
                    parkingLots.Add(parkingLot);
                }
                reader.Close();
            }

            return parkingLots;
        }

        // Get parking lot by ID
        public ParkingLot GetParkingLotById(string id)
        {
            ParkingLot parkingLot = null;

            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(parkingLotByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    parkingLot = new ParkingLot
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Longitude = Convert.ToDecimal(reader["Longitude"]),
                        Latitude = Convert.ToDecimal(reader["Latitude"])
                    };
                }
                reader.Close();
            }

            return parkingLot;
        }

        // Add a new parking lot
        public string AddParkingLot(ParkingLot parkingLot)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertParkingLotQuery, connection);

                string newId = Guid.NewGuid().ToString();

                command.Parameters.AddWithValue("@Id", newId);  
                command.Parameters.AddWithValue("@Name", parkingLot.Name);
                command.Parameters.AddWithValue("@Longitude", parkingLot.Longitude);
                command.Parameters.AddWithValue("@Latitude", parkingLot.Latitude);

                connection.Open();
                command.ExecuteNonQuery();

                return newId;
            }
        }

        // Update a parking lot by ID
        public int UpdateParkingLot(ParkingLot parkingLot)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateParkingLotQuery, connection);
                command.Parameters.AddWithValue("@Id", parkingLot.Id);
                command.Parameters.AddWithValue("@Name", parkingLot.Name);
                command.Parameters.AddWithValue("@Longitude", parkingLot.Longitude);
                command.Parameters.AddWithValue("@Latitude", parkingLot.Latitude);

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        // Delete a parking lot by ID
        public bool DeleteParkingLot(string id)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteParkingLotQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // Get parking lot by name
        public ParkingLot GetParkingLotByName(string name)
        {
            ParkingLot parkingLot = null;

            using (SqlConnection connection = new SqlConnection(_sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(parkingLotByNameQuery, connection);
                command.Parameters.AddWithValue("@Name", name);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    parkingLot = new ParkingLot
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Longitude = Convert.ToDecimal(reader["Longitude"]),
                        Latitude = Convert.ToDecimal(reader["Latitude"])
                    };
                }
                reader.Close();
            }

            return parkingLot;
        }
    }
}
