public class TeleporterBlock : Block
{
    public int linkedTeleporterId;
    private TeleporterModel model;

    private void Start()
    {
        model = GetComponentInChildren<TeleporterModel>();
    }

    public void Teleport()
    {
        if (model == null) model = GetComponentInChildren<TeleporterModel>();
        model.StartTeleportationTween();
    }

    public void ReceiveTeleport()
    {
        if (model == null) model = GetComponentInChildren<TeleporterModel>();
        model.StartReceiveTeleportationTween();
    }
}