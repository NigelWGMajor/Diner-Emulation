USE [nix]
go

set ansi_nulls on
go

set quoted_identifier on
go
------------------- Flow definition Schema --------------
create schema [flo]
go
grant select, insert, update, delete on schema::[flo] TO [public]
go
------------------- Operational Schema ------------------
create schema [ops]
go
grant select, insert, update, delete on schema::[ops] TO [public]
go
------------------ Archive schema -----------------------
create schema [arc]
go
grant select, insert, update, delete on schema::[arc] TO [public]
go
------------------ Flow definition ----------------------
create table [flo].[Plans](
	[PlanId] [Bigint] identity(1000,1) not null,
    [Status] [nvarchar](25) not null,
	[ItemName] [nvarchar](50) not null,
	[ItemKind] [nvarchar](50) null,
	[Description] [nvarchar](500) null,
	[Availability] [int] not null,
 constraint [PK_MenuPlans] primary key clustered
(
	[PlanId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off) on [Primary]) ON [Primary]
go
alter table [flo].[Plans] add  constraint [DF_Plans_Status]  default ('Active') for [Status]
go
alter table [flo].[Plans] add  constraint [DF_plans_Availability]  default ((1)) for [Availability]
go

create table [flo].[Stages](
	[StageId] [Bigint] identity(5000,1) not null,
	[Name] [nvarchar](25) not null,
	[Activity] [nvarchar](50) not null,
	[CycleCount] [int] not null,
	[PlanId] [bigint] not null,
	[SequenceIndex] int not null
 constraint [PK_Stages] primary key clustered
(
	[StageId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off)  on [Primary]) ON [Primary]
go

create table [flo].[Trials](
	[TrialId] [Bigint] identity(3000,1) not null,
	[Strategy] [nvarchar](25) not null,
	[Message] [nvarchar](100) not null,
	[RetryCount] [int] not null,
	[StageId] [bigint] not null,
 constraint [PK_Reactions] primary key clustered
(
	[TrialId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off) on [Primary]) ON [Primary]
go

------------------- Operations -------------------------------

create table ops.[Attempts](
	[AttemptId] [Bigint] identity(2000,1) not null,
	[MenuPlanId] [int] not null,
	[StageIndex] [int] not null,
	[Sequence] [int] not null,
	[ItemName] [nvarchar](50) not null,
	[ItemKind] [nvarchar](50) not null,
	[OperationName] [nvarchar](50) not null,
	[CycleCount] [int] not null,
	[Activity] [nvarchar](50) not null,
	[Strategy] [nvarchar](25) not null,
	[RetryLimit] [int] not null,
	[Message] [nvarchar](250) not null,
	[Outcome] [nvarchar](50) not null,
	[Executor] [nvarchar](50) not null,
	[DeliverableId] [int]  not null,
	[AttemptedCount] [int] not null,
 constraint [PK_Attempts] primary key clustered
(
	[AttemptId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off)  on [Primary]) ON [Primary]
go

create table [ops].[Deliverables](
	[DeliverableId] [Bigint] identity(4000,1) not null,
	[Executor] [nvarchar](50) not null,
	[RequestId] [int] not null,
	[DinerIndex] [int] not null,
	[ItemIndex] [int] not null,
	[MenuPlanId] [int] not null,
	[CurrentStage] [int] not null,
	[CurrentSequence] [int] not null,
	[CurrentAttempt] [int] not null,
	[MaxSequence] [int] not null,
	[Outcome] [nvarchar](250) not null,
	[Churn] [int] not null,
	[MaxStage] [int] not null,
	[IsComplete] [int] not null,
 constraint [PK_Deliverables] primary key clustered
(
	[DeliverableId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off)  on [Primary]) ON [Primary]
go

alter table [ops].[Deliverables] add  constraint [DF_Deliverables_Executor]  default ('') for [Executor]
go

alter table [ops].[Deliverables] add  constraint [DF_Deliverables_CurrentStage]  default ((-1)) for [CurrentStage]
go

alter table [ops].[Deliverables] add  constraint [DF_Deliverables_CurrentSequence]  default ((-1)) for [CurrentSequence]
go

alter table [ops].[Deliverables] add  constraint [DF_Deliverables_CurrentAttempt]  default ((-1)) for [CurrentAttempt]
go

alter table [ops].[Deliverables] add  constraint [DF_Deliverables_Churn]  default ((0)) for [Churn]
go

create table [ops].[Requests](
	[RequestId] [Bigint] identity(6000,1) not null,
	[Origin] [BigInt] not null,
	[Initiator] [nvarchar](50) not null,
	[Contact] [nvarchar](50) not null,
	[Executor] [nvarchar](50) not null,
	[Request] [nvarchar](500) not null,
	[ReceivedAt] [datetime2](7) not null,
	[IsActive] [bit] not null,
	[ReturnedAt] [nvarchar](250) null,
	[Outcome] [nvarchar](50) null,
	[Churn] [int] not null,
 constraint [PK_Requests] primary key clustered
(
	[RequestId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off)  on [Primary]) ON [Primary]
go
------------------------------ Archives -------------------------------
create table [arc].[Attempts](
	[AttemptId] [Bigint] identity(1,1) not null,
	[MenuPlanId] [int] not null,
	[StageIndex] [int] not null,
	[Sequence] [int] not null,
	[ItemName] [nvarchar](50) not null,
	[ItemKind] [nvarchar](50) not null,
	[OperationName] [nvarchar](50) not null,
	[CycleCount] [int] not null,
	[Activity] [nvarchar](50) not null,
	[Strategy] [nvarchar](25) not null,
	[RetryLimit] [int] not null,
	[Message] [nvarchar](250) not null,
	[Outcome] [nvarchar](50) not null,
	[Executor] [nvarchar](50) not null,
	[DeliverableId] [int]  not null,
	[AttemptedCount] [int] not null,
 constraint [PK_Attempts] primary key clustered
(
	[AttemptId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off)  on [Primary]) ON [Primary]
go

create table [arc].[Deliverables](
	[DeliverableId] [Bigint] identity(1,1) not null,
	[Executor] [nvarchar](50) null,
	[RequestId] [int] null,
	[DinerIndex] [int] null,
	[ItemIndex] [int] null,
	[MenuPlanId] [int] null,
	[CurrentStage] [int] null,
	[CurrentSequence] [int] null,
	[CurrentAttempt] [int] null,
	[MaxSequence] [int] null,
	[Outcome] [nvarchar](250) null,
	[Churn] [int] null,
	[MaxStage] [int] null,
	[IsComplete] [int] null,
 constraint [PK_Deliverables] primary key clustered
(
	[DeliverableId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off)  on [Primary]) ON [Primary]
go

create table [arc].[Requests](
	[RequestId] [Bigint] identity(1,1) not null,
	[Origin] [BigInt] null,
	[Initiator] [nvarchar](50) null,
	[Contact] [nvarchar](50) null,
	[Executor] [nvarchar](50) null,
	[Request] [nvarchar](500) null,
	[ReceivedAt] [datetime2](7) null,
	[IsActive] [bit] null,
	[ReturnedAt] [nvarchar](250) null,
	[Outcome] [nvarchar](50) null,
	[Churn] [int] null,
 constraint [PK_Requests] primary key clustered
(
	[RequestId] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off)  on [Primary]) ON [Primary]
go
