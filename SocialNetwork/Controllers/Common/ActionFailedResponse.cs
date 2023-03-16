namespace SocialNetwork.Controllers.Common;

public class ActionFailedResponse
{
    public string Message { get; set; }
    public string RequestId { get; set; }
    public int Status { get; set; }
}