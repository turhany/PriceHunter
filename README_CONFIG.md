#### Before Usage
* If you want to use local environment, you need to update Redis and Mongo connection strings in API, ScheduleService, Consumers project  (appsettings.json)    
    * **RedisConnectionString** field for DistributedCache
    * **Mongo Settings (MongoDBOption)**
        * ConnectionString
        * Database
    * **Distributed Lock Settings (RedLockSettings)**
        * RedLockHostAddress
        * RedLockHostPort
        * RedLockHostPassword > if you dont have pass you need to set it null
        * RedLockHostSsl  
    * **Message Queue Settings (RabbitMqSettings)**
        * HostAddress
        * UserName
        * Password
        * Also need to add your queue names here like "ParserQueue" and in C# "RabbitMqOption" class for option mapping
    * Also project has ready to run **Docker support**
        * **docker-compose file store in solution directory**
        * All connection string stores in appsettings.DockerCompose.json environment file
