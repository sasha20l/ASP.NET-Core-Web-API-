using FluentMigrator;
using MetricsCommon.SQL_Settings;
using System;

namespace MetricsAgent.DTO.Migrations
{    
    [Migration(1)]
    public class FirstMigration : Migration
    {
        private readonly ISQLSettings _settingsSQL;
        public FirstMigration(ISQLSettings settingsSQL)
        {
            _settingsSQL = settingsSQL;
        }
        public override void Up()
        {
            foreach (var table in Enum.GetValues(typeof(Tables)))
            {
                Create.Table(_settingsSQL[(Tables)table])
                      .WithColumn(_settingsSQL[AgentFields.Id]).AsInt64().PrimaryKey().Identity()
                      .WithColumn(_settingsSQL[AgentFields.Value]).AsInt32()
                      .WithColumn(_settingsSQL[AgentFields.Time]).AsInt64();
            }
        }
        public override void Down()
        {
            foreach (var table in Enum.GetValues(typeof(Tables)))
            {
                Delete.Table(_settingsSQL[(Tables)table]);
            }
        }
    }
}
