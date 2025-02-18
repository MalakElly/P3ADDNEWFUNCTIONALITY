using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

public class CartHub : Hub
{
    // Méthode pour notifier les clients que le produit est devenu indisponible
    public async Task NotifyProductUnavailable(int productId,int productQuantity)
    {
        Console.WriteLine($"[CartHub] Notification envoyée pour produit ID: {productId}");
        await Clients.All.SendAsync("ProductUnavailable", productId);
    }
}