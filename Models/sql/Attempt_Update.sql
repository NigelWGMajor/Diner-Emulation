USE [nix]
GO
/****** Object:  StoredProcedure [dbo].[Attempt_Update]    Script Date: 4/19/2023 12:52:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[Notify_Outcome](
  @deliverableId int,
  @outcome nvarchar(25)
)
as
begin
	declare 
		@retryLimit int,
		@attemptedCount int,
		@maxSequence int,
		@currentSequence int,
		@currentStage int,
		@maxStage int,
		@nextStage int;
	select 
		@retryLimit = a.RetryLimit, 
		@attemptedCount = isnull(a.attemptedCount, 0), 
		@maxSequence = d.MaxSequence,
		@maxStage = d.MaxStage,
		@currentSequence = d.CurrentSequence,
		@currentStage = d.CurrentStage
	from 
		Attempts a
		inner join 
			Deliverables d
			on a.DeliverableId = d.Id
			and a.[Sequence] = d.CurrentSequence
	where a.DeliverableId = @deliverableId;

-- -- set attempt outcome and increment the attemptedCount
	update Attempts 
	set 
		Outcome = @outcome,
		AttemptedCount = @attemptedCount + 1
	from 
		Attempts a
		inner join 
			Deliverables d
			on a.DeliverableId = d.Id
			and a.[Sequence] = d.CurrentSequence
	where a.DeliverableId = @deliverableId;

	-- -- increment sequence number

	set @currentSequence = @currentSequence + 1;

	-- -- if current sequenece <= max, do these steps first:

	if @currentSequence <= @maxSequence

	begin
	-- -- -- if the attempt succeeded, if this is the last stage, we are done and should close the deliverable!
		if @outcome = 'Succeeded'
		begin
			if @currentStage = @maxStage
			begin
				update Deliverables 
				set IsComplete = 1,
					Outcome = 'Succeeded'
					-- could also update churn here...
				where Id = @DeliverableId;
				return;
			end
		end
		else -- the attempt failed - if we are below the retry limit, exit and it will retry
		begin
			if @attemptedCount < @retryLimit
			begin
			update Attempts
			set 
				Outcome = 'Retrying'
			from
				Attempts a
				inner join 
				Deliverables d
				on a.DeliverableId = d.Id
				and a.[Sequence] = d.CurrentSequence
				where a.DeliverableId = @deliverableId
				--
				return;
			end
		end
		-- here we have failed, and exhausted retries in this attempt. 
		-- If the next attempt is the same stage as this, we can let it retry.
		select @nextStage = isnull(StageIndex, -1) 
		from dbo.Attempts a
		where a.DeliverableId = @deliverableId 
		and Sequence = @currentSequence + 1;
		if @nextStage = @CurrentStage
		begin
		update Attempts
		set 
			Outcome = 'Retrying'
		from
			Attempts a
			inner join 
			Deliverables d
			on a.DeliverableId = d.Id
			and a.[Sequence] = d.CurrentSequence
			where a.DeliverableId = @deliverableId
			--
			return;
		end
	end
	update Deliverables 
	set IsComplete = 1,
		Outcome = @outcome
		-- could also update churn here...
	where Id = @DeliverableId;
	return;
end




