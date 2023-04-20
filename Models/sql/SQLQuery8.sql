use [nix]
go
set ansi_nulls on
go
set quoted_identifier on
go
alter procedure [dbo].[Attempt_Update](
  @deliverableId int,
  @outcome nvarchar(25)
)
as
begin
update 
	dbo.Deliverables 
set 
	Outcome = @outcome 
where 
	Id = @deliverableId
update 
	Attempts
set 
	Outcome = @outcome 
--> Select * 
from
	Attempts a
	inner join 
	Deliverables d
	on a.DeliverableId = d.Id
	and a.[Sequence] = d.CurrentSequence
	where a.DeliverableId = @deliverableId
select @@identity
end