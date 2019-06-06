CREATE TABLE [dbo].[VirtualServers](
	[VirtualServerId] [int] IDENTITY(1,1) NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[RemoveDateTime] [datetime] NULL,
	[SelectedForRemove] [bit] NOT NULL CONSTRAINT [DF_VirtualServers_SelectedForRemove]  DEFAULT ((0)),
 CONSTRAINT [PK_VirtualServers] PRIMARY KEY CLUSTERED 
(
	[VirtualServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


