using FluentMigrator;
using MetricsCommon.SQL_Settings;
using System;

namespace MetricsManager.DAL.Migrations
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
                      .WithColumn(_settingsSQL[ManagerFields.Id]).AsInt64().PrimaryKey().Identity()
                      .WithColumn(_settingsSQL[ManagerFields.AgentId]).AsInt32()
                      .WithColumn(_settingsSQL[ManagerFields.Value]).AsInt32()
                      .WithColumn(_settingsSQL[ManagerFields.Time]).AsInt64();
            }
            Create.Table(SQLSettings.AgentsTable)
                  .WithColumn(_settingsSQL[RegisteredAgentsFeilds.AgentId]).AsInt32().PrimaryKey().Identity()
                  .WithColumn(_settingsSQL[RegisteredAgentsFeilds.AgentUrl]).AsString();
        }
        public override void Down()
        {
            foreach (var table in Enum.GetValues(typeof(Tables)))
            {
                Delete.Table(_settingsSQL[(Tables)table]);
            }
            Delete.Table(SQLSettings.AgentsTable);
        }
    }
}
