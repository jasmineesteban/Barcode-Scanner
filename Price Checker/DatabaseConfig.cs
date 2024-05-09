using System.Configuration;
using System.Xml.Linq;

public class DatabaseConfig
{
    public string Server { get; set; }

    public string Port { get; set; }
    public string Uid { get; set; }
    public string Pwd { get; set; }
    public string Database { get; set; }

    public DatabaseConfig()
    {
        var doc = XDocument.Load(@"C:\ESTEBAN_JASMINE_PUPSMB\C#\Barcode-Scanner\PriceScannerV1\Price Checker\config.xml");
        var databaseSettings = doc.Element("configuration").Element("databaseSettings");
        Server = databaseSettings.Element("add").Attribute("server").Value;
        Port = databaseSettings.Element("add").Attribute("port").Value;
        Uid = databaseSettings.Element("add").Attribute("uid").Value;
        Pwd = databaseSettings.Element("add").Attribute("pwd").Value;
        Database = databaseSettings.Element("add").Attribute("database").Value;
    }
}