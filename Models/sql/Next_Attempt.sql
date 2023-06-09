use [nix]
go
set ansi_nulls on
go
set quoted_identifier on
go
create procedure [dbo].[Next_Attempt]
(
	@executor nvarchar(50)
)
as
begin
	declare @deliverableId int,
	@stageIndex int,
	@currentSequence int,
	@nextSequence int,
	@maxSequence int,
	@planItemId int,
	@outcome nvarchar(50),
	@retryLimit int,
	@attemptedCount int;
	-- get the oldest deliverable for the executor that is not complete
	select @deliverableId = (select top(1) Id from dbo.Deliverables d where d.Executor = @executor and d.IsComplete < 1)
	-- get the stages and potential attempts for that deliverable
	declare @allAttempts table(
		MenuPlanId	int, 
		StageIndex	int,
		[Sequence] int,
		ItemName	nvarchar(50),
		ItemKind nvarchar(50),
		OperationName nvarchar(50),
		Activity nvarchar(50),
		CycleCount int,
		Strategy nvarchar(50),
		Message nvarchar(250),
		RetryLimit int 
	);
	insert into 
		@allAttempts
	exec 
		dbo.operations_forplanitem @planItemId;
	-- 	 

	select 
		@stageIndex = CurrentStage, 
		@CurrentSequence = CurrentSequence, 
		@planItemId = menuplanId  
	from dbo.Deliverables 
	where id = @deliverableId;
	-- get current attempt info
	select 
		@outcome = a.outcome ,
		@retryLimit = a.RetryLimit,
		@attemptedCount = a.AttemptedCount,
		@stageIndex = a.StageIndex
	from
		Attempts a
	inner join 
		Deliverables d
		on a.DeliverableId = d.Id
		and a.[Sequence] = d.CurrentSequence
	where a.DeliverableId = @deliverableId;

	-- is current sequence < 0?
	if @currentSequence < 0
	begin
	    set @maxSequence = (select max([Sequence]) from @allAttempts);
		update Deliverables 
		set 
			MaxSequence = @maxSequence,
			CurrentSequence = 0
			where Id = @deliverableId;
        set @currentSequence = 0;
	end -- current sequence < 0
	-- increment current sequence: is current Sequence > max sequence?
	set @nextSequence = 1 + @currentSequence;
	if @nextSequence > @maxSequence
	begin
		-- finalize the outcome, we are complete.
		-- -- read the outcome
		select @outcome = a.outcome 
		from
			Attempts a
		inner join 
			Deliverables d
			on a.DeliverableId = d.Id
			and a.[Sequence] = d.CurrentSequence
	    where a.DeliverableId = @deliverableId;
		-- -- update the deliverable
		update dbo.Deliverables 
		set 
			IsComplete = 1,
			Outcome = @outcome
		where Id = @deliverableId;
        -- -- there is no attempt for this deliverable. 
		-- -- because of the likelihood of using deliverableIds rather than executors (or a combination) this will return nothing.
		select top(0) * from @allAttempts;
		return; 
	end
	else -- the sequence still has stuff
	begin
		-- did the current attempt succeed?
		if @outcome = 'Succeeded'
		begin -- return the first attempt in the NEXT stage
			select 
				top (1) * 
			from 
				@allAttempts a 
			where 
				a.StageIndex = @StageIndex + 1 
			order by a.Sequence
			return;
		end
		else
		begin -- is the attempted count at the retrylimit?
			if @attemptedCount < @retryLimit
			begin -- return the current attempt for a retry
			    select * from @allAttempts where Sequence = @currentSequence;
			    return;
			end
			else
			begin -- is the next possibility in the same state?
				if (select StageIndex from @allAttempts a where a.Sequence = @nextSequence) = @stageIndex
				begin -- okay, try the next attempt of the same stage
				    select * from @allAttempts where Sequence = @currentSequence;
				    return;    
				end
				else
				begin -- remedies are exhausted, return nothing useful.
				    select top (0) * from @allAttempts;
					return; 
				end
			end
		end
	end	
end
-- exec Next_Attempt 'chef ramsey'