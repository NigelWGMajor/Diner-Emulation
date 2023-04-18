USE [nix]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[Operation_Next]
(
	@executor nvarchar(50)
)
as

declare @deliverableId int;
select @deliverableId = Id from dbo.Deliverables d where d.Executor = @executor
-- if nothing, :(
declare @currentStage int;
declare @currentSequence int;
declare @maxSequence int;
declare @planItemId int;
select @currentStage = CurrentStage, @CurrentSequence = CurrentSequence, @planItemId = menuplanId  from dbo.Deliverables where id = @deliverableId
declare @operations table 
( MenuPlanId	int, 
  StageIndex	int,
 [Sequence] int,
 ItemName	nvarchar(50),
 ItemKind nvarchar(50),
 OperationName nvarchar(50),
 Activity nvarchar(50),
 CycleCount int,
 Strategy nvarchar(50),
 Message nvarchar(250),
 RetryCount int 
)

insert into @operations
exec dbo.operations_forplanitem @planItemId;
select * from @operations;
if @currentStage < 0
begin -- we haven't started yet!
set @maxSequence = (select max([Sequence]) from @operations);
set @currentStage = 0;
set @currentSequence = 0;
end

set @currentStage = @CurrentStage + 1;
set @currentSequence = @CurrentSequence + 1;
if @currentSequence > @maxSequence
begin
   select top(0) * from dbo.Attempts for json path;
end
else
begin
   insert into dbo.Attempts
   select
      MenuPlanId,
	  StageIndex,
	  [Sequence],
	  ItemName,
	  ItemKind,
	  OperationName,
	  CycleCount,
	  Activity,
	  Strategy,
	  RetryCount,
	  Message,
	  x.Outcome,
	  x.Executor
  
  from @operations o
	  join (select 'Working' Outcome, @executor Executor, @currentSequence SequenceX) x
	  on [Sequence] = SequenceX
   ;
update dbo.Deliverables 
  set CurrentSequence = @currentSequence,
      CurrentStage = @CurrentStage,
  	MaxSequence = @maxSequence,
	Outcome = 'Working'
where Id = @deliverableId;


   select  * from dbo.Attempts where id = @@IDENTITY for json path;
end

-- select @currentStage stage, @currentSequence sequence, @maxSequence maxSequence;
-- select * from dbo.deliverables where Id = @deliverableId

-- exec Operation_Next 'nick'