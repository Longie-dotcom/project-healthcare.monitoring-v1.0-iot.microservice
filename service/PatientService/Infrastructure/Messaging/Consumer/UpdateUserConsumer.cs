using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;

namespace Infrastructure.Messaging.Consumer
{
    public class UpdateUserConsumer : IConsumer<UpdateUser>
    {
        #region Attributes
        private readonly IPatientService patientService;
        #endregion

        #region Properties
        #endregion

        public UpdateUserConsumer(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        #region Methods
        public async Task Consume(ConsumeContext<UpdateUser> context)
        {
            var message = context.Message;

            try
            {
                await patientService.SyncUpdateAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consuming update user message: {ex} for patient: {message.IdentityNumber}");
                throw;
            }
        }
        #endregion
    }
}
