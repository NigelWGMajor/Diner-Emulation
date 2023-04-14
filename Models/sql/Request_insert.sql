
USE [nix]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].Request_Insert
( 
@origin int,
@initiator varchar(50),
@contact varchar(50),
@request varchar (500)
)
as
insert into dbo.Requests
(
Origin
,Initiator
,Contact 
--Executor, 
,Request
,ReceivedAt 
,IsActive 
--ReturnedAt, 
--Outcome, 
--Churn
) values
(
@origin,
@initiator,
@contact,
@request,
GetDate(),
1
);
select @@identity  RequestId;