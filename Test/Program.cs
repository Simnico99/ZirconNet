using ZirconNet.Core.Events;

await Task.Delay(1000);

var test = new WeakEvent<int>();
test.Subscribe((x) => Console.WriteLine($"Hello {x}!"));
test.Publish(1);

await Task.Delay(1000);

Console.ReadLine();
await Task.Delay(1000);
