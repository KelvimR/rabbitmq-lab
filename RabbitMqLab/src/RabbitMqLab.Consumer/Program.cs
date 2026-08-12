using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqLab.Contracts;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

const string queueName = "orders.created";

var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "user",
    Password = "password",
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync(
    queue: queueName,
    durable: true,
    exclusive: false,
    autoDelete: false);

var consumer = new AsyncEventingBasicConsumer(channel);

//Quando chegar uma mensagem, execute este código.
consumer.ReceivedAsync += async (_, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    var order = JsonSerializer.Deserialize<OrderCreated>(json);

    Console.WriteLine();
    Console.WriteLine("Mensagem recebida!");
    Console.WriteLine($"Pedido..: {order?.OrderId}");
    Console.WriteLine($"Cliente.: {order?.CustomerName}");
    Console.WriteLine($"Total...: R$ {order?.Total}");
    Console.WriteLine($"Data....: {order?.CreatedAt}");
    Console.WriteLine("-------------------------");

    //RabbitMQ, processei essa mensagem com sucesso.
    await channel.BasicAckAsync(
        deliveryTag: eventArgs.DeliveryTag, 
        multiple: false);
};

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false,
    consumer: consumer);

Console.WriteLine($"Consumer aguardando mensagens em '{queueName}'...");
Console.WriteLine("CTRL+C para encerrar.");

await Task.Delay(Timeout.Infinite);