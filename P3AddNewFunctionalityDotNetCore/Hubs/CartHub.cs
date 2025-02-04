using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class CartHub : Hub
{
    // Méthode pour notifier les clients que le produit est devenu indisponible
    public async Task NotifyProductUnavailable(int productId)
    {
        await Clients.All.SendAsync("ProductUnavailable", productId);
    }
}