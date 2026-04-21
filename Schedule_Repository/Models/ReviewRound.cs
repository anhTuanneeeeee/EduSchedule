using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class ReviewRound
{
    public long ReviewRoundId { get; set; }

    public long ProjectGroupId { get; set; }

    public int RoundNumber { get; set; }

    public string? RoundName { get; set; }

    public string Status { get; set; } = null!;

    public virtual ProjectGroup ProjectGroup { get; set; } = null!;

    public virtual ReviewAssignment? ReviewAssignment { get; set; }
}
