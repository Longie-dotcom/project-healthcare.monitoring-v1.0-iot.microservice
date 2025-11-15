using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;

namespace Infrastructure.Messaging.Consumer
{
    public class DeleteUserConsumer : IConsumer<DeleteUser>
    {
        #region Attributes
        private readonly IStaffService staffService;
        #endregion

        #region Properties
        #endregion

        public DeleteUserConsumer(IStaffService staffService)
        {
            this.staffService = staffService;
        }

        #region Methods
        public async Task Consume(ConsumeContext<DeleteUser> context)
        {
            try
            {
                var message = context.Message;
                await staffService.SyncDeleteAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consuming delete user message: {ex}");
                throw;
            }
        }
        #endregion
    }
}
