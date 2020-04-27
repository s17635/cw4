CREATE PROCEDURE PromoteStudents
@Studies NVARCHAR(100),
@Semester INT
AS
BEGIN
	DECLARE @nextSemester INT;
	SET @nextSemester = @Semester +1 ;
	DECLARE @oldIdEnroll INT;
	DECLARE @newIdEnroll INT;
	DECLARE @idStudy INT;
	
	SET @oldIdEnroll = (SELECT e.IdEnrollment FROM Enrollment e JOIN Studies s ON e.IdStudy = s.IdStudy WHERE s.Name = @Studies AND e.Semester = @Semester);
	SET @newIdEnroll = (SELECT e.IdEnrollment FROM Enrollment e JOIN Studies s ON e.IdStudy = s.IdStudy WHERE s.Name = @Studies AND e.Semester = @nextSemester);
	IF @newIdEnroll IS NULL
	BEGIN
		SET @newIdEnroll = (SELECT Max(IdEnrollment) FROM Enrollment)+1;
		SET @idStudy = (SELECT IdStudy FROM Studies WHERE Name=@Studies);
		INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES (@newIdEnroll,@nextSemester,@idStudy,CURRENT_TIMESTAMP);
	END
	
	UPDATE Student SET IdEnrollment = @newIdEnroll WHERE IdEnrollment = @oldIdEnroll;
END