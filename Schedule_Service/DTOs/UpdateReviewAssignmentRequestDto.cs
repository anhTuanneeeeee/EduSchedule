using System;

namespace Schedule_Service.DTOs
{
    public class UpdateReviewAssignmentRequestDto
    {
        public DateOnly AssignedDate { get; set; }
        public long TimeSlotId { get; set; }
        public string? Location { get; set; }
        public string? Note { get; set; }
    }
}
