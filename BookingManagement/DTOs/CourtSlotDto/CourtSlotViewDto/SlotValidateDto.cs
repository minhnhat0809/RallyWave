namespace BookingManagement.DTOs.CourtSlotDto.CourtSlotViewDto;

public class SlotValidateDto(int slotId, TimeOnly timeStart, TimeOnly timeEnd)
{
    public int SlotId { set; get; } = slotId;

    public TimeOnly TimeStart { get; set; } = timeStart;

    public TimeOnly TimeEnd { get; set; } = timeEnd;
}