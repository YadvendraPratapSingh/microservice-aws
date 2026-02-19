AWS Console Tasks
-------------------
1. Create ECR Repository for each services
2. Create IAM user and custom policy to Give (List,Read, Write)permission to user for ECR repository
3. Create Credetial file on local machine Home directory to create profile for IAM user

4.  Run script on local machine to build and push images to ECR repository

5. Create cluster in Amazon ECS 
6. Create VPC with Subnets
7. Create a new task definition file in ECS using fargate

8. Create Postgress Database in RDS, may be we need to enable DNS Hostnames in VPC
9. Connect RDS Postgress database with PGAdmin (postgress client tool) 
10. Create user in RDS postgress database using PGAdmin query console
    ## CREATE USER weather_stage WITH password 'changeme!';
	
	# or may be generate idempotent sql scripts 
	- dotnet ef migrations script --idempotent -o 000_init_tempdb.sql
	
	- CREATE database cloud_weather_temperature;
	
	- run created migration script (000_init_tempdb.sql) command on cloud_weather_temperature database using Query console.
	
  - Do same this for each service
11. GRANT ALL PRIVILEGES ON DATABASE cloud_weather_temperature TO weather_stage; 
    GRANT ALL PRIVILEGES ON DATABASE cloud_weather_precipitation TO weather_stage; 
	  GRANT ALL PRIVILEGES ON DATABASE cloud_weather_report TO weather_stage; 
	
12. In task definition file, while createing container create environment variable
    1. CONNECTIONSTRINGS__APPDB
	2. ASPNETCORE_URLS with value http://+:5000
	
	- for report service we have different Environment variable
	1. WEATHERCONFIG__PRECIPDATAHOST
	2. WEATHERCONFIG__PRECIPDATAPROTOCOL
	3. WEATHERCONFIG__PRECIPDATAPORT
	4. WEATHERCONFIG__TEMPDATAHOST
	5. WEATHERCONFIG__TEMPDATAPROTOCOL
	6. WEATHERCONFIG__TEMPDATAPORT
	7. ASPNETCORE_URLS = http://+
	
	
	# select grantee and priviledge_type for temperature table 
	SELECT grantee, privilege_type
	FROM information_schema.role_table_grants
	WHERE table_name='temperature'
	
	# run this query t each table to grant permission
	
	# to assign priviledge_type to specific table 
	
	GRANT SELECT, INSERT, UPDATE, DELETE
	ON ALL TABLES in SCHEMA public 
	TO weather_stage;
	
    
ISSUES Encountered while deploying services to Cloud
- appSetting.json file name is different in code, case sensitive issue (capital case is used)
- by mistake added a space in begining while providing host value to HOST environment variable
- SecurityGroup inbound rule has wrong configuration, provided apecific IpAddress
- While inserting data in table in postgress database having table permission issue for user
- Connection refused error for Report service, the reason is we forget to add environment variable for connection string

