USE [nix]
GO
/****** Object:  StoredProcedure [dbo].[ClaimTask]    Script Date: 4/13/2023 10:26:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[OperationsForMenuItem]
( @ItemName nvarchar(50)
)
as
Declare @MenuItemId bigint = (select Id from dbo.MenuPlans where ItemName = @ItemName)
--select * from dbo.Stages where MenuItemid = @MenuItemId order by id

select 

mp.Id MenuPlanId
,st.Id StageId
--,isnull(AtStage, 0) AtStage
,row_number () over ( partition by mp.Id order by re.Id ) Sequence
,ItemName
,ItemKind
--IsAvailable,

,Name OperationName
,Activity
,CycleCount
--MenuItemId,
--re.Id ReactionId,
,Strategy
,Message
,0 Attempts
,isnull(RetryCount,2) RetryCount
-- ,StageId

from MenuPlans mp
join Stages st on mp.Id = st.MenuItemId
join Reactions re on re.StageId = st.Id

where menuitemid = @menuitemid;

-- exec OperationsForMenuItem 'catfish'