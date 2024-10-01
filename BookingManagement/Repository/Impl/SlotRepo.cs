using Entity;

namespace BookingManagement.Repository.Impl;

public class SlotRepo(RallywaveContext repositoryContext) : RepositoryBase<Slot>(repositoryContext), ISlotRepo
{
    
}