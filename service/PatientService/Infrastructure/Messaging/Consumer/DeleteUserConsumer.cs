using Application.DTO;
using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;

namespace Infrastructure.Messaging.Consumer
{
    public class DeleteUserConsumer : IConsumer<DeleteUser>
    {
        #region Attributes
        private readonly IPatientService patientService;
        #endregion

        #region Properties
        #endregion

        public DeleteUserConsumer(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        #region Methods
        public async Task Consume(ConsumeContext<DeleteUser> context)
        {
            try
            {
                var message = context.Message;
                await patientService.SyncDeleteAsync(new IAMSyncDeleteDTO()
                {
                    IdentityNumber = message.IdentityNumber,
                    PerformedBy = message.PerformedBy,
                });
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
