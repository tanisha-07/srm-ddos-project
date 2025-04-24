CREATE PROCEDURE [dbo].[spSaveUpdateWhitelistedIP]
	@pIPAddress VARCHAR(45),
	@pIsActive BIT,
    @pAppKey INT,
    @pReason NVARCHAR(255) = NULL
AS
	BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        MERGE INTO [dbo].[WhitelistedIPs] AS target
        USING (SELECT @pIPAddress AS IPAddress) AS source
        ON (target.IPAddress = source.IPAddress)
        WHEN MATCHED THEN
            UPDATE SET Reason = @pReason, IsActive = @pIsActive, ApplicationKey = @pAppKey
        WHEN NOT MATCHED THEN
            INSERT (IPAddress, Reason, IsActive, ApplicationKey)
            VALUES (REPLACE(LTRIM(RTRIM(@pIPAddress)), '  ', ''), @pReason, @pIsActive, @pAppKey);

        Return 1;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
