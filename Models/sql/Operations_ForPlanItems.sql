USE [nix]
GO
/****** Object:  StoredProcedure [dbo].[OperationsForMenuItem]    Script Date: 4/17/2023 7:01:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[Operations_ForPlanItem]
( @menuPlanId int
)
as
select 
mp.Id MenuPlanId
,st.Id StageId
,row_number () over ( partition by mp.Id order by re.Id ) Sequence
,ItemName
,ItemKind
,Name OperationName
,Activity
,CycleCount
,Strategy
,Message
,isnull(RetryCount,2) RetryCount
from MenuPlans mp
join Stages st on mp.Id = st.MenuItemId
join Reactions re on re.StageId = st.Id
where mp.id = @menuplanid

-- exec Operations_ForPlanItem @menuPlanId = 4