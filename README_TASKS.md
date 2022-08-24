#### Tasks
<ol>
<li>Make overall system design</li>
<li>Make overall database design</li>
<li>Make overall solution(sln) design with template projects.</li>
<li>Design "User" database schema.</li>
<li>Design "UserProduct" database schema.</li>
<li>Design "Product" database schema.</li> 
<li>Database
    <ol>
        <li>Design generic database crud repository (Repository Pattern)</li>
        <li>Develop re-usable generic repository works with MongoDB</li>
    </ol>
</li>
<li>Messaging
    <ol>
        <li>Design messaging system business flow(RabbitMQ)</li>
        <li>Develop consumer re-usable template project</li>
    </ol>
</li>
<li>Backgound Services
    <ol>
        <li>Design background messaging system business flow(HangFire)</li>
        <li>Develop background service re-usable template project</li>
    </ol>
</li>
<li>Cache
    <ol>
        <li>Design cache system business flow</li>
        <li>Develop cache service re-usable template project</li>
        <li>Implement redis distributed cache service(Redis)</li>
    </ol>
</li>
<li>Lock
    <ol>
        <li>Design lock system business flow</li>
        <li>Develop lock service re-usable template project</li>
        <li>Implement redis distributed lock service(Redis-RedLock)</li>
    </ol>
</li>
<li>Notification
    <ol>
        <li>Design notification system business flow</li>
        <li>Develop notification service re-usable template project</li>
        <li>Implement email notification service</li>
        <li>Implement sms notification service</li>
        <li>Implement push-notification, notification service</li>
    </ol>
</li>
<li>Implement log system use Serilog</li>
<li>API 
    <ol>
        <li>LoginController
        <ol>
            <li>Implement login business flow</li>
            <li>Implement "GetToken" endpoint</li>
            <li>Implement "RefreshToken" endpoint</li>
        </ol> 
        </li>  
        <li>UserController
        <ol>
            <li>Implement Get User Detail - "Get" endpoint, service and repository flow</li>
            <li>Implement Create User - "Post" endpoint, service and repository flow</li>
            <li>Implement Update User - "Put" endpoint, service and repository flow</li>
            <li>Implement Delete User - "Delete" endpoint, service and repository flow</li>
        </ol>
        </li>    
        <li>UserProductController
        <ol>
            <li>Implement Get User Product Detail - "Get" endpoint, service and repository flow</li>
            <li>Implement Create User Product - "Post" endpoint, service and repository flow</li>
            <li>Implement Update User Product - "Put" endpoint, service and repository flow</li>
            <li>Implement Delete User Product - "Delete" endpoint, service and repository flow</li>
            <li>Implement product price last 6 month trens endpoint</li>
        </ol>
        </li>  
        <li>Develop request-response log system(Middleware)</li>
        <li>Develop global exception handling system(Middleware)</li>
        <li>Implement swagger</li>
        <li>Implement API versioning</li>
    </ol>
</li>
<li> Parsers
    <ol>
        <li>Design & Develop re-usable parser template project</li>
        <li>Implement Amazon price parser</li>
        <li>Implement Alibaba price parser</li>
        <li>Implement ALiExpress price parser</li>
    </ol>
</li>
<li> Consumer
    <ol> 
        <li>Develop generic parser consumer</li>
        <li>Develop generic notification consumer</li>
    </ol>
</li>
<li> Background Service
    <ol> 
        <li>Develop parser time check and send parser event backgound service</li>
    </ol>
</li>
<li>Implement gateway project(Ocelot)</li>
<li> Docker
    <ol> 
        <li>Add API docker support</li>
        <li>Add BackgroundService docker support</li>
        <li>Add Parser Consumer docker support</li>
        <li>Add Notification Consumer docker support</li>
        <li>Add docker-commpose for start all system</li>
    </ol>
</li> 
<li>UI App 
    <ol>
        <li>Develop Auth provider
        <li>Find and Develop/Implement LocalStorage system
        <li>Find and Develop/Implement Notification(Toast) system
        <li>Develop http request helper</li>
        <li>Pages
        <ol>
            <li>Login page</li>
            <li>Logout page</li>
            <li>Profile page</li>
            <li>Develop User product list page/business flow</li>
            <li>Develop User product detail page/business flow</li>
            <li>Develop User product edit page/business flow</li>
            <li>Develop User product add page/business flow</li>
            <li>Develop User product delete flow</li>
            <li>Develop User product last 6 month price changes flow</li>
        </ol> 
        </li>  
    </ol>
</li>
<li>Service discovery mesh gateway implentation</li>
</ol>