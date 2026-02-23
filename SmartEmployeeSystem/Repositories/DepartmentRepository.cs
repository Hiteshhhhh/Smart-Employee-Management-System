using Npgsql;
using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly NpgsqlConnection conn;

        public DepartmentRepository(IConfiguration config)
        {
            conn = new NpgsqlConnection(
                config.GetConnectionString("DefaultConnection"));
        }

        public List<DepartmentModel> GetDepartments()
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();
            try
            {
                conn.Open();
                string query = "SELECT * FROM departments";
                var cmd = new NpgsqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new DepartmentModel
                    {
                        department_id   = reader.GetInt32(0),
                        department_name = reader.GetString(1),
                        description     = reader.GetString(2),
                        is_active       = reader.GetBoolean(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return departments;
        }

        public DepartmentModel GetDepartmentById(int id)
        {
            DepartmentModel department = null;
            try
            {
                conn.Open();
                string query = "SELECT * FROM departments WHERE department_id = @id";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    department = new DepartmentModel
                    {
                        department_id   = reader.GetInt32(0),
                        department_name = reader.GetString(1),
                        description     = reader.GetString(2),
                        is_active       = reader.GetBoolean(3)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return department;
        }

        public void AddDepartment(DepartmentModel department)
        {
            try
            {
                conn.Open();
                string query = @"INSERT INTO departments 
                                (department_name, description, is_active) 
                                VALUES (@n, @d, true)";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@n", department.department_name);
                cmd.Parameters.AddWithValue("@d", department.description);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateDepartment(DepartmentModel department)
        {
            try
            {
                conn.Open();
                string query = @"UPDATE departments 
                                SET department_name = @n, 
                                    description = @d,
                                    is_active = @a
                                WHERE department_id = @id";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@n", department.department_name);
                cmd.Parameters.AddWithValue("@d", department.description);
                cmd.Parameters.AddWithValue("@a", department.is_active);
                cmd.Parameters.AddWithValue("@id", department.department_id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void DeleteDepartment(int id)
        {
            try
            {
                conn.Open();
                string query = @"DELETE FROM departments 
                                WHERE department_id = @id";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}