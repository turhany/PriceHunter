#### Usages
* When API project run, it insert test data to database
* Application has multi language support for demo pupose now only EN
* Before request auth need endpoints like User and Product endpoints first need to Login over login/token enpoint
    * **Accept-Language** can set "en-US"
    * **Login Email:** user@pricehunter.com
    * **Login Pass:** 123456789.tY
* When you get token from **login/token** Endpoint **"accessToken"** field can set swagger **"Authorize"** area with **"Bearer {accessToken}"** syntax
