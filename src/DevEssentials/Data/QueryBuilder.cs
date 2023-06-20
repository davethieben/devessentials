using System.Data;
using System.Dynamic;
using System.Text;
using Essentials.Reflection;

namespace Essentials.Data
{
    public interface IQueryMapper<TEntity, TChild>
    {
        TEntity Map(TEntity entity, TChild child);
    }

    public interface IQueryMapper<TEntity, TChild1, TChild2>
    {
        TEntity Map(TEntity entity, TChild1 child1, TChild2 child2);
    }

    public class QueryBuilder
    {
        private readonly List<object> _parameters = new();

        public QueryBuilder(string? from = null)
        {
            if (null != from && !string.IsNullOrEmpty(from))
                Joins.Add(from);
        }

        public List<string> Fields { get; } = new();

        public List<string> Joins { get; } = new();

        public List<string> Conditions { get; } = new();

        public List<string> Orders { get; } = new();

        public bool OptionPaging { get; set; }

        public bool OptionRecompile { get; set; }

        public TimeSpan? CommandTimeout { get; set; }

        public QueryBuilder Select(params string[] fields)
        {
            if (fields.IsNullOrEmpty()) throw new ArgumentNullException(nameof(fields));

            Fields.AddRange(fields);
            return this;
        }

        public QueryBuilder From(string source)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));

            Joins.Add(source);
            return this;
        }

        public QueryBuilder InnerJoin(string source, string? alias = null, string? on = null)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));

            if (!string.IsNullOrEmpty(on))
                on = $"ON {on}";

            Joins.Add($"INNER JOIN {source} {alias} {on}");
            return this;
        }

        public QueryBuilder OuterJoin(string source, string? alias = null, string? on = null)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));

            if (!string.IsNullOrEmpty(on))
                on = $"ON {on}";

            Joins.Add($"LEFT OUTER JOIN {source} {alias} {on}");
            return this;
        }

        public QueryBuilder Where(string condition, object? parameters = null)
        {
            if (string.IsNullOrEmpty(condition)) throw new ArgumentNullException(nameof(condition));

            if (condition.ToLower().StartsWith("where "))
                condition = condition.Substring(6);

            Conditions.Add(condition);

            if (parameters != null)
                _parameters.Add(parameters);

            return this;
        }

        public QueryBuilder ForPage(int offset, int limit)
        {
            OptionPaging = true;
            _parameters.Add(new { offset, limit });

            return this;
        }

        public QueryBuilder AddParameter(object values)
        {
            _parameters.Add(values);
            return this;
        }

        public object GetParameters()
        {
            object dynparameters = new ExpandoObject();
            foreach (var value in _parameters)
            {
                dynparameters = dynparameters.Merge(value);
            }

            return dynparameters;
        }

        public QueryBuilder OrderBy(string order)
        {
            if (string.IsNullOrEmpty(order)) throw new ArgumentNullException(nameof(order));
            Orders.Add(order);
            return this;
        }

        private QueryBuilder Clone()
        {
            var clone = new QueryBuilder
            {
                OptionRecompile = OptionRecompile,
                OptionPaging = OptionPaging,
                CommandTimeout = CommandTimeout
            };

            clone.Fields.AddRange(Fields);
            clone.Joins.AddRange(Joins);
            clone.Conditions.AddRange(Conditions);
            clone._parameters.AddRange(_parameters);
            clone.Orders.AddRange(Orders);

            return clone;
        }

        public QueryRunner Run()
            => new QueryRunner(Clone());

        public QueryRunner<TEntity, TChild> Run<TEntity, TChild>() where TEntity : IQueryMapper<TEntity, TChild>
            => new QueryRunner<TEntity, TChild>(Clone());

        public QueryRunner<TEntity, TChild1, TChild2> Run<TEntity, TChild1, TChild2>() where TEntity : IQueryMapper<TEntity, TChild1, TChild2>
            => new QueryRunner<TEntity, TChild1, TChild2>(Clone());

    }

    public class QueryRunner
    {
        private readonly QueryBuilder _builder;

        internal QueryRunner(QueryBuilder builder)
        {
            _builder = builder;
        }

        public TimeSpan? CommandTimeout => _builder.CommandTimeout;

        public QueryRunner Where(string condition, object? parameters = null)
        {
            if (string.IsNullOrEmpty(condition)) throw new ArgumentNullException(nameof(condition));

            if (condition.ToLower().StartsWith("where "))
                condition = condition.Substring(6);

            _builder.Where(condition, parameters);
            return this;
        }

        public QueryRunner ForPage(int offset, int limit)
        {
            _builder.ForPage(offset, limit);
            return this;
        }

        public QueryRunner AddParameter(object values)
        {
            _builder.AddParameter(values);
            return this;
        }

        public object GetParameters() => _builder.GetParameters();

        public QueryRunner OrderBy(string order)
        {
            _builder.OrderBy(order);
            return this;
        }

        public override string ToString()
        {
            StringBuilder query = new();
            query.Append("SELECT ");

            if (_builder.Fields.Any())
            {
                query.AppendLine(string.Join(", ", _builder.Fields));
            }
            else
            {
                query.AppendLine(" * ");
            }
            while (query[query.Length - 1] == ',')
                query.Remove(query.Length - 1, 1);

            if (_builder.Joins.Any())
            {
                query.Append(" FROM ")
                    .AppendLine(string.Join(Environment.NewLine, _builder.Joins));
            }

            if (_builder.Conditions.Any())
            {
                query.Append(" WHERE ")
                    .AppendLine(string.Join(Environment.NewLine + " AND ",
                        _builder.Conditions.Select(clause => $"({clause})")));
            }

            if (_builder.Orders.Any())
            {
                query.Append(" ORDER BY ")
                    .AppendLine(string.Join(", ", _builder.Orders));
            }

            if (_builder.OptionPaging)
                query.AppendLine(" OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY");

            if (_builder.OptionRecompile)
                query.AppendLine(" OPTION(RECOMPILE)");

            return query.ToString();
        }

        // for Dapper integration:
        //public static implicit operator CommandDefinition(QueryRunner runner) =>
        //    new CommandDefinition(
        //        commandText: runner.ToString(),
        //        parameters: runner.GetParameters(),
        //        commandTimeout: (int?)runner.CommandTimeout?.TotalSeconds);

    }

    public class QueryRunner<TEntity, TChild> : QueryRunner
        where TEntity : IQueryMapper<TEntity, TChild>
    {
        public QueryRunner(QueryBuilder builder) : base(builder)
        {
        }

        public List<string> SplitOnFields { get; } = new();

        public QueryRunner<TEntity, TChild> SplitOn(params string[] fields)
        {
            SplitOnFields.AddRange(fields.EmptyIfNull().SelectMany(str => str.Split(',')));
            return this;
        }
    }

    public class QueryRunner<TEntity, TChild1, TChild2> : QueryRunner
        where TEntity : IQueryMapper<TEntity, TChild1, TChild2>
    {
        public QueryRunner(QueryBuilder builder) : base(builder)
        {
        }

        public List<string> SplitOnFields { get; } = new();

        public QueryRunner<TEntity, TChild1, TChild2> SplitOn(params string[] fields)
        {
            SplitOnFields.AddRange(fields.EmptyIfNull().SelectMany(str => str.Split(',')));
            return this;
        }

    }

    public static class QueryBuilderExtensions
    {
        /* Dapper integration:
        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TChild>(this IDbConnection conn, QueryRunner<TEntity, TChild> query)
            where TEntity : IQueryMapper<TEntity, TChild>
        {
            string splitOn = "Id";
            if (query.SplitOnFields.Any())
                splitOn = string.Join(",", query.SplitOnFields);

            return await conn.QueryAsync<TEntity, TChild, TEntity>(sql: query.ToString(),
                map: (entity, child) => entity.Map(entity, child),
                param: query.GetParameters(),
                splitOn: splitOn,
                commandTimeout: (int?)query.CommandTimeout?.TotalSeconds);
        }

        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TChild1, TChild2>(this IDbConnection conn, QueryRunner<TEntity, TChild1, TChild2> query)
            where TEntity : IQueryMapper<TEntity, TChild1, TChild2>
        {
            string splitOn = "Id";
            if (query.SplitOnFields.Any())
                splitOn = string.Join(",", query.SplitOnFields);

            return await conn.QueryAsync<TEntity, TChild1, TChild2, TEntity>(sql: query.ToString(),
                map: (entity, child1, child2) => entity.Map(entity, child1, child2),
                param: query.GetParameters(),
                splitOn: splitOn,
                commandTimeout: (int?)query.CommandTimeout?.TotalSeconds);
        }
        */

    }

}
