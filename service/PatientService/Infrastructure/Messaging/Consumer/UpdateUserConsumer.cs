using Application.DTO;
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
            try
            {
                var message = context.Message;
                await patientService.SyncUpdateAsync(new IAMSyncUpdateDTO()
                {
                    PerformedBy = message.PerformedBy,
                    IdentityNumber = message.IdentityNumber,
                    Address = message.Address,
                    DateOfBirth = message.Dob,
                    FullName = message.Name,
                    Gender = message.Gender,
                    Phone = message.Phone,
                    Email = message.Email,
                });
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
