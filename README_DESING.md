<!-- Define initial Architecture -->
## Architecture 

PriceHunter is full scalable, microservice application.

## System Design

![alt tag](files/images/system_diagram.png) 

## Database Diagram

https://dbdiagram.io/d/62ea4d95f31da965e86f8388

![alt tag](files/images/database_diagram.png) 

## Project Diagram

![alt tag](files/images/project_diagram.png) 

#### Structure
- **WEB:** Blazor based ui project  
- **API:** Endpoint project for client usage  
- **Business:** Project for business logic        
- **Common:** Cross cutting consern items (like cache, lock...)   
- **Container:** DI configuration project   
- **Contract:** Dtos, layer transfer and api response - request objects   
- **Data:** Database layer files (Mongo implementations and repositories)    
- **Model:** Database entity models   
- **Resources:** Language resx files project   
- **Messaging/Consumer:** Message service(RabbitMQ) implementation project
- **ScheduleService:** Schedule tasks project   
- **Tests:** Unit test project

## Comon interfaces of interservice comunications
Will be add.


<!-- Define list of components to implement -->
## System Components
#### Gateway
This is the entry point of the system. Request life cycle strat from here. Its manage all requests.

#### Client App
This is client/admin app which user can manage all functionalty. For example can add product, can see price details belong to product.

#### Web Api
This is the layer all bussiness logic works.

#### Database
The place where data stored.

#### Cache
The place where data cached.

#### Message Queue
The place where messages stored.

#### Notification Consumer
The app which send users email, sms and push notifications.

#### Parser Consumer
The app which parse product web pages for the get and save price info.

#### Background Worker
The app run background jobs, for example prepare notification for send and put them in releated queue.

### Technologies
* .Net 6
* Mongo DB for database
* Blazor for Front end
* RabbitMQ for queue(Publisher/Consumer system) 
* Email/SMS/Push Notification system 
* Redis for distributed cache and lock
* Hangfire for background services
* Ocelot for gateway system

<!-- ## Define all api endpoints and their schema -->
#### Generic Base Response Type
```cs 
BaseResponse{
    public object Data { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public List<string> ValidationMessages { get; set; }
}
``` 

#### Generic Search Request Type
```cs 
 public class FilterRequest
{
    public List<FilterItem> AndFilters { get; set; } = new List<FilterItem>();
    public List<FilterItem> OrFilters { get; set; } = new List<FilterItem>();

    public Dictionary<string, OrderOperation> OrderOperations { get; set; } = new Dictionary<string, OrderOperation>();

    public int PageNumber { get; set; } 
    public int PageSize { get; set; }
}

public class FilterItem
{
    public object Value { get; set; } 
    public string TargetFieldName { get; set; } 
    public FilterOperation Operation { get; set; }
}

public enum FilterOperation
{
    Equal,
    NotEqual,
    Contains,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    StartsWith,
    EndsWith
}
``` 

#### Generic Search Response Type
```cs 
public class PagedList<T>
{
    public List<T> Data { get; set; }
    public Page PageInfo { get; set; }
}

public class Page
{
    public int TotalItemCount { get;  set; }
    public int PageSize { get;  set; }
    public int PageNumber { get;  set; }
    public int TotalPageCount => (int)Math.Ceiling(TotalItemCount / (double)PageSize);
}
``` 

#### Standart Types
```cs
public enum SupplierType{
    None = 0,
    Amazon = 1,
    Alibaba = 2,
    AliExpress = 3
}
```
 
#### GetToken > {url}/login/token - POST
```cs
//Request
public class GetTokenContract
{
    public string Email { get; set; }
    public string Password { get; set; }
}

//Response
public class AccessTokenContract
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int? ExpiresIn { get; set; }
    public DateTime RefreshTokenExpireDate { get; set; }
}
``` 
#### GetToken > {url}/login/refresh-token - POST
```cs
//Request
public class RefreshTokenContract
{
    public string Token { get; set; }
}

//Response
public class AccessTokenContract
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int? ExpiresIn { get; set; }
    public DateTime RefreshTokenExpireDate { get; set; }
}
``` 

#### Create User > {url}/users - POST
```cs
//Request
public class CreateUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public string Email { get; set; }
    public string Password { get; set; }
}
``` 
#### Read/Get User > {url}/users/{id} - GET
```cs
//Response model in BaseResponse > Data
public class UserViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Image { get; set; }
    public string Email { get; set; }
    public string Type { get; set; }
}
``` 
#### Update User > {url}/users/{id} - PUT
```cs
//Request
public class UpdateUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserType Type { get; set; }
    public bool IsActive { get; set; }
}
```
#### Update User > {url}/users/uploadprofileimage/{id} - POST
```cs
//Request
public class ProfileFileContractServiceRequest
{
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
}
```
#### Search User > {url}/users/search - POST
Use generic filter request - response data type
```cs
//Response model in generic filter response
public class UserViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Image { get; set; }
    public string Email { get; set; }
    public string Type { get; set; }
}
``` 
#### Delete User > {url}/users/{id} - DELETE
No need extra info. Return "<b>BaseResponse</b>" model.

#### Create User Product > {url}/userproducts - POST
```cs
//Request
public class CreateUserProductRequest
{
    public string Name { get; set; }
    public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; }
}

public class UrlSupplierMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
#### Read/Get User Product> {url}/userproducts/{id} - GET
```cs
//Response model in BaseResponse > Data
public class UserProductViewModel
{
    public string Name { get; set; }
    public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewModel>();
}

public class UrlSupplierMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
#### Update User Product > {url}/userproducts/{id} - PUT
```cs
//Request
public class UpdateUserProductRequest
{
    public string Name { get; set; } 
    public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; }
}

public class UrlSupplierMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
#### Get User Product Last 6 Month changes> {url}/userproducts/last6monthchanges/{id} - GET
Use generic filter request - response data type
```cs
//Response model in BaseResponse > Data
public class ProductPriceChangesViewModel
{
    public double Price { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}
``` 
#### Delete User Product > {url}/userproducts/{id} - DELETE
No need extra info. Return "<b>BaseResponse</b>" model.

#### Create Product > {url}/products - POST
```cs
//Request
 public class CreateProductRequest
{
    public string Name { get; set; }
    public List<ProductSupplierInfoMappingViewModel> UrlSupplierMapping { get; set; }
}

public class ProductSupplierInfoMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
#### Read/Get Product > {url}/products/{id} - GET
```cs
//Response model in BaseResponse > Data
public class ProductViewModel
{
    public string Name { get; set; }
    public List<ProductSupplierInfoMappingViewModel> UrlSupplierMapping { get; set; } = new List<ProductSupplierInfoMappingViewModel>();
}

public class ProductSupplierInfoMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
#### Update Product > {url}/products/{id} - PUT
```cs
//Request
 public class UpdateProductRequest
{
    public string Name { get; set; }
    public List<ProductSupplierInfoMappingViewModel> UrlSupplierMapping { get; set; } 
}

public class ProductSupplierInfoMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
```
#### Search Product > {url}/products/search - POST
Use generic filter request - response data type
```cs
//Response model in generic filter response
public class ProductSearchViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }        
}
``` 
#### Search Product Price History> {url}/products/pricehistory/search - POST
Use generic filter request - response data type
```cs
//Response model in generic filter response
public class ProductPriceHistorySearchViewModel
{
    public Guid ProductId { get; set; }
    public Guid SupplierId { get; set; }
    public double Price { get; set; }
}
``` 
#### Delete Product > {url}/products/{id} - DELETE
No need extra info. Return "<b>BaseResponse</b>" model.
#### Get Product Last 6 Month changes> {url}/products/last6monthchanges/{id} - GET
Use generic filter request - response data type
```cs
//Response model in BaseResponse > Data
public class ProductPriceChangesViewModel
{
    public double Price { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}
``` 

#### Read/Get Product > {url}/suppliers/all - GET
```cs
//Response model in BaseResponse > Data
public class SupplierViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
``` 

## Define all consumers and their interfaces
#### Price parser consumer event model
```cs
public class SendParserCommand
{
    public Guid ProductId { get; set; }
    public Guid SupplierId { get; set; }
    public string Url { get; set; }
    public int EnumMapping { get; set; }
    public DateTime RequestTime { get; set; }
}
``` 
#### Notification consumer event model
```cs
public class SendNotificationCommand
{
    public Guid ProductId { get; set; }
}
``` 


## Define all producers and their interfaces
#### Price parser consumer event model
```cs
public class SendParserCommand
{
    public Guid ProductId { get; set; }
    public Guid SupplierId { get; set; }
    public string Url { get; set; }
    public int EnumMapping { get; set; }
    public DateTime RequestTime { get; set; }
}
``` 
#### Notification consumer event model
```cs
public class SendNotificationCommand
{
    public Guid ProductId { get; set; }
}
``` 

## Define all indexes / queries for searching the data / how would you create the index with products with all product prices from all marketplaces
Will be add.

