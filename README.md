# Price Hunter Software requirements

Price Hunter is a distributed price monitoring system deisgned to work with any website that offers pricing (supported)

## Workflow

A user opens the price hunder dashboard and logs in. I not registered he proceeds to the register page.
Once logged in a main dashboard show up with statiscs on added products for monitoring. Currently supported sites are Amazon, Alibaba, AliExpress, comming soon sites are ebay, shopping.com,taobao etc.
From this main dashboard screen he could see the list of favorite producs added to the monitor and manage them ( add/remove from favorites), as well as see the most recent price changes in values. Aditionaly a price chart show price history for the selected product from the list.

In the main menu there are sever menu items Home, Producs, Search, Profile ( with user image).

When navigating to the products page a new paged list is displayed containg more details about the producs like image , Name, Current Min price, Current Min Price market.
When a product is selected a chart (bar) chart is shown with current levels / + line chart with prices trends for the last 6 months
The user can add a product to his list of products for monitoring / with an url/ after which the product data is extracted and a search for the product is performed within our database ( index) for a matching product; catergory etc.
When a product is removed from the list only the product reference for the user is removed not the product pricing itself as well as all other history prices.

The system should check prices periodically ever X minutes (different for each market) and in case of change an PriceChangeEvent should be emited and all consumers subscribed to this type of product should be notified via email,sms,push notification.

TODO: Define missing requirements.

## Technical Requirements

All services will be based on .Net 6
Front End + Back End For Front end done in a Blazor App + Api /
Each Product Parser should be Independantly developed but based on same consumer-producer / base libraries

- For now prace detection changes should be performed via an endpoint in order not to complicate implementation
- Changes in pricing should emit events that will be consumed by notifier which will produce the email message
- Changes in pricing should be consumed by a search indexer that is consumed by the app for faster search and product display

Email notification should be done (for now by placing a record in a mongo collection)

## Tasks

Define initial Architecture
Define initial Comon interfaces of interservice comunications
  TODO: Research for starnards
Define list of components to implement
Define all api endpoints and their schema
Define all consumers and their interfaces
Define all producers and their interfaces
Define all indexes / queries for searching the data / how would you create the index with products with all product prices from all marketplaces

Implement front end

- propose initial simple design
- propose component separation
- propose authentication solution (do not implement at phaze 1)
- propose BFFE api endpoints for data consumpion
- implement api endpoints
- implement components / think and propose component reuse ()
- implement pages
- tests
- think on validation for components and implement

Implement Producers / Consumers

- how will the whole workflow be executed / write up
- implement component scheduling
- implement component (fake parsing via api endponit)

implement Notification components

- Think on separation of conerns for the components /
- Propose implementation
- Implement component

Implement Indexsing and Searching

- propose index schema
- propose consumer/s
- propose data transformation to match
- implement component
- implement related infrastracture

Infrastructure Infrastructure

- propose infrastructure docker compose infrastructure
- propose netwroking configuration for docker
- propose persistence metods (mongo volume)
- build configuration for containers and deployment

###### Doc Version : PriceHunter-Requirements v 0.5

## Docs
* Architecture Documentation [here](README_DESING.md)
* Technologies [here](README_TECHNOLOGIES.md)
* Development Task Documentation [here](README_TASKS.md)
* Local Development Docker Codes [here](README_LOCALDEVDOCKER.md)
* App GUI(s) [here](README_GUIS.md)
* DB, Distributed Cache - Lock, Message Queue Settings  [here](README_CONFIG.md)
* Sample Data Flow (Usage)  [here](README_USAGE.md)
* Application URLS [here](README_URLS.md)
* Blazor Resources [here](README_BLAZOR.md)
* What's next? [here](README_NEXT.md)