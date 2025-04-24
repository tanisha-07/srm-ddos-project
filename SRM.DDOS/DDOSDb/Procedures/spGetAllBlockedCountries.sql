CREATE PROCEDURE [spGetAllBlockedCountries]
	@pIsActive BIT = NULL,
    @pAppKey INT = NULL
AS
	BEGIN
    SET NOCOUNT ON;

        SELECT Country 
        FROM BlockedCountries
         WHERE @pIsActive IS NULL OR IsActive = @pIsActive
         AND @pAppKey IS NULL OR ApplicationKey = @pAppKey;

END;
