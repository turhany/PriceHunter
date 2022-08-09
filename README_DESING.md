<!-- Define initial Architecture -->
## Architecture 

PriceHunter is full scalable, microservice application.

## System Design

![alt tag](files/images/system_diagram.png) 

## Database Diagram

![alt tag](files/images/database_diagram.png) 


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
 
#### Create Product > {url}/products - POST
```cs
//Request
public class CreateProductRequest{
    public string Name {get; set;}
    public IFormFile Image {get; set;} 
    List<UrlSupplierMappingViewModel> UrlSupplierMapping {get; set;}
}

public class UrlSupplierMappingViewModel{
    public string Url {get; set;}
    public SupplierType SupplierType {get; set;}
}
``` 
#### Update Product > {url}/products/{id} - PUT
```cs
//Request
public class UpdateProductRequest{
    public string Name {get; set;}
    public IFormFile Image {get; set;} 
    List<UrlSupplierMappingViewModel> UrlSupplierMapping {get; set;}
}

public class UrlSupplierMappingViewModel{
    public string Url {get; set;}
    public SupplierType SupplierType {get; set;}
}
``` 

#### Read/Get Product > {url}/products/{id} - GET
```cs
//Response model in BaseResponse > Data
public class ProductViewModel{
    public Guid Id {get; set;}
    public string Name {get; set;} 
    public string Image {get; set}
    public double CurrentPrice {get; set;}
    public string CurrentMinMarket {get; set;} 
}
``` 

#### Delete Product > {url}/products/{id} - DELETE
No need extra info. Return "<b>BaseResponse</b>" model.

#### Search Product > {url}/products/search - POST
This end point get "<b>FilterRequest</b>" for search flow and return <b>PagedList</b>" base response.
```cs
//Response model in PagedList > Data list
public class ProductListViewModel{
    public Guid Id {get; set;}
    public string Name {get; set;}
    public string Image {get; set}
    public double CurrentPrice {get; set;}
    public string CurrentMinMarket {get; set;} 
}
``` 

#### Search Product Params > {url}/products/search/params - GET
This end point will give the options which will be use for search request.
```cs
//Response model in PagedList > Data list
public class ProductSearchParamsViewModel{
    public List<int, String> SupplierTypes {get; set;} 
}
``` 

#### Get Product Trend> {url}/products/trend/last6month/{id} - GET
```cs
//Response model in BaseResponse > Data = List of ProductTrendModel
public class ProductTrendViewModel{
    public int Order {get; set;} 
    public string Month {get; set;}  
    public double Price {get; set;} 
}
``` 

#### Get User Profile> {url}/users/profile/{id} - GET
```cs
//Response model in BaseResponse > Data
public class UserViewModel{
    public int Id {get; set;} 
    public string Image {get; set;}
    public string FirstName {get; set;}  
    public string LastName {get; set;} 
}
``` 

## Define all consumers and their interfaces
Consumers will consume all producer events.

## Define all producers and their interfaces
```cs
public class ProductPriceChangedEvent{
    public int Id {get; set;} 
    public int Name {get; set;} 
    public string NewPrice {get; set;}
    public string Url {get; set;}
    public SupplierType SupplierType {get; set;}
}
``` 
```cs
public class CheckProductPriceEvent{
    public int Id {get; set;} 
    public int Name {get; set;} 
    public string Url {get; set;}
    public SupplierType SupplierType {get; set;}
}
``` 

## Define all indexes / queries for searching the data / how would you create the index with products with all product prices from all marketplaces
Will be add.