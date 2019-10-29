# ASP.NET C\# Vulnerability Demo

## Setup
* cd ${workplace}
* docker-compose up --build
* {open browser and navigate to localhost:5005}

## Vulnerability Demo

### SQL Injection:

### Directory traversal:

### Hard Coded credentials:

### Sensitive data leak:

### Insider attack:

### Business logic flaw
- Explain: It is a **Deserialization Vulnerability**. 
Basically, server code deserialize a input controlled by the attack.
When deserialize an object, its "ReadObject" method was invoked implicitly by the deserializer,
which futher invoke the malicious payload (Gadget)
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
