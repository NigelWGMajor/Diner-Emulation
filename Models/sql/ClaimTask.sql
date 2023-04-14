USE [nix]
GO
/****** Object:  StoredProcedure [dbo].[ClaimTask]    Script Date: 4/14/2023 9:56:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[ClaimTask]
( @name nvarchar(50)
)
as
declare @id bigint;
set @id = (select min(Id) from dbo.Requests where IsActive = 1 and Executor is null)
update dbo.requests set Executor = @name, ReceivedAt = GetDate(), Outcome = 'Processing' where Id = @id;
select * from dbo.Requests where Id = @id for json auto;

-- exec claimtask @Name='Nick'