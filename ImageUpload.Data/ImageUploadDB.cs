using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageUpload.Data
{
    public class ImageUploadDB
    {
        private string _connectionString;

        public ImageUploadDB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Add(Image image)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Images(FileName, Password, ViewCount, UserId) " +
                                  "VALUES (@FileName, @Password, 0, @UserId) SELECT SCOPE_IDENTITY() ";
                cmd.Parameters.AddWithValue("@FileName", image.FileName);
                cmd.Parameters.AddWithValue("@Password", image.Password);
                cmd.Parameters.AddWithValue("@UserId", image.UserId);
                connection.Open();
                image.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }
        public Image GetImagebyId(int Id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Select Top 1 * from Images where Id=@id";
                cmd.Parameters.AddWithValue("@id", Id);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new Image

                {
                    Id = (int)reader["Id"],
                    FileName = (string)reader["FileName"],
                    Password = (string)reader["Password"],
                    ViewCount = (int)reader["ViewCount"]
                };
            }
        }
        public IEnumerable<Image> GetAllImages()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images";
                connection.Open();
                var reader = cmd.ExecuteReader();
                var result = new List<Image>();
                while (reader.Read())
                {
                    result.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Password = (string)reader["Password"],
                        ViewCount = (int)reader["ViewCount"],
                    });

                }
                return result;
            }
        }


        public void UpdateViewCount(int Id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE Images SET ViewCount=ViewCount + 1 WHERE Id = @id;";
                cmd.Parameters.AddWithValue("@id", Id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddUser(User user, string password)
        {
            string passwordSalt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, passwordSalt);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash, PasswordSalt) " +
                                  "VALUES (@name, @email, @passwordHash, @passwordSalt)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                cmd.Parameters.AddWithValue("@passwordSalt", passwordSalt);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public User GetByEmail(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return new User
                {
                    Email = email,
                    Name = (string)reader["Name"],
                    Id = (int)reader["Id"],
                    PasswordHash = (string)reader["PasswordHash"],
                    PasswordSalt = (string)reader["PasswordSalt"],
                };
            }
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (isCorrectPassword)
            {
                return user;
            }

            return null;
        }
        public IEnumerable<Image> GetImagesByUserId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images WHERE UserId = @userId";
                cmd.Parameters.AddWithValue("@userId", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                var result = new List<Image>();
                while (reader.Read())
                {
                    result.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Password = (string)reader["Password"],
                        ViewCount = (int)reader["ViewCount"],
                        UserId = (int)reader["UserId"]
                    });

                }
                return result;
            }
        }

        public void DeleteImage(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Images WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}



