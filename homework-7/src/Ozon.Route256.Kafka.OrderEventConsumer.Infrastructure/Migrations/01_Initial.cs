using System;
using FluentMigrator;

using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(1, "Initial migration")]
public sealed class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
        CREATE TABLE IF NOT EXISTS item_sales (
              item_id       BIGINT PRIMARY KEY
            , reserved      INT
            , sold          INT
            , canceled      INT
            , modified_at   TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
        );

        CREATE TABLE IF NOT EXISTS payments (
              seller_id       BIGINT PRIMARY KEY
            , rub             DECIMAL(18,2)
            , kzt             DECIMAL(18,2)
        );
";
}
