﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "COST_AMT",
                table: "SL_TRANSACTION",
                type: "decimal(2)",
                precision: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");


            migrationBuilder.Sql(@"
CREATE VIEW sl_year_stats_data_vw
AS
SELECT x.user_id
	  ,x.show_name
	  ,YEAR(x.date_watched) year
	  ,x.show_type_id
	  ,CASE WHEN x.show_type_id = 1000 THEN SUM(ei.runtime)
			ELSE SUM(mi.runtime)
	   END total_runtime
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_type
			ELSE mi.api_type
	   END api_type
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_id
			ELSE mi.api_id
	   END api_id
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.backdrop_url
			ELSE mi.backdrop_url
	   END backdrop_url
	  ,COUNT(*) watch_count
FROM   sl_show x
LEFT OUTER JOIN sl_tv_episode_info ei
	ON (ei.tv_episode_info_id = x.info_id)
LEFT OUTER JOIN sl_tv_info ti
	ON (ti.tv_info_id = ei.tv_info_id)
LEFT OUTER JOIN sl_movie_info mi
	ON (mi.movie_info_id = x.info_id)
GROUP BY x.user_id
	  ,x.show_name
	  ,YEAR(x.date_watched)
	  ,x.show_type_id
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_type
			ELSE mi.api_type
	   END
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_id
			ELSE mi.api_id
	   END
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.backdrop_url
			ELSE mi.backdrop_url
	   END
;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "COST_AMT",
                table: "SL_TRANSACTION",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2)",
                oldPrecision: 2);

            migrationBuilder.Sql(@"
DROP VIEW sl_year_stats_data_vw;
");
        }
    }
}
