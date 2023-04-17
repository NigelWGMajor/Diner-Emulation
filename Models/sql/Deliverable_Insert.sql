USE [nix]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[Deliverable_insert]
( @requestId int,
  @dinerIndex int,
  @itemIndex int,
  @itemName nvarchar(50)
)
as
declare @menuPlanId int = (select [Id] from MenuPlans where [ItemName] = @itemName);

insert into dbo.Deliverables
(
RequestId
,DinerIndex
,ItemIndex
,MenuPlanId
,CurrentStage
,CurrentAttempt
,Outcome
,Churn
) values
(
@requestId
,@dinerIndex
,@itemIndex
,@menuPlanId
,-1
,-1
,''
,0
);
select @@identity  RequestId;