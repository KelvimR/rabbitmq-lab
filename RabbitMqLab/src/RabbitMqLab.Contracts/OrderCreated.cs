namespace RabbitMqLab.Contracts;

//Producer e Consumer precisam concordar sobre qual é o formato da mensagem.
public record OrderCreated
(
    Guid OrderId,
    string CustomerName,
    decimal Total,
    DateTime CreatedAt
);
