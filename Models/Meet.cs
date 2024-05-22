namespace Meet.Models
{
    public class Meet
    {
        public Meet()
        {
        }

        public Meet(int meetId, string? meetTitle, DateTime meetStartTime, DateTime meetEndTime, string? meetMessage, DateTime? meetCreateTime, string? address, bool? regular, string? meetingRoom)
        {
            MeetId = meetId;
            MeetTitle = meetTitle;
            MeetStartTime = meetStartTime;
            MeetEndTime = meetEndTime;
            MeetMessage = meetMessage;
            MeetCreateTime = meetCreateTime;
            Address = address;
            Regular = regular;
            MeetingRoom = meetingRoom;
        }
        public int MeetSortId { get; set; }
        public int MeetId { get; set; }

        public string? MeetTitle { get; set; }

        public DateTime MeetStartTime { get; set; }

        public DateTime MeetEndTime { get; set; }

        public string? MeetMessage { get; set; }

        public DateTime? MeetCreateTime { get; set; }

        public string? Address { get; set; }

        public bool? Regular { get; set; }

        public string? MeetingRoom { get; set; }
        public string MeetText { get; set; }
    }
}
