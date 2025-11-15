using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;

namespace Infrastructure.Messaging.Consumer
{
    public class UpdateUserConsumer : IConsumer<UpdateUser>
    {
        #region Attributes
        private readonly IStaffService staffService;
        #endregion

        #region Properties
        #endregion

        public UpdateUserConsumer(IStaffService staffService)
        {
            this.staffService = staffService;
        }

        #region Methods
        public async Task Consume(ConsumeContext<UpdateUser> context)
        {
            try
            {
                var message = context.Message;
                await staffService.SyncUpdateAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consuming update user message: {ex}");
                throw;
            }
        }
        #endregion
    }
}
