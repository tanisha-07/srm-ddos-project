CREATE PROCEDURE [spGetAllBlacklistedIPs]
	@pIsActive BIT = NULL,
    @pAppKey INT = NULL
AS
	BEGIN
    SET NOCOUNT ON;

        SELECT IPAddress 
        FROM BlacklistedIPs
         WHERE @pIsActive IS NULL OR IsActive = @pIsActive
         AND @pAppKey IS NULL OR ApplicationKey = @pAppKey;

END;
