using ConsoleTables;
using Newtonsoft.Json;
using System.Reflection;
using Trogsoft.CommandLine;
using Trogsoft.Ectobi.Common;

namespace ectobi
{
    public class EctoVerb : Verb
    {
        protected void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        protected int WriteSuccess(Success success, bool verbose = false)
        {
            if (success.Succeeded)
                WriteSuccess("Success.");
            else
                WriteError($"[{success.ErrorCode}] {success.StatusMessage ?? "Failed. Reason unknown."}");

            return success.ErrorCode;
        }

        protected int WriteSuccess<T>(Success<T> success, bool verbose = false)
        {
            if (success.Succeeded)
            {
                WriteSuccess("Success.");
                if (success.Result == null) return success.ErrorCode;

                if (success.Result is BackgroundTaskInfo bi)
                {
                    Console.WriteLine($"Background Task ID: {bi.Id}");
                    return 0;
                }

                var resultType = success.Result.GetType();
                var id = resultType.GetProperty("Id");
                var tid = resultType.GetProperty("TextId");
                if (!verbose)
                {
                    if (id != null)
                        Console.WriteLine($"Id = {id.GetValue(success.Result)}");

                    if (tid != null)
                        Console.WriteLine($"TextId = {tid.GetValue(success.Result)}");
                }
                else if (verbose)
                {
                    if (success.Result != null) Console.WriteLine(JsonConvert.SerializeObject(success.Result, Formatting.Indented));
                }
            }
            else
            {
                WriteError($"[{success.ErrorCode}] {success.StatusMessage ?? "Failed. Reason unknown."}");
                if (success.Result != null) Console.WriteLine(JsonConvert.SerializeObject(success.Result, Formatting.Indented));
            }

            return success.ErrorCode;
        }

    }
}