namespace Application.Interface.IGrpc
{
    public interface IPatientGrpcClient
    {
        Task<bool> IsControllerInUseAsync(string controllerKey);
    }
}
