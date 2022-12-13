using SibersDatabase.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SibersDatabase.Data
{
    public class MainDB
    {
        public readonly SQLiteAsyncConnection db;

        public Repository<Employee> EmployeesTableMethods;
        public Repository<Project> ProjectsTableMethods;
        public Repository<EmployeesInProject> EmployeesInProjectsTableMethods;

        public MainDB(string pathToDb)
        {
            db = new SQLiteAsyncConnection(pathToDb);

            db.CreateTableAsync<Employee>().Wait();
            db.CreateTableAsync<Project>().Wait();
            db.CreateTableAsync<EmployeesInProject>().Wait();

            EmployeesTableMethods = new Repository<Employee>(db);
            ProjectsTableMethods = new Repository<Project>(db);
            EmployeesInProjectsTableMethods = new Repository<EmployeesInProject>(db);
        }

        interface IRepository<T> where T : class, new()
        {
            Task<List<T>> GetAsync();
            Task<T> GetAsync(int id);
            Task<List<T>> GetAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null);
            Task<int> InsertAsync(T entity);
            Task<int> UpdateAsync(T entity);
            Task<int> DeleteAsync<T>(int primaryKey);
            Task<int> DeleteAllAsync(Expression<Func<T, bool>> predicate);
        }

        public class Repository<T> : IRepository<T> where T : class, new()
        {
            private readonly SQLiteAsyncConnection db;

            public Repository(SQLiteAsyncConnection db)
            {
                this.db = db;
            }

            public async Task<List<T>> GetAsync() =>
                await db.Table<T>().ToListAsync();

            public async Task<T> GetAsync(int id) =>
                 await db.FindAsync<T>(id);

            public async Task<List<T>> GetAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
            {
                var tableValues = db.Table<T>();

                if (predicate != null)
                    tableValues = tableValues.Where(predicate);

                if (orderBy != null)
                    tableValues = tableValues.OrderBy(orderBy);

                return await tableValues.ToListAsync();
            }

            public async Task<int> InsertAsync(T entity) =>
                 await db.InsertAsync(entity);

            public async Task<int> UpdateAsync(T entity) =>
                 await db.UpdateAsync(entity);

            public async Task<int> DeleteAsync<T>(int primaryKey) =>
                 await db.DeleteAsync<T>(primaryKey);

            public async Task<int> DeleteAllAsync(Expression<Func<T, bool>> predicate)
            {
                var entitiesToDelete = db.Table<T>().Where(predicate) as IEnumerable<List<T>>;
                if (entitiesToDelete == null) return 0;
                int count = 0;
                foreach (var entity in entitiesToDelete)
                    count += await db.DeleteAsync<T>(entity);
                return count;
            }
        }
    }
}