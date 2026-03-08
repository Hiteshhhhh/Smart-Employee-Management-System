

using Microsoft.AspNetCore.Identity;
using Npgsql;
using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _conn;
        private readonly NpgsqlConnection connection;
        private readonly IHttpContextAccessor access;

        public UserRepository(IConfiguration config, IHttpContextAccessor accessor)
        {
            _conn = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is missing in configuration.");
            connection = new NpgsqlConnection(_conn);
            access = accessor;
        }

        public void AddUser(UserModel user)
        {
            try
            {
                var hasher = new PasswordHasher<UserModel>();
                user.password_hash = hasher.HashPassword(user, user.password_hash ?? string.Empty);
                connection.Open();
                string query = @"insert into users(username,password_hash,email,role,is_active,created_at) values (@u,@p,@e,'Employee',true,@c)";
                var cmd = new NpgsqlCommand(query,connection);
                cmd.Parameters.AddWithValue("@u",user.username ?? string.Empty);
                cmd.Parameters.AddWithValue("@p",user.password_hash ?? string.Empty);
                cmd.Parameters.AddWithValue("@e",user.email ?? string.Empty);
                cmd.Parameters.AddWithValue("@c",DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        public bool IsEmailExist(string email)
        {
            try
            {
                connection.Open();
                string query = @"select * from users where email = @e";
                var cmd = new NpgsqlCommand(query,connection);
                cmd.Parameters.AddWithValue("@e",email);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

       public bool Login(UserModel user)
        {
            try
            {
                connection.Open();
                string query = @"SELECT user_id, username, email, password_hash, role 
                                FROM public.users 
                                WHERE email = @e AND is_active = true";
                var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@e", user.email ?? string.Empty);
                var rows = cmd.ExecuteReader();
                if (rows.Read())
                {
                    string? storedHash = rows["password_hash"] as string;
                    if (string.IsNullOrWhiteSpace(storedHash))
                    {
                        return false;
                    }

                    if (VerifyPassword(storedHash, user.password_hash ?? string.Empty))
                    {
                        var session = access.HttpContext?.Session;
                        if (session is null)
                        {
                            return false;
                        }

                        session.SetInt32("userid", rows.GetInt32(0));
                        session.SetString("username", rows.GetString(1));
                        session.SetString("useremail", rows.GetString(2));
                        session.SetString("userrole", rows.GetString(4));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

        public bool VerifyPassword(string storedHash, string providedPassword)
        {
            var passwordHasher = new PasswordHasher<string>();
            var result = passwordHasher.VerifyHashedPassword(string.Empty, storedHash, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}