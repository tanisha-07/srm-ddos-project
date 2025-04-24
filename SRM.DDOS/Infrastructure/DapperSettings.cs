namespace SRM.DDOS.API.Infrastructure
{
    public sealed class DapperSettings
    {
        /// <summary>
        /// The section name
        /// </summary>
        public const string SectionName = "ConnectionStrings";

        /// <summary>
        /// Gets or sets the SQL server.
        /// </summary>
        /// <value>
        /// The SQL server.
        /// </value>
        public string SqlServer { get; set; } = null!;
    }
}
