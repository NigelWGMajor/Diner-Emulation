USE [nix]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[Claim_Responibility]
( @executor nvarchar(50)
)
as
declare @deliverableId int = (select top (1) [Id] from dbo.Deliverables where Executor = '' order by id)
if (@deliverableId <> null)
begin
  update dbo.Deliverables set Executor = @executor;
  select @@identity  DeliverableId for json path;
end
else
begin
  select 0 for json path;
end