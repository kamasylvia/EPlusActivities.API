using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.DrawingCommand;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;

namespace EPlusActivities.API.Application.Actors.DrawingActors
{
    public partial class DrawingActor
    {
        public async Task UpdateLotteryRecord(UpdateDrawingRecordCommand command)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(command.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                throw new NotFoundException("Could not find the lottery.");
            }
            #endregion

            #region Database operations
            lottery = _mapper.Map<UpdateDrawingRecordCommand, LotteryDetail>(command, lottery);
            lottery.PickedUpTime = command.PickedUpTime; // Skip auto mapper.
            _lotteryRepository.Update(lottery);
            if (!await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }
        public async Task DeleteLotteryRecord(DeleteDrawingRecordCommand command)
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
