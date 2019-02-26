using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    partial class SchemaType
    {
        private static string ProcCompareDbs = "CREATE PROC [dbo].[sp_CompareDb]\n" +
                                              "(\n" +
                                              "	@SourceDB SYSNAME,\n" +
                                              "	@TargetDb SYSNAME\n" +
                                              ")\n" +
                                              "AS\n" +
                                              "BEGIN\n" +
                                              "/*\n" +
                                              "	DECLARE @SourceDB SYSNAME='DB1',@TargetDb SYSNAME='DB2'\n" +
                                              "*/\n" +
                                              "	SET NOCOUNT ON;\n" +
                                              "	SET ANSI_WARNINGS ON;\n" +
                                              "	SET ANSI_NULLS ON;   \n" +
                                              "\n" +
                                              "	DECLARE @sqlStr VARCHAR(8000)\n" +
                                              "	SET @SourceDB = RTRIM(LTRIM(@SourceDB))\n" +
                                              "	IF DB_ID(@SourceDB) IS NULL \n" +
                                              "	BEGIN\n" +
                                              "		PRINT 'Error: Unable to find the database '+ @SourceDB +'!!!'\n" +
                                              "		RETURN\n" +
                                              "	END\n" +
                                              "\n" +
                                              "	SET @TargetDb = RTRIM(LTRIM(@TargetDb))\n" +
                                              "	IF DB_ID(@SourceDB) IS NULL \n" +
                                              "	BEGIN\n" +
                                              "		PRINT 'Error: Unable to find the database '+ @TargetDb +'!!!'\n" +
                                              "		RETURN\n" +
                                              "	END\n" +
                                              "	\n" +
                                              "	PRINT Replicate('-', Len(@SourceDB) + Len(@TargetDb) + 25); \n" +
                                              "	PRINT 'Comparing databases ' + @SourceDB + ' and ' + @TargetDb; \n" +
                                              "	PRINT Replicate('-', Len(@SourceDB) + Len(@TargetDb) + 25);\n" +
                                              "     \n" +
                                              "	----------------------------------------------------------------------------------------- \n" +
                                              "	-- Create temp tables needed to hold the db structure \n" +
                                              "	----------------------------------------------------------------------------------------- 	\n" +
                                              "	\n" +
                                              "	IF OBJECT_ID('TEMPDB..#TABLIST_SOURCE')IS NOT NULL\n" +
                                              "		DROP TABLE #TABLIST_SOURCE;\n" +
                                              "	IF OBJECT_ID('TEMPDB..#TABLIST_TARGET') IS NOT NULL\n" +
                                              "		DROP TABLE #TABLIST_TARGET;\n" +
                                              "	IF OBJECT_ID('TEMPDB..#IDXLIST_SOURCE') IS NOT NULL\n" +
                                              "		DROP TABLE #IDXLIST_SOURCE\n" +
                                              "	IF OBJECT_ID('TEMPDB..#IDXLIST_TARGET') IS NOT NULL\n" +
                                              "		DROP TABLE #IDXLIST_TARGET\n" +
                                              "	IF OBJECT_ID('TEMPDB..#FKLIST_SOURCE') IS NOT NULL\n" +
                                              "		DROP TABLE #FKLIST_SOURCE\n" +
                                              "	IF OBJECT_ID('TEMPDB..#FKLIST_TARGET') IS NOT NULL\n" +
                                              "		DROP TABLE #FKLIST_TARGET\n" +
                                              "	IF OBJECT_ID('TEMPDB..#TAB_RESULTS') IS NOT NULL\n" +
                                              "		DROP TABLE #TAB_RESULTS\n" +
                                              "	IF OBJECT_ID('TEMPDB..#IDX_RESULTS') IS NOT NULL\n" +
                                              "		DROP TABLE #IDX_RESULTS\n" +
                                              "	IF OBJECT_ID('TEMPDB..#FK_RESULTS') IS NOT NULL\n" +
                                              "		DROP TABLE #FK_RESULTS\n" +
                                              "\n" +
                                              "	CREATE TABLE #TABLIST_SOURCE\n" +
                                              "	(\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		TABLENAME SYSNAME,\n" +
                                              "		COLUMNNAME SYSNAME,\n" +
                                              "		DATATYPE SYSNAME,\n" +
                                              "		NULLABLE VARCHAR(15)\n" +
                                              "	)\n" +
                                              "\n" +
                                              "	CREATE TABLE #TABLIST_TARGET\n" +
                                              "	(\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		TABLENAME SYSNAME,\n" +
                                              "		COLUMNNAME SYSNAME,\n" +
                                              "		DATATYPE SYSNAME,\n" +
                                              "		NULLABLE VARCHAR(15)\n" +
                                              "	)\n" +
                                              "\n" +
                                              "	CREATE TABLE #IDXLIST_SOURCE (\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		TABLE_NAME SYSNAME,\n" +
                                              "		IDX_NAME SYSNAME,\n" +
                                              "		IDX_TYPE VARCHAR(20),\n" +
                                              "		IS_PRIMARY_KEY VARCHAR(10),\n" +
                                              "		IS_UNIQUE VARCHAR(10),\n" +
                                              "		IDX_COLUMNS VARCHAR(1000),\n" +
                                              "		IDX_INCLUDED_COLUMNS VARCHAR(1000)\n" +
                                              "	);\n" +
                                              "\n" +
                                              "	CREATE TABLE #IDXLIST_TARGET (\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		TABLE_NAME SYSNAME,\n" +
                                              "		IDX_NAME SYSNAME,\n" +
                                              "		IDX_TYPE VARCHAR(20),\n" +
                                              "		IS_PRIMARY_KEY VARCHAR(10),\n" +
                                              "		IS_UNIQUE VARCHAR(10),\n" +
                                              "		IDX_COLUMNS VARCHAR(1000),\n" +
                                              "		IDX_INCLUDED_COLUMNS VARCHAR(1000)\n" +
                                              "	);\n" +
                                              "\n" +
                                              "	CREATE TABLE #FKLIST_SOURCE (\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		FK_NAME SYSNAME,\n" +
                                              "		FK_TABLE SYSNAME,\n" +
                                              "		FK_COLUMNS VARCHAR(1000),\n" +
                                              "		PK_TABLE SYSNAME,\n" +
                                              "		PK_COLUMNS VARCHAR(1000)\n" +
                                              "	);\n" +
                                              "\n" +
                                              "	CREATE TABLE #FKLIST_TARGET (\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		FK_NAME SYSNAME,\n" +
                                              "		FK_TABLE SYSNAME,\n" +
                                              "		FK_COLUMNS VARCHAR(1000),\n" +
                                              "		PK_TABLE SYSNAME,\n" +
                                              "		PK_COLUMNS VARCHAR(1000)\n" +
                                              "	);\n" +
                                              "\n" +
                                              "	CREATE TABLE #TAB_RESULTS (\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		TABLENAME SYSNAME,\n" +
                                              "		COLUMNNAME SYSNAME,\n" +
                                              "		DATATYPE SYSNAME,\n" +
                                              "		NULLABLE VARCHAR(15),\n" +
                                              "		REASON VARCHAR(150)\n" +
                                              "	);\n" +
                                              "\n" +
                                              "	CREATE TABLE #IDX_RESULTS (\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		TABLE_NAME SYSNAME,\n" +
                                              "		IDX_NAME SYSNAME,\n" +
                                              "		IDX_TYPE VARCHAR(20),\n" +
                                              "		IS_PRIMARY_KEY VARCHAR(10),\n" +
                                              "		IS_UNIQUE VARCHAR(10),\n" +
                                              "		IDX_COLUMNS VARCHAR(1000),\n" +
                                              "		IDX_INCLUDED_COLUMNS VARCHAR(1000),\n" +
                                              "		REASON VARCHAR(150)\n" +
                                              "	);\n" +
                                              "\n" +
                                              "	CREATE TABLE #FK_RESULTS (\n" +
                                              "		ID INT IDENTITY (1, 1),\n" +
                                              "		DATABASENAME SYSNAME,\n" +
                                              "		FK_NAME SYSNAME,\n" +
                                              "		FK_TABLE SYSNAME,\n" +
                                              "		FK_COLUMNS VARCHAR(1000),\n" +
                                              "		PK_TABLE SYSNAME,\n" +
                                              "		PK_COLUMNS VARCHAR(1000),\n" +
                                              "		REASON VARCHAR(150)\n" +
                                              "	);\n" +
                                              "\n" +
                                              "	PRINT 'Getting table and column list!';\n" +
                                              "	PRINT REPLICATE('-', LEN(@SourceDB) + LEN(@TargetDb) + 25);\n" +
                                              "\n" +
                                              "	BEGIN\n" +
                                              "	INSERT INTO #TABLIST_SOURCE (DATABASENAME, TABLENAME, COLUMNNAME, DATATYPE, NULLABLE)\n" +
                                              "	EXEC ('SELECT ''' + @SourceDB + ''', T.TABLE_NAME TABLENAME, \n" +
                                              "				 C.COLUMN_NAME COLUMNNAME,\n" +
                                              "				 TY.name + case when TY.name IN (''char'',''varchar'',''nvarchar'') THEN	\n" +
                                              "					''(''+CASE WHEN C.CHARACTER_MAXIMUM_LENGTH>0 THEN CAST(C.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) ELSE ''max''END+'')''\n" +
                                              "					ELSE	\n" +
                                              "						''''\n" +
                                              "					END\n" +
                                              "					DATATYPE,\n" +
                                              "					CASE WHEN C.is_nullable=''NO'' THEN	\n" +
                                              "						''NOT NULL'' \n" +
                                              "						ELSE\n" +
                                              "						''NULL''\n" +
                                              "					END NULLABLE\n" +
                                              "						FROM ' + @SourceDB + '.INFORMATION_SCHEMA.TABLES T \n" +
                                              "							INNER JOIN  ' + @SourceDB + '.INFORMATION_SCHEMA.COLUMNS C\n" +
                                              "								ON T.TABLE_NAME=C.TABLE_NAME\n" +
                                              "								and T.TABLE_CATALOG=C.TABLE_CATALOG\n" +
                                              "								and T.TABLE_SCHEMA=C.TABLE_SCHEMA\n" +
                                              "							 INNER JOIN ' + @SourceDB + '.sys.types TY\n" +
                                              "							ON C.DATA_TYPE =TY.name		\n" +
                                              "							ORDER BY TABLENAME, COLUMNNAME,C.ORDINAL_POSITION');\n" +
                                              "\n" +
                                              "	INSERT INTO #TABLIST_TARGET (DATABASENAME, TABLENAME, COLUMNNAME, DATATYPE, NULLABLE)\n" +
                                              "	EXEC ('SELECT ''' + @TargetDB + ''', T.TABLE_NAME TABLENAME, \n" +
                                              "				 C.COLUMN_NAME COLUMNNAME,\n" +
                                              "				 TY.name + case when TY.name IN (''char'',''varchar'',''nvarchar'') THEN	\n" +
                                              "					''(''+CASE WHEN C.CHARACTER_MAXIMUM_LENGTH>0 THEN CAST(C.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) ELSE ''max''END+'')''\n" +
                                              "					ELSE	\n" +
                                              "						''''\n" +
                                              "					END\n" +
                                              "					DATATYPE,\n" +
                                              "					CASE WHEN C.is_nullable=''NO'' THEN	\n" +
                                              "						''NOT NULL'' \n" +
                                              "						ELSE\n" +
                                              "						''NULL''\n" +
                                              "					END NULLABLE\n" +
                                              "						FROM ' + @TargetDB + '.INFORMATION_SCHEMA.TABLES T \n" +
                                              "							INNER JOIN  ' + @TargetDB + '.INFORMATION_SCHEMA.COLUMNS C\n" +
                                              "								ON T.TABLE_NAME=C.TABLE_NAME\n" +
                                              "								and T.TABLE_CATALOG=C.TABLE_CATALOG\n" +
                                              "								and T.TABLE_SCHEMA=C.TABLE_SCHEMA\n" +
                                              "							 INNER JOIN ' + @TargetDB + '.sys.types TY\n" +
                                              "							ON C.DATA_TYPE =TY.name		\n" +
                                              "							ORDER BY TABLENAME, COLUMNNAME,C.ORDINAL_POSITION');\n" +
                                              "\n" +
                                              "\n" +
                                              "	PRINT 'Getting index list!';\n" +
                                              "	PRINT REPLICATE('-', LEN(@SourceDB) + LEN(@TargetDb) + 25);\n" +
                                              "\n" +
                                              "	INSERT INTO #IDXLIST_SOURCE (DATABASENAME, TABLE_NAME, IDX_NAME, IDX_TYPE, IS_PRIMARY_KEY, IS_UNIQUE, IDX_COLUMNS, IDX_INCLUDED_COLUMNS)\n" +
                                              "	EXEC ('WITH CTE AS ( \n" +
                                              "						 SELECT      ic.index_id + ic.object_id AS IndexId,t.name AS TableName \n" +
                                              "												 ,i.name AS IndexName\n" +
                                              "												 ,case when ic.is_included_column =0 then\n" +
                                              "														c.name end AS ColumnName\n" +
                                              "												,case when ic.is_included_column =1 then\n" +
                                              "														c.name end AS IncludedColumn\n" +
                                              "														,i.type_desc,ic.key_ordinal \n" +
                                              "												 ,i.is_primary_key,i.is_unique \n" +
                                              "						 FROM  ' + @SourceDB + '.sys.indexes i \n" +
                                              "						 INNER JOIN ' + @SourceDB + '.sys.index_columns ic \n" +
                                              "										 ON  i.index_id    =   ic.index_id \n" +
                                              "										 AND i.object_id   =   ic.object_id \n" +
                                              "						 INNER JOIN ' + @SourceDB + '.sys.columns c \n" +
                                              "										 ON  ic.column_id  =   c.column_id \n" +
                                              "										 AND i.object_id   =   c.object_id \n" +
                                              "						 INNER JOIN (SELECT object_id,name FROM ' + @SourceDB + '.sys.tables  union SELECT object_id,name FROM ' + @SourceDB + '.sys.views)t \n" +
                                              "										 ON  i.object_id = t.object_id \n" +
                                              "	) \n" +
                                              "	SELECT ''' + @SourceDB + ''',c.TableName TABLE_NAME,c.IndexName INDEX_NAME,c.type_desc INDEX_TYPE ,c.is_primary_key IS_PRIMARY_KEY,c.is_unique IS_UNIQUE\n" +
                                              "				 ,STUFF( ( SELECT '',''+ a.ColumnName FROM CTE a WHERE c.IndexId = a.IndexId ORDER BY key_ordinal FOR XML PATH('''')),1 ,1, '''') AS COLUMNS\n" +
                                              "				 ,STUFF( ( SELECT '',''+ a.IncludedColumn FROM CTE a WHERE c.IndexId = a.IndexId ORDER BY key_ordinal,IncludedColumn FOR XML PATH('''')),1 ,1, '''') AS INCLUDED_COLUMNS\n" +
                                              "	FROM   CTE c \n" +
                                              "	GROUP  BY c.IndexId,c.TableName,c.IndexName,c.type_desc,c.is_primary_key,c.is_unique \n" +
                                              "	ORDER  BY c.TableName ASC,c.is_primary_key DESC; ');\n" +
                                              "\n" +
                                              "\n" +
                                              "	INSERT INTO #IDXLIST_TARGET (DATABASENAME, TABLE_NAME, IDX_NAME, IDX_TYPE, IS_PRIMARY_KEY, IS_UNIQUE, IDX_COLUMNS, IDX_INCLUDED_COLUMNS)\n" +
                                              "	EXEC ('WITH CTE AS ( \n" +
                                              "						 SELECT      ic.index_id + ic.object_id AS IndexId,t.name AS TableName \n" +
                                              "												 ,i.name AS IndexName\n" +
                                              "												 ,case when ic.is_included_column =0 then\n" +
                                              "														c.name end AS ColumnName\n" +
                                              "												,case when ic.is_included_column =1 then\n" +
                                              "														c.name end AS IncludedColumn\n" +
                                              "														,i.type_desc \n" +
                                              "												 ,i.is_primary_key,i.is_unique,ic.key_ordinal \n" +
                                              "						 FROM  ' + @TargetDB + '.sys.indexes i \n" +
                                              "						 INNER JOIN ' + @TargetDB + '.sys.index_columns ic \n" +
                                              "										 ON  i.index_id    =   ic.index_id \n" +
                                              "										 AND i.object_id   =   ic.object_id \n" +
                                              "						 INNER JOIN ' + @TargetDB + '.sys.columns c \n" +
                                              "										 ON  ic.column_id  =   c.column_id \n" +
                                              "										 AND i.object_id   =   c.object_id \n" +
                                              "							INNER JOIN (SELECT object_id,name FROM ' + @TargetDB + '.sys.tables  union SELECT object_id,name FROM ' + @TargetDB + '.sys.views)t \n" +
                                              "										 ON  i.object_id = t.object_id \n" +
                                              "	) \n" +
                                              "	SELECT ''' + @TargetDB + ''',c.TableName,c.IndexName,c.type_desc,c.is_primary_key,c.is_unique \n" +
                                              "				 ,STUFF( ( SELECT '',''+ a.ColumnName FROM CTE a WHERE c.IndexId = a.IndexId ORDER BY key_ordinal FOR XML PATH('''')),1 ,1, '''') AS Columns \n" +
                                              "				 ,STUFF( ( SELECT '',''+ a.IncludedColumn FROM CTE a WHERE c.IndexId = a.IndexId ORDER BY key_ordinal,IncludedColumn FOR XML PATH('''')),1 ,1, '''') AS IncludedColumns \n" +
                                              "	FROM   CTE c \n" +
                                              "	GROUP  BY c.IndexId,c.TableName,c.IndexName,c.type_desc,c.is_primary_key,c.is_unique \n" +
                                              "	ORDER  BY c.TableName ASC,c.is_primary_key DESC; ');\n" +
                                              "\n" +
                                              "\n" +
                                              "	PRINT 'Getting foreign key list!';\n" +
                                              "	PRINT REPLICATE('-', LEN(@SourceDB) + LEN(@TargetDb) + 25);\n" +
                                              "\n" +
                                              "	INSERT INTO #FKLIST_SOURCE (DATABASENAME, FK_NAME, FK_TABLE, FK_COLUMNS, PK_TABLE, PK_COLUMNS)\n" +
                                              "	EXEC ('With CTE\n" +
                                              "					AS\n" +
                                              "				(select OBJECT_NAME(FK.parent_object_id,db_id(''' + @SourceDB + ''')) PK_TABLE,	 \n" +
                                              "							C1.name PK_COLUMN,\n" +
                                              "				object_name(FK.referenced_object_id,db_id(''' + @SourceDB + '''))FK_TABLE,\n" +
                                              "				C2.name FK_COLUMN,\n" +
                                              "				FK.name	 FK_NAME\n" +
                                              "	from\n" +
                                              "			' + @SourceDB + '.sys.foreign_keys FK\n" +
                                              "				inner join \n" +
                                              "			' + @SourceDB + '.sys.foreign_key_columns FKC\n" +
                                              "				on FK.object_id=FKC.constraint_object_id\n" +
                                              "				inner join \n" +
                                              "			' + @SourceDB + '.sys.columns C1 \n" +
                                              "				on FKC.parent_column_id=C1.column_id\n" +
                                              "				and FKC.parent_object_id=C1.object_id\n" +
                                              "				inner join \n" +
                                              "			' + @SourceDB + '.sys.columns C2\n" +
                                              "				on FKC.referenced_column_id=C2.column_id\n" +
                                              "				and FKC.referenced_object_id=C2.object_id							\n" +
                                              "		)\n" +
                                              "	SELECT ''' + @SourceDB + ''',C.FK_NAME,\n" +
                                              "				 C.FK_TABLE,			 STUFF( ( SELECT '',''+ A.FK_COLUMN FROM CTE a WHERE c.FK_NAME = a.FK_NAME and C.FK_TABLE=a.FK_TABLE FOR XML PATH('''')),1 ,1, '''') AS FK_COLUMNS,\n" +
                                              "				 C.PK_TABLE,			 			 \n" +
                                              "				 STUFF( ( SELECT '',''+ A.PK_Column FROM CTE a WHERE c.FK_NAME = a.FK_NAME and C.PK_TABLE=a.PK_TABLE FOR XML PATH('''')),1 ,1, '''') AS PK_COLUMNS \n" +
                                              "	FROM CTE C\n" +
                                              "	group by C.FK_NAME,\n" +
                                              "				 C.FK_TABLE,			 \n" +
                                              "				 C.PK_TABLE')\n" +
                                              "\n" +
                                              "	INSERT INTO #FKLIST_TARGET (DATABASENAME, FK_NAME, FK_TABLE, FK_COLUMNS, PK_TABLE, PK_COLUMNS)\n" +
                                              "	EXEC ('\n" +
                                              "			With CTE\n" +
                                              "	AS\n" +
                                              "	(select OBJECT_NAME(FK.parent_object_id,db_id(''' + @TargetDB + ''')) PK_TABLE,	 \n" +
                                              "				C1.name PK_COLUMN,\n" +
                                              "				object_name(FK.referenced_object_id,db_id(''' + @TargetDB + '''))FK_TABLE,\n" +
                                              "				C2.name FK_COLUMN,\n" +
                                              "				FK.name	 FK_NAME\n" +
                                              "	from\n" +
                                              "			' + @TargetDB + '.sys.foreign_keys FK\n" +
                                              "				inner join \n" +
                                              "			' + @TargetDB + '.sys.foreign_key_columns FKC\n" +
                                              "				on FK.object_id=FKC.constraint_object_id\n" +
                                              "				inner join \n" +
                                              "			' + @TargetDB + '.sys.columns C1 \n" +
                                              "				on FKC.parent_column_id=C1.column_id\n" +
                                              "				and FKC.parent_object_id=C1.object_id\n" +
                                              "				inner join \n" +
                                              "			' + @TargetDB + '.sys.columns C2\n" +
                                              "				on FKC.referenced_column_id=C2.column_id\n" +
                                              "				and FKC.referenced_object_id=C2.object_id							\n" +
                                              "		)\n" +
                                              "	SELECT ''' + @TargetDB + ''',C.FK_NAME,\n" +
                                              "				 C.FK_TABLE,			 STUFF( ( SELECT '',''+ A.FK_COLUMN FROM CTE a WHERE c.FK_NAME = a.FK_NAME and C.FK_TABLE=a.FK_TABLE FOR XML PATH('''')),1 ,1, '''') AS FK_COLUMNS,\n" +
                                              "				 C.PK_TABLE,			 			 \n" +
                                              "				 STUFF( ( SELECT '',''+ A.PK_Column FROM CTE a WHERE c.FK_NAME = a.FK_NAME and C.PK_TABLE=a.PK_TABLE FOR XML PATH('''')),1 ,1, '''') AS PK_COLUMNS \n" +
                                              "	FROM CTE C\n" +
                                              "	group by C.FK_NAME,\n" +
                                              "				 C.FK_TABLE,			 \n" +
                                              "				 C.PK_TABLE')\n" +
                                              "	END;\n" +
                                              "\n" +
                                              "	PRINT 'Print column mismatches!';\n" +
                                              "	PRINT REPLICATE('-', LEN(@SourceDB) + LEN(@TargetDb) + 25);\n" +
                                              "\n" +
                                              "	INSERT INTO #TAB_RESULTS (DATABASENAME, TABLENAME, COLUMNNAME, DATATYPE, NULLABLE, REASON)\n" +
                                              "		SELECT\n" +
                                              "			@SourceDB AS DATABASENAME,\n" +
                                              "			TABLENAME,\n" +
                                              "			COLUMNNAME,\n" +
                                              "			DATATYPE,\n" +
                                              "			NULLABLE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				TABLENAME,\n" +
                                              "				COLUMNNAME,\n" +
                                              "				DATATYPE,\n" +
                                              "				NULLABLE\n" +
                                              "			FROM #TABLIST_SOURCE\n" +
                                              "			EXCEPT\n" +
                                              "			SELECT\n" +
                                              "				TS.TABLENAME,\n" +
                                              "				TS.COLUMNNAME,\n" +
                                              "				TS.DATATYPE,\n" +
                                              "				TS.NULLABLE\n" +
                                              "			FROM #TABLIST_SOURCE TS\n" +
                                              "			INNER JOIN #TABLIST_TARGET TT\n" +
                                              "				ON TS.TABLENAME = TT.TABLENAME\n" +
                                              "				AND TS.COLUMNNAME = TT.COLUMNNAME) TAB_NONMATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Missing Column' AS Reason) Tab2\n" +
                                              "		UNION ALL\n" +
                                              "		SELECT\n" +
                                              "			@TargetDb AS DATABASENAME,\n" +
                                              "			TABLENAME,\n" +
                                              "			COLUMNNAME,\n" +
                                              "			DATATYPE,\n" +
                                              "			NULLABLE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				TABLENAME,\n" +
                                              "				COLUMNNAME,\n" +
                                              "				DATATYPE,\n" +
                                              "				NULLABLE\n" +
                                              "			FROM #TABLIST_TARGET\n" +
                                              "			EXCEPT\n" +
                                              "			SELECT\n" +
                                              "				TT.TABLENAME,\n" +
                                              "				TT.COLUMNNAME,\n" +
                                              "				TT.DATATYPE,\n" +
                                              "				TT.NULLABLE\n" +
                                              "			FROM #TABLIST_TARGET TT\n" +
                                              "			INNER JOIN #TABLIST_SOURCE TS\n" +
                                              "				ON TS.TABLENAME = TT.TABLENAME\n" +
                                              "				AND TS.COLUMNNAME = TT.COLUMNNAME) TAB_MATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Missing column ' AS Reason) Tab2\n" +
                                              "\n" +
                                              "	--NON MATCHING COLUMNS\n" +
                                              "	INSERT INTO #TAB_RESULTS (DATABASENAME, TABLENAME, COLUMNNAME, DATATYPE, NULLABLE, REASON)\n" +
                                              "		SELECT\n" +
                                              "			@SourceDB AS DATABASENAME,\n" +
                                              "			TABLENAME,\n" +
                                              "			COLUMNNAME,\n" +
                                              "			DATATYPE,\n" +
                                              "			NULLABLE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				*\n" +
                                              "			FROM (SELECT\n" +
                                              "					TS.TABLENAME,\n" +
                                              "					TS.COLUMNNAME,\n" +
                                              "					TS.DATATYPE,\n" +
                                              "					TS.NULLABLE\n" +
                                              "				FROM #TABLIST_SOURCE TS\n" +
                                              "				INNER JOIN #TABLIST_TARGET TT\n" +
                                              "					ON TS.TABLENAME = TT.TABLENAME\n" +
                                              "					AND TS.COLUMNNAME = TT.COLUMNNAME) T\n" +
                                              "			EXCEPT\n" +
                                              "			(SELECT\n" +
                                              "				TABLENAME,\n" +
                                              "				COLUMNNAME,\n" +
                                              "				DATATYPE,\n" +
                                              "				NULLABLE\n" +
                                              "			FROM #TABLIST_SOURCE\n" +
                                              "			INTERSECT\n" +
                                              "			SELECT\n" +
                                              "				TABLENAME,\n" +
                                              "				COLUMNNAME,\n" +
                                              "				DATATYPE,\n" +
                                              "				NULLABLE\n" +
                                              "			FROM #TABLIST_TARGET)) TT1\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Definition not matching' AS REASON) t\n" +
                                              "\n" +
                                              "		UNION ALL\n" +
                                              "\n" +
                                              "		SELECT\n" +
                                              "			@TargetDb AS DATABASENAME,\n" +
                                              "			TABLENAME,\n" +
                                              "			COLUMNNAME,\n" +
                                              "			DATATYPE,\n" +
                                              "			NULLABLE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				*\n" +
                                              "			FROM (SELECT\n" +
                                              "					TT.TABLENAME,\n" +
                                              "					TT.COLUMNNAME,\n" +
                                              "					TT.DATATYPE,\n" +
                                              "					TT.NULLABLE\n" +
                                              "				FROM #TABLIST_TARGET TT\n" +
                                              "				INNER JOIN #TABLIST_SOURCE TS\n" +
                                              "					ON TS.TABLENAME = TT.TABLENAME\n" +
                                              "					AND TS.COLUMNNAME = TT.COLUMNNAME) T\n" +
                                              "			EXCEPT\n" +
                                              "			(SELECT\n" +
                                              "				TABLENAME,\n" +
                                              "				COLUMNNAME,\n" +
                                              "				DATATYPE,\n" +
                                              "				NULLABLE\n" +
                                              "			FROM #TABLIST_TARGET\n" +
                                              "			INTERSECT\n" +
                                              "			SELECT\n" +
                                              "				TABLENAME,\n" +
                                              "				COLUMNNAME,\n" +
                                              "				DATATYPE,\n" +
                                              "				NULLABLE\n" +
                                              "			FROM #TABLIST_SOURCE)) TAB_NONMATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Definition not matching' AS REASON) T;\n" +
                                              "\n" +
                                              "	PRINT 'Print index mismatches!';\n" +
                                              "	PRINT REPLICATE('-', LEN(@SourceDB) + LEN(@TargetDb) + 25);\n" +
                                              "\n" +
                                              "\n" +
                                              "	INSERT INTO #IDX_RESULTS (DATABASENAME, TABLE_NAME, IDX_NAME, IDX_COLUMNS, IDX_INCLUDED_COLUMNS, IS_PRIMARY_KEY, IS_UNIQUE, REASON)\n" +
                                              "		SELECT\n" +
                                              "			@SourceDB AS DATABASENAME,\n" +
                                              "			TABLE_NAME,\n" +
                                              "			IDX_NAME,\n" +
                                              "			IDX_COLUMNS,\n" +
                                              "			IDX_INCLUDED_COLUMNS,\n" +
                                              "			IS_PRIMARY_KEY,\n" +
                                              "			IS_UNIQUE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				TABLE_NAME,\n" +
                                              "				IDX_NAME,\n" +
                                              "				IDX_COLUMNS,\n" +
                                              "				IDX_INCLUDED_COLUMNS,\n" +
                                              "				IS_PRIMARY_KEY,\n" +
                                              "				IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_SOURCE\n" +
                                              "			EXCEPT\n" +
                                              "			SELECT\n" +
                                              "				TS.TABLE_NAME,\n" +
                                              "				TS.IDX_NAME,\n" +
                                              "				TS.IDX_COLUMNS,\n" +
                                              "				TS.IDX_INCLUDED_COLUMNS,\n" +
                                              "				TS.IS_PRIMARY_KEY,\n" +
                                              "				TS.IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_SOURCE TS\n" +
                                              "			INNER JOIN #IDXLIST_TARGET TT\n" +
                                              "				ON TS.TABLE_NAME = TT.TABLE_NAME\n" +
                                              "				AND TS.IDX_NAME = TT.IDX_NAME) TAB_NONMATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Missing Index n' AS Reason) Tab2\n" +
                                              "		UNION ALL\n" +
                                              "		SELECT\n" +
                                              "			@TargetDb AS DATABASENAME,\n" +
                                              "			TABLE_NAME,\n" +
                                              "			IDX_NAME,\n" +
                                              "			IDX_COLUMNS,\n" +
                                              "			IDX_INCLUDED_COLUMNS,\n" +
                                              "			IS_PRIMARY_KEY,\n" +
                                              "			IS_UNIQUE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				TABLE_NAME,\n" +
                                              "				IDX_NAME,\n" +
                                              "				IDX_COLUMNS,\n" +
                                              "				IDX_INCLUDED_COLUMNS,\n" +
                                              "				IS_PRIMARY_KEY,\n" +
                                              "				IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_TARGET\n" +
                                              "			EXCEPT\n" +
                                              "			SELECT\n" +
                                              "				TT.TABLE_NAME,\n" +
                                              "				TT.IDX_NAME,\n" +
                                              "				TT.IDX_COLUMNS,\n" +
                                              "				TT.IDX_INCLUDED_COLUMNS,\n" +
                                              "				TT.IS_PRIMARY_KEY,\n" +
                                              "				TT.IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_TARGET TT\n" +
                                              "			INNER JOIN #IDXLIST_SOURCE TS\n" +
                                              "				ON TS.TABLE_NAME = TT.TABLE_NAME\n" +
                                              "				AND TS.IDX_NAME = TT.IDX_NAME) TAB_MATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Missing index ' AS Reason) Tab2\n" +
                                              "\n" +
                                              "	--NON MATCHING INDEX\n" +
                                              "	INSERT INTO #IDX_RESULTS (DATABASENAME, TABLE_NAME, IDX_NAME, IDX_COLUMNS, IDX_INCLUDED_COLUMNS, IS_PRIMARY_KEY, IS_UNIQUE, REASON)\n" +
                                              "		SELECT\n" +
                                              "			@SourceDB AS DATABASENAME,\n" +
                                              "			TABLE_NAME,\n" +
                                              "			IDX_NAME,\n" +
                                              "			IDX_COLUMNS,\n" +
                                              "			IDX_INCLUDED_COLUMNS,\n" +
                                              "			IS_PRIMARY_KEY,\n" +
                                              "			IS_UNIQUE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				*\n" +
                                              "			FROM (SELECT\n" +
                                              "					TS.TABLE_NAME,\n" +
                                              "					TS.IDX_NAME,\n" +
                                              "					TS.IDX_COLUMNS,\n" +
                                              "					TS.IDX_INCLUDED_COLUMNS,\n" +
                                              "					TS.IS_PRIMARY_KEY,\n" +
                                              "					TS.IS_UNIQUE\n" +
                                              "				FROM #IDXLIST_SOURCE TS\n" +
                                              "				INNER JOIN #IDXLIST_TARGET TT\n" +
                                              "					ON TS.TABLE_NAME = TT.TABLE_NAME\n" +
                                              "					AND TS.IDX_NAME = TT.IDX_NAME) T\n" +
                                              "			EXCEPT\n" +
                                              "			(SELECT\n" +
                                              "				TABLE_NAME,\n" +
                                              "				IDX_NAME,\n" +
                                              "				IDX_COLUMNS,\n" +
                                              "				IDX_INCLUDED_COLUMNS,\n" +
                                              "				IS_PRIMARY_KEY,\n" +
                                              "				IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_SOURCE\n" +
                                              "			INTERSECT\n" +
                                              "			SELECT\n" +
                                              "				TABLE_NAME,\n" +
                                              "				IDX_NAME,\n" +
                                              "				IDX_COLUMNS,\n" +
                                              "				IDX_INCLUDED_COLUMNS,\n" +
                                              "				IS_PRIMARY_KEY,\n" +
                                              "				IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_TARGET)) TT1\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Definition not matching' AS REASON) t\n" +
                                              "\n" +
                                              "		UNION ALL\n" +
                                              "\n" +
                                              "		SELECT\n" +
                                              "			@TargetDb AS DATABASENAME,\n" +
                                              "			TABLE_NAME,\n" +
                                              "			IDX_NAME,\n" +
                                              "			IDX_COLUMNS,\n" +
                                              "			IDX_INCLUDED_COLUMNS,\n" +
                                              "			IS_PRIMARY_KEY,\n" +
                                              "			IS_UNIQUE,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				*\n" +
                                              "			FROM (SELECT\n" +
                                              "					TT.TABLE_NAME,\n" +
                                              "					TT.IDX_NAME,\n" +
                                              "					TT.IDX_COLUMNS,\n" +
                                              "					TT.IDX_INCLUDED_COLUMNS,\n" +
                                              "					TT.IS_PRIMARY_KEY,\n" +
                                              "					TT.IS_UNIQUE\n" +
                                              "				FROM #IDXLIST_TARGET TT\n" +
                                              "				INNER JOIN #IDXLIST_SOURCE TS\n" +
                                              "					ON TS.TABLE_NAME = TT.TABLE_NAME\n" +
                                              "					AND TS.IDX_NAME = TT.IDX_NAME) T\n" +
                                              "			EXCEPT\n" +
                                              "			(SELECT\n" +
                                              "				TABLE_NAME,\n" +
                                              "				IDX_NAME,\n" +
                                              "				IDX_COLUMNS,\n" +
                                              "				IDX_INCLUDED_COLUMNS,\n" +
                                              "				IS_PRIMARY_KEY,\n" +
                                              "				IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_TARGET\n" +
                                              "			INTERSECT\n" +
                                              "			SELECT\n" +
                                              "				TABLE_NAME,\n" +
                                              "				IDX_NAME,\n" +
                                              "				IDX_COLUMNS,\n" +
                                              "				IDX_INCLUDED_COLUMNS,\n" +
                                              "				IS_PRIMARY_KEY,\n" +
                                              "				IS_UNIQUE\n" +
                                              "			FROM #IDXLIST_SOURCE)) TAB_NONMATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Definition not matching' AS REASON) T;\n" +
                                              "\n" +
                                              "\n" +
                                              "	PRINT 'Print key mismatches!';\n" +
                                              "	PRINT REPLICATE('-', LEN(@SourceDB) + LEN(@TargetDb) + 25);\n" +
                                              "\n" +
                                              "	INSERT INTO #FK_RESULTS (DATABASENAME, FK_NAME, FK_TABLE, FK_COLUMNS, PK_TABLE, PK_COLUMNS, REASON)\n" +
                                              "		SELECT\n" +
                                              "			@SourceDB AS DATABASENAME,\n" +
                                              "			FK_NAME,\n" +
                                              "			FK_TABLE,\n" +
                                              "			FK_COLUMNS,\n" +
                                              "			PK_TABLE,\n" +
                                              "			PK_COLUMNS,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				FK_NAME,\n" +
                                              "				FK_TABLE,\n" +
                                              "				FK_COLUMNS,\n" +
                                              "				PK_TABLE,\n" +
                                              "				PK_COLUMNS\n" +
                                              "			FROM #FKLIST_SOURCE\n" +
                                              "			EXCEPT\n" +
                                              "			SELECT\n" +
                                              "				TS.FK_NAME,\n" +
                                              "				TS.FK_TABLE,\n" +
                                              "				TS.FK_COLUMNS,\n" +
                                              "				TS.PK_TABLE,\n" +
                                              "				TS.PK_COLUMNS\n" +
                                              "			FROM #FKLIST_SOURCE TS\n" +
                                              "			INNER JOIN #FKLIST_TARGET TT\n" +
                                              "				ON TS.FK_NAME = TT.FK_NAME) TAB_NONMATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Missing Index n' AS Reason) Tab2\n" +
                                              "\n" +
                                              "		UNION ALL\n" +
                                              "\n" +
                                              "		SELECT\n" +
                                              "			@TargetDb AS DATABASENAME,\n" +
                                              "			FK_NAME,\n" +
                                              "			FK_TABLE,\n" +
                                              "			FK_COLUMNS,\n" +
                                              "			PK_TABLE,\n" +
                                              "			PK_COLUMNS,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				FK_NAME,\n" +
                                              "				FK_TABLE,\n" +
                                              "				FK_COLUMNS,\n" +
                                              "				PK_TABLE,\n" +
                                              "				PK_COLUMNS\n" +
                                              "			FROM #FKLIST_TARGET\n" +
                                              "			EXCEPT\n" +
                                              "			SELECT\n" +
                                              "				TT.FK_NAME,\n" +
                                              "				TT.FK_TABLE,\n" +
                                              "				TT.FK_COLUMNS,\n" +
                                              "				TT.PK_TABLE,\n" +
                                              "				TT.PK_COLUMNS\n" +
                                              "			FROM #FKLIST_TARGET TT\n" +
                                              "			INNER JOIN #FKLIST_SOURCE TS\n" +
                                              "				ON TS.FK_NAME = TT.FK_NAME) TAB_MATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Missing key' AS Reason) Tab2\n" +
                                              "\n" +
                                              "\n" +
                                              "	--NON MATCHING Keys\n" +
                                              "	INSERT INTO #FK_RESULTS (DATABASENAME, FK_NAME, FK_TABLE, FK_COLUMNS, PK_TABLE, PK_COLUMNS, REASON)\n" +
                                              "		SELECT\n" +
                                              "			@SourceDB AS DATABASENAME,\n" +
                                              "			FK_NAME,\n" +
                                              "			FK_TABLE,\n" +
                                              "			FK_COLUMNS,\n" +
                                              "			PK_TABLE,\n" +
                                              "			PK_COLUMNS,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				*\n" +
                                              "			FROM (SELECT\n" +
                                              "					TS.FK_NAME,\n" +
                                              "					TS.FK_TABLE,\n" +
                                              "					TS.FK_COLUMNS,\n" +
                                              "					TS.PK_TABLE,\n" +
                                              "					TS.PK_COLUMNS\n" +
                                              "				FROM #FKLIST_SOURCE TS\n" +
                                              "				INNER JOIN #FKLIST_TARGET TT\n" +
                                              "					ON TS.FK_NAME = TT.FK_NAME) T\n" +
                                              "			EXCEPT\n" +
                                              "			(SELECT\n" +
                                              "				FK_NAME,\n" +
                                              "				FK_TABLE,\n" +
                                              "				FK_COLUMNS,\n" +
                                              "				PK_TABLE,\n" +
                                              "				PK_COLUMNS\n" +
                                              "			FROM #FKLIST_SOURCE\n" +
                                              "			INTERSECT\n" +
                                              "			SELECT\n" +
                                              "				FK_NAME,\n" +
                                              "				FK_TABLE,\n" +
                                              "				FK_COLUMNS,\n" +
                                              "				PK_TABLE,\n" +
                                              "				PK_COLUMNS\n" +
                                              "			FROM #FKLIST_TARGET)) TT1\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Definition not matching' AS REASON) t\n" +
                                              "\n" +
                                              "		UNION ALL\n" +
                                              "\n" +
                                              "		SELECT\n" +
                                              "			@TargetDb AS DATABASENAME,\n" +
                                              "			FK_NAME,\n" +
                                              "			FK_TABLE,\n" +
                                              "			FK_COLUMNS,\n" +
                                              "			PK_TABLE,\n" +
                                              "			PK_COLUMNS,\n" +
                                              "			REASON\n" +
                                              "		FROM (SELECT\n" +
                                              "				*\n" +
                                              "			FROM (SELECT\n" +
                                              "					TT.FK_NAME,\n" +
                                              "					TT.FK_TABLE,\n" +
                                              "					TT.FK_COLUMNS,\n" +
                                              "					TT.PK_TABLE,\n" +
                                              "					TT.PK_COLUMNS\n" +
                                              "				FROM #FKLIST_TARGET TT\n" +
                                              "				INNER JOIN #FKLIST_SOURCE TS\n" +
                                              "					ON TS.FK_NAME = TT.FK_NAME) T\n" +
                                              "			EXCEPT\n" +
                                              "			(SELECT\n" +
                                              "				FK_NAME,\n" +
                                              "				FK_TABLE,\n" +
                                              "				FK_COLUMNS,\n" +
                                              "				PK_TABLE,\n" +
                                              "				PK_COLUMNS\n" +
                                              "			FROM #FKLIST_TARGET\n" +
                                              "			INTERSECT\n" +
                                              "			SELECT\n" +
                                              "				FK_NAME,\n" +
                                              "				FK_TABLE,\n" +
                                              "				FK_COLUMNS,\n" +
                                              "				PK_TABLE,\n" +
                                              "				PK_COLUMNS\n" +
                                              "			FROM #FKLIST_SOURCE)) TAB_NONMATCH\n" +
                                              "		CROSS JOIN (SELECT\n" +
                                              "				'Definition not matching' AS REASON) T;\n" +
                                              "\n" +
                                              "	--Print Final Results	\n" +
                                              "\n" +
                                              "	SELECT\n" +
                                              "		*\n" +
                                              "	FROM #TAB_RESULTS\n" +
                                              "	SELECT\n" +
                                              "		*\n" +
                                              "	FROM #IDX_RESULTS\n" +
                                              "	SELECT\n" +
                                              "		*\n" +
                                              "	FROM #FK_RESULTS\n" +
                                              "END";
    }
}
