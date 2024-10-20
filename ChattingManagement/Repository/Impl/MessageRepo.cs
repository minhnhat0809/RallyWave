using Entity;

namespace ChattingManagement.Repository.Impl;

public class MessageRepo(RallyWaveContext repositoryContext) : RepositoryBase<Message>(repositoryContext), IMessageRepo
{
    
}