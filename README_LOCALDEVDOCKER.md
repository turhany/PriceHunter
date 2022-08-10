#### For local Development - Docker Run Codes
* **MongoDB >**  docker run -d -p 27017:27017 --name mongocontainer mongo:latest
* **Redis >** docker run --name redis -p 6379:6379 -d redis --requirepass 123456789.tY
* **RabbitMQ >** docker run -d --hostname rabbitmq --name rabbitmq -p 15672:15672 -p 5672:5672 rabbitmq:3.10.1-management 
    * Default username and password: guest