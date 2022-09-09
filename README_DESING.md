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
```json
//Request Sample
{
  "email": "user@pricehunter.com",
  "password": "123456789.tY"
}
//Response Sample
{
    "data": {
        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImY1YWIxNTZkLWIyZmItNDhmNS1hZWFhLTFhMWIyY2ZkM2MxNyIsIm5hbWUiOiJVc2VyIFByaWNlSHVudGVyIiwiZW1haWwiOiJ1c2VyQHByaWNlaHVudGVyLmNvbSIsInJvbGUiOiJSb290IiwibmJmIjoxNjYyMDE0NTM0LCJleHAiOjE2NjIwMzYxMzQsImlhdCI6MTY2MjAxNDUzNH0.nUdfAN3R7OhVGvoX0lyNngVPm_LXzseVinM5nqxPNC8",
        "refreshToken": "XM454AKDxkraNBkAIfrNUQGYYv5ouEv1GAwifraSUmc=",
        "expiresIn": 1662036134,
        "refreshTokenExpireDate": "2022-09-01T13:42:14.5342912Z"
    },
    "status": "SUCCESS",
    "message": "Resource retrieved",
    "validationMessages": []
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
    public Guid CurrencyId { get; set; }
    public bool IsFavorite { get; set; }
    public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; }
}

public class UrlSupplierMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
``` json
//Sample Request
{
  "name": "Test Product",
  "currencyId": "90fc6e57-44e8-4597-93c1-bbadf45cd5b9",
  "isFavorite" : true,
  "urlSupplierMapping": [
    {
      "url": "https://www.amazon.com.tr/Logitech-Gaming-Headset-Mikrofontechnologie-Lautsprechern-Akkulaufzeit/dp/B07W7KT2B3/ref=pd_day0fbt_img_sccl_1/259-7749636-5841963?pd_rd_w=IIbx8&content-id=amzn1.sym.91d7beb6-46ba-40be-8fc8-873cbca32304&pf_rd_p=91d7beb6-46ba-40be-8fc8-873cbca32304&pf_rd_r=TAK06A102M6NR8408H10&pd_rd_wg=ad3s9&pd_rd_r=f641d3ad-c7ec-4fbb-b75a-4350ed3117d7&pd_rd_i=B07W7KT2B3&psc=1",
      "supplierId": "1e3f6f8b-1578-4576-8420-753efbae2726"
    }
  ]
}

//Sample Response
{
    "data": {
        "Id": "c2e80c5c-18e9-4572-9cff-a2cd02748990"
    },
    "status": "SUCCESS",
    "message": "New product \"Test Product\" has been created.",
    "validationMessages": []
}
``` 
#### Read/Get User Product> {url}/userproducts/{id} - GET
```cs
//Response model in BaseResponse > Data
public class UserProductViewModel
{
    public string Name { get; set; }
    public Guid CurrencyId { get; set; }
    public bool IsFavorite { get; set; }
    public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; } = new List<UrlSupplierMappingViewModel>();
}

public class UrlSupplierMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
``` json
//Sample Response
{
    "data": {
        "name": "Headphones",
        "currencyId": "90fc6e57-44e8-4597-93c1-bbadf45cd5b9",
        "isFavorite" : true,
        "urlSupplierMapping": [
            {
                "url": "https://www.amazon.com.tr/Logitech-LIGHTSPEED-kulakl%C4%B1%C4%9F%C4%B1-Teknolojisi-Hoparl%C3%B6rler/dp/B07W6FQ658/?_encoding=UTF8&pd_rd_w=iLJWl&content-id=amzn1.sym.8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_p=8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_r=30X37DGQX0WYDH83EXPP&pd_rd_wg=mbI3e&pd_rd_r=14e9fb5a-1d11-4240-991f-856395bdcdc7&ref_=pd_gw_crs_zg_bs_12466497031",
                "supplierId": "1e3f6f8b-1578-4576-8420-753efbae2726"
            }
        ]
    },
    "status": "SUCCESS",
    "message": "Resource retrieved",
    "validationMessages": []
}
``` 

#### Update User Product > {url}/userproducts/{id} - PUT
```cs
//Request
public class UpdateUserProductRequest
{
    public string Name { get; set; } 
    public Guid CurrencyId { get; set; }
    public bool IsFavorite { get; set; }
    public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; }
}

public class UrlSupplierMappingViewModel
{
    public string Url { get; set; }
    public SupplierType SupplierType { get; set; }
}
``` 
``` json
//Sample Request
{
  "name": "Test Product",
  "currencyId": "90fc6e57-44e8-4597-93c1-bbadf45cd5b9",
  "isFavorite" : true,
  "urlSupplierMapping": [
    {
      "url": "https://www.amazon.com.tr/Logitech-Gaming-Headset-Mikrofontechnologie-Lautsprechern-Akkulaufzeit/dp/B07W7KT2B3/ref=pd_day0fbt_img_sccl_1/259-7749636-5841963?pd_rd_w=IIbx8&content-id=amzn1.sym.91d7beb6-46ba-40be-8fc8-873cbca32304&pf_rd_p=91d7beb6-46ba-40be-8fc8-873cbca32304&pf_rd_r=TAK06A102M6NR8408H10&pd_rd_wg=ad3s9&pd_rd_r=f641d3ad-c7ec-4fbb-b75a-4350ed3117d7&pd_rd_i=B07W7KT2B3&psc=1",
      "supplierId": "1e3f6f8b-1578-4576-8420-753efbae2726"
    }
  ]
}

//Sample Response
{
    "data": {
        "Id": "c2e80c5c-18e9-4572-9cff-a2cd02748990"
    },
    "status": "SUCCESS",
    "message": "UserProduct \"Test Product\" has been updated",
    "validationMessages": []
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
    public Guid SupplierId { get; set; }
}
``` 
```json
//Sample Response
{
    "data": [
        {
            "price": 330.9264705882353,
            "year": 2022,
            "month": 9
        }
    ],
    "status": "SUCCESS",
    "message": "Resource retrieved",
    "validationMessages": []
}
``` 
#### Search User Product > {url}/userproducts/search - POST
Use generic filter request - response data type
```cs
//Response model in generic filter response
public class UserProductSearchViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }        
}
``` 
```cs
//Sample Request
{
  "andFilters": [
    {
      "value": "head",
      "targetFieldName": "name",
      "operation": "Contains"
    }
  ],
  "orFilters": [],
  "orderOperations": {
    "name": "Ascending"
  },
  "pageNumber": 1,
  "pageSize": 20
}

//Sample Response
{
    "data": {
        "data": [
            {
                "id": "29022d5d-74fd-4e96-beba-7cd80754b5ec",
                "name": "Headphones"
            },
            {
                "id": "448076eb-7f9c-4713-92e2-05e695d9571e",
                "name": "Headphones2"
            }
        ],
        "pageInfo": {
            "totalItemCount": 2,
            "pageSize": 20,
            "pageNumber": 1,
            "totalPageCount": 1
        }
    },
    "status": "SUCCESS",
    "message": null,
    "validationMessages": []
}
```
#### Delete User Product > {url}/userproducts/{id} - DELETE
No need extra info. Return "<b>BaseResponse</b>" model.
```json
//Sample Response
{
    "data": null,
    "status": "SUCCESS",
    "message": "UserProduct \"Test Product\" has been deleted",
    "validationMessages": []
}
``` 
#### Create Product > {url}/products - POST
```cs
//Request
 public class CreateProductRequest
{
    public string Name { get; set; }
    public Guid CurrencyId { get; set; }
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
    public Guid CurrencyId { get; set; }
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
    public Guid CurrencyId { get; set; }
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
    public Guid SupplierId { get; set; }
}
``` 

#### All Suppliers > {url}/suppliers/all - GET
```cs
//Response model in BaseResponse > Data
public class SupplierViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
``` 
```json
//Sample Response
{
    "data": [
        {
            "id": "1e3f6f8b-1578-4576-8420-753efbae2726",
            "name": "Amazon",
            "order": 1
        },
        {
            "id": "5a2d3e77-1298-4441-83fd-893c821c3b4a",
            "name": "Alibaba",
            "order": 2
        },
        {
            "id": "e458c69a-da18-4b05-974b-f9f41167042a",
            "name": "AliExpress",
            "order": 3
        }
    ],
    "status": "SUCCESS",
    "message": "Resource retrieved",
    "validationMessages": []
}
``` 

#### All Currencies > {url}/currencies/all - GET
```cs
//Response model in BaseResponse > Data
public class CurrencyViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ShortCode { get; set; }
    public int Order { get; set; }
}
``` 
```json
//Sample Response
{
    "data": [
        {
            "id": "90fc6e57-44e8-4597-93c1-bbadf45cd5b9",
            "name": "Dolar",
            "shortCode": "USD",
            "order": 1
        },
        {
            "id": "95dff93d-bfb9-410c-a4e1-ed289649ff4f",
            "name": "Euro",
            "shortCode": "EU",
            "order": 2
        },
        {
            "id": "5ea2f914-6b88-4397-80be-23e5242564f1",
            "name": "Lira",
            "shortCode": "TL",
            "order": 3
        }
    ],
    "status": "SUCCESS",
    "message": "Resource retrieved",
    "validationMessages": []
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

