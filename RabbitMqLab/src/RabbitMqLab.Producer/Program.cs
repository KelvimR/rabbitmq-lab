using RabbitMQ.Client;
using RabbitMqLab.Contracts;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

const string queueName = "orders.created";

//configura onde está o RabbitMQ.
var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "user",
    Password = "password",
};

//cria uma conexão.
await using var connection = await factory.CreateConnectionAsync();

//O Channel é uma sessão lógica dentro da conexão.
await using var channel = await connection.CreateChannelAsync();

//RabbitMQ, garanta que exista uma fila chamada orders.created.
await channel.QueueDeclareAsync(
    queue: queueName,
    durable: true, //significa que a definição da fila sobrevive a reinicializações do broker.
    exclusive: false,
    autoDelete: false);

var order = new OrderCreated(
    OrderId: Guid.NewGuid(),
    CustomerName: "Kelvim",
    Total: 499.99m,
    CreatedAt: DateTime.UtcNow);

var json = JsonSerializer.Serialize(order);
var body = Encoding.UTF8.GetBytes(json);
var properties = new BasicProperties
{
    Persistent = true, //indica que a mensagem deve ser tratada como persistente.
    ContentType = "application/json",
};

await channel.BasicPublishAsync(
    exchange: string.Empty,
    routingKey: queueName,
    mandatory: false,
    basicProperties: properties,
    body: body);


Console.WriteLine();
Console.WriteLine("Pedido publicado!");
Console.WriteLine($"Order Id: {order.OrderId}");
Console.WriteLine($"Cliente: {order.CustomerName}");
Console.WriteLine($"Total: R$ {order.Total}");
Console.WriteLine();
Console.WriteLine($"Mensagem:");
Console.WriteLine(json);