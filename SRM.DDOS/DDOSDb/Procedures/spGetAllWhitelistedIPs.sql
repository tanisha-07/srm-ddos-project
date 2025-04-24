CREATE PROCEDURE [dbo].[spGetAllWhitelistedIPs]
	@pIsActive BIT = NULL,
    @pAppKey INT = NULL
AS
	BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @IPList NVARCHAR(MAX);

        SELECT @IPList = STRING_AGG(REPLACE(LTRIM(RTRIM(IPAddress)), '  ', ''), ';') 
        FROM [dbo].[WhitelistedIps]
         WHERE @pIsActive IS NULL OR IsActive = @pIsActive
         AND @pAppKey IS NULL OR ApplicationKey = @pAppKey;

        SELECT @IPList AS IPList;
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
