using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using ServerSide.Models;
using ServerSide.Utilities;

namespace ServerSide.DBinteractions
{
    public class UsersDB
    {
        private static string sqlConnectionStr;

        public static void SetConnectionString(string connectionString)
        {
            sqlConnectionStr = connectionString;
        }

        private static readonly string allUsersQuery = "SELECT * FROM [Users]";
        private static readonly string userByIdQuery = "SELECT * FROM [Users] WHERE Id = @Id";
        private static readonly string insertUserQuery = "INSERT INTO [Users] (FirstName, LastName, EmailAddress, PhoneNumber, Password, IsAdmin, HasDisabledCertificate, IsMom) OUTPUT INSERTED.Id VALUES (@FirstName, @LastName, @EmailAddress, @PhoneNumber, @Password, @IsAdmin, @HasDisabledCertificate, @IsMom)";
        private static readonly string updateUserQuery = "UPDATE [Users] SET FirstName = @FirstName, LastName = @LastName, EmailAddress = @EmailAddress, PhoneNumber = @PhoneNumber, Password = @Password, IsAdmin = @IsAdmin, HasDisabledCertificate = @HasDisabledCertificate, IsMom = @IsMom WHERE Id = @Id";
        private static readonly string deleteUserQuery = "DELETE FROM [Users] WHERE Id = @Id";

        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(allUsersQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User
                    {
                        Id = reader["Id"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        EmailAddress = reader["EmailAddress"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Password = reader["Password"].ToString(),
                        IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                        HasDisabledCertificate = reader["HasDisabledCertificate"] != DBNull.Value && Convert.ToBoolean(reader["HasDisabledCertificate"]),
                        IsMom = reader["IsMom"] != DBNull.Value && Convert.ToBoolean(reader["IsMom"]),
                        Cars = new List<Car>()
                    };
                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }

        public static User GetUserById(string id)
        {
            User user = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(userByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader["Id"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        EmailAddress = reader["EmailAddress"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Password = reader["Password"].ToString(),
                        IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                        HasDisabledCertificate = reader["HasDisabledCertificate"] != DBNull.Value && Convert.ToBoolean(reader["HasDisabledCertificate"]),
                        IsMom = reader["IsMom"] != DBNull.Value && Convert.ToBoolean(reader["IsMom"]),
                        Cars = new List<Car>()
                    };
                }
                reader.Close();
            }

            return user;
        }

        public static string InsertUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertUserQuery, connection);
                string hashedPassword = PasswordUtilities.HashPassword(user.Password);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                command.Parameters.AddWithValue("@HasDisabledCertificate", user.HasDisabledCertificate);
                command.Parameters.AddWithValue("@IsMom", user.IsMom);

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        public static int UpdateUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateUserQuery, connection);
                string hashedPassword = PasswordUtilities.HashPassword(user.Password);
                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                command.Parameters.AddWithValue("@HasDisabledCertificate", user.HasDisabledCertificate);
                command.Parameters.AddWithValue("@IsMom", user.IsMom);

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static bool DeleteUser(string id)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteUserQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public static int UpdateUserPassword(string id, string newPassword)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                string query = "UPDATE [Users] SET Password = @Password WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                string hashedPassword = PasswordUtilities.HashPassword(newPassword);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static User AuthenticateUser(string email, string password)
        {
            User user = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                string query = "SELECT * FROM [Users] WHERE EmailAddress = @EmailAddress";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmailAddress", email);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string storedPasswordHash = reader["Password"].ToString();
                    bool passwordValid = PasswordUtilities.VerifyPassword(password, storedPasswordHash);

                    if (passwordValid)
                    {
                        user = new User
                        {
                            Id = reader["Id"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            EmailAddress = reader["EmailAddress"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            Password = storedPasswordHash,
                            IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                            HasDisabledCertificate = reader["HasDisabledCertificate"] != DBNull.Value && Convert.ToBoolean(reader["HasDisabledCertificate"]),
                            IsMom = reader["IsMom"] != DBNull.Value && Convert.ToBoolean(reader["IsMom"]),
                            Cars = new List<Car>()
                        };
                    }
                }
                reader.Close();
            }

            return user;
        }
        public static User GetUserByEmail(string email)
        {
            User user = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                string query = "SELECT * FROM [Users] WHERE EmailAddress = @EmailAddress";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmailAddress", email);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader["Id"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        EmailAddress = reader["EmailAddress"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Password = reader["Password"].ToString(),
                        IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                        HasDisabledCertificate = reader["HasDisabledCertificate"] != DBNull.Value && Convert.ToBoolean(reader["HasDisabledCertificate"]),
                        IsMom = reader["IsMom"] != DBNull.Value && Convert.ToBoolean(reader["IsMom"]),
                        Cars = new List<Car>()
                    };
                }
                reader.Close();
            }

            return user;
        }

        public static void HandleForgotPassword(string emailAddress)
        {
            User user = GetUserByEmail(emailAddress);
            if (user == null)
                throw new Exception("Email address not found.");

            string tempPassword = GenerateTemporaryPassword();
            string hashedPassword = PasswordUtilities.HashPassword(tempPassword);
            UpdateUserPassword(user.Id, hashedPassword);
            SendEmail(user.EmailAddress, tempPassword);
        }

        private static string GenerateTemporaryPassword()
        {
            var options = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            char[] tempPassword = new char[8];
            for (int i = 0; i < tempPassword.Length; i++)
            {
                tempPassword[i] = options[random.Next(options.Length)];
            }
            return new string(tempPassword);
        }

        private static void SendEmail(string email, string tempPassword)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("spotseeker8@gmail.com", "mtbi wxhq tbve pjtn"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("spotseeker8@gmail.com"),
                Subject = "SpotCker Temporary Password",
                Body = $"Your temporary password is: {tempPassword}. Please log in and change your password.",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }
    }
}
