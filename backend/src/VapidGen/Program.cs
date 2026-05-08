using WebPush;
var keys = VapidHelper.GenerateVapidKeys();
System.Console.WriteLine($"PublicKey: {keys.PublicKey}");
System.Console.WriteLine($"PrivateKey: {keys.PrivateKey}");
