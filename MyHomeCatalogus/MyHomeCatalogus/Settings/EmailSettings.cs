namespace MyHomeCatalogus.Settings
{
	public class EmailSettings
	{
		public string SmtpServer { get; set; } = "";
		public int SmtpPort { get; set; } = 0;
		public string SenderName { get; set; } = "";
		public string SenderEmail { get; set; } = "";
		public string Username { get; set; } = "";
		public string AppPassword { get; set; } = "";
	}
}
