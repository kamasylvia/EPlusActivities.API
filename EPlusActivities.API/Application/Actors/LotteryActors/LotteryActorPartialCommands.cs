using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.LotteryCommands;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;

namespace EPlusActivities.API.Application.Actors.LotteryActors
{
    public partial class LotteryActor
    {
        public async Task UpdateLotteryRecord(UpdateLotteryRecordCommand command)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                throw new NotFoundException("Could not find the lottery.");
            }
            #endregion

            #region Database operations
            lottery = _mapper.Map<UpdateLotteryRecordCommand, Lottery>(command, lottery);
            lottery.PickedUpTime = command.PickedUpTime; // Skip auto mapper.
            _lotteryRepository.Update(lottery);
            if (!await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
        public async Task DeleteLotteryRecord(DeleteLotteryRecordCommand command)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                throw new NotFoundException("Could not find the the lottery.");
            }
            #endregion

            #region Database operations
            _lotteryRepository.Remove(lottery);
            if (await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
    }
}
