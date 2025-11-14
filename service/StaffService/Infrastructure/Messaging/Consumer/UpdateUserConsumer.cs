using Application.DTO;
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
                await staffService.SyncUpdateAsync(new IAMSyncUpdateDTO()
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
