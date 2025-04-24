CREATE PROCEDURE [spSaveTraffic]
	@pIPAddress VARCHAR(45),
    @pAppKey INT
AS
	BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO TrafficLogs (IPAddress, ApplicationKey)
            VALUES (REPLACE(LTRIM(RTRIM(@pIPAddress)), '  ', ''), @pAppKey);

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
