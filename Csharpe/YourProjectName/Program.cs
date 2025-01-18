using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Timers;

class FileReadWrite
{
    static string outputDirectory = "Logs"; 
    static System.Timers.Timer? timer; 

    static void Main(string[] args)
    {
        // Create Logs directory if not exists
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Set up a timer to trigger every 30 minutes (1800000 ms)
        timer = new System.Timers.Timer(10000); 
        timer.Elapsed += GenerateNewLogFile;
        timer.Start();


        GenerateNewLogFile(null, null);

        Console.WriteLine("Logging started. Press Enter to stop.");
        Console.ReadLine();
    }

    static void GenerateNewLogFile(object? sender, ElapsedEventArgs? e)
    {
        string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string newLogFile = Path.Combine(outputDirectory, $"log_{timeStamp}.txt");

        using (StreamWriter writer = new StreamWriter(newLogFile))
        {
            writer.WriteLine("Log Start Time: " + DateTime.Now);

            FileReadWrite instance = new FileReadWrite();
            instance.WriteJsonToFile(writer, "input.json");
            instance.WriteXmlToFile(writer, "input.xml");
            instance.WriteTxtToFile(writer, "input.txt");

            writer.WriteLine("Log End Time: " + DateTime.Now);
        }
    }

    public void WriteJsonToFile(StreamWriter writer, string inputJson)
    {
        if (File.Exists(inputJson))
        {
            try
            {
                string jsonData = File.ReadAllText(inputJson);
                JObject jsonObject = JObject.Parse(jsonData);



                writer.WriteLine("JSON Data:" + DateTime.Now);

                if(jsonObject["ctRoot"] is JArray ctRootArray){
                     foreach(var item in ctRootArray){
                            writer.WriteLine("Name: " + (item["name"]?.ToString() ?? "N/A"));
                            writer.WriteLine("Email: " + (item["email"]?.ToString() ?? "N/A"));
                     }
                }
                
            }
            catch (Exception ex)
            {
                writer.WriteLine(DateTime.Now+" Error in JSON: " + ex.Message);
            }
        }
        else
        {
            writer.WriteLine($"JSON file '{inputJson}' not found.");
        }
    }

    public void WriteXmlToFile(StreamWriter writer, string inputXml)
    {
        if (File.Exists(inputXml))
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(inputXml);

                writer.WriteLine(DateTime.Now + " XML Data:" );

                XmlNodeList customerList = xmlDoc.GetElementsByTagName("customer");
                foreach (XmlNode customer in customerList)
                {
                    writer.WriteLine("Name: " + (customer["name"]?.InnerText ?? "N/A"));
                    writer.WriteLine("State: " + (customer.SelectSingleNode("address/state")?.InnerText ?? "N/A"));
                }
            }
            catch (Exception ex)
            {
                writer.WriteLine(DateTime.Now + " Error processing XML: " + ex.Message);
            }
        }
        else
        {
            writer.WriteLine($"XML file '{inputXml}' not found.");
        }
    }

    public void WriteTxtToFile(StreamWriter writer, string inputTxt)
    {
        if (File.Exists(inputTxt))
        {
            try
            {
                string txt = File.ReadAllText(inputTxt);
                writer.WriteLine(DateTime.Now + " TXT Data:");
                writer.WriteLine(txt);
            }
            catch (Exception ex)
            {
                writer.WriteLine(DateTime.Now+ " Error processing TXT: " + ex.Message);
            }
        }
        else
        {
            writer.WriteLine($"TXT file '{inputTxt}' not found.");
        }
    }
}
