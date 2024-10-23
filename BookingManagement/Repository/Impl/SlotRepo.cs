using Entity;

namespace BookingManagement.Repository.Impl;

public class SlotRepo(RallyWaveContext repositoryContext) : RepositoryBase<Slot>(repositoryContext), ISlotRepo
{
    
}