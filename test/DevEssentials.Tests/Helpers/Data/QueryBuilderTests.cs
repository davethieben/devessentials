using Essentials.Data;
using Xunit;

namespace Essentials.Test.Helpers.Data
{
    public class QueryBuilderTests
    {

        [Fact]
        public void GeneratesSimpleQuery()
        {
            var builder = new QueryBuilder(from: "Users");
            var runner = builder.Run();
            string query = runner.ToString();

            Assert.Matches(@"SELECT\s+\*\s+FROM\s+Users", query);
        }
        
        [Fact]
        public void GeneratesQueryWithFields()
        {
            var builder = new QueryBuilder(from: "Users");
            builder.Select("FirstName", "LastName");

            var runner = builder.Run();
            string query = runner.ToString();

            Assert.Matches(@"SELECT\s+FirstName,\s+LastName\s+FROM\s+Users", query);
        }
        
        [Fact]
        public void GeneratesQueryWithJoins()
        {
            var builder = new QueryBuilder(from: "Users");
            builder.Select("FirstName", "LastName");
            builder.InnerJoin("UserRoles");

            var runner = builder.Run();
            string query = runner.ToString();

            Assert.Matches(@"FROM\s+Users\s+INNER JOIN\s+UserRoles", query);
        }
        
        [Fact]
        public void GeneratesQueryWithConditions()
        {
            var builder = new QueryBuilder(from: "Users");
            builder.Select("FirstName", "LastName");

            var runner = builder.Run();
            runner.Where("UserName = @username", new { username = "tester" });

            string query = runner.ToString();

            Assert.Matches(@"\s+FROM\s+Users\s+WHERE[ (]+UserName = @username[) ]", query);
        }

    }
}
