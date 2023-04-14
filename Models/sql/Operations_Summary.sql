USE [nix]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].Operations_Summary

as
;with detail as (
  select 
	sum(attempts) Attempts, 
	sum(RetryCount) Retries, 
	sum(ErrorCount) Errors, 
	case 
		when sum(attempts) = 0 then 'Pending'
		when sum(errorCount) >= sum(attempts) then 'Failed'
		when Max(IsComplete) > 0 then 'Complete' 
		when sum(Attempts) > 0 then 'Started'
		end Status 
	from operations group by RequestId, DinerIndex, ItemIndex
)
select -- * from detail
(select count(Status) from detail where Status = 'Pending') Pending,
(select count(Status) from detail where Status = 'Started') Started,
(select count(Status) from detail where Status = 'Failed') Failed,
(select count(Status) from detail where Status = 'Complete') Complete



-- exec Operations_Summary