using Entity;

namespace ChattingManagement.Repository.Impl;

public class MessageRepo(RallywaveContext repositoryContext) : RepositoryBase<Message>(repositoryContext), IMessageRepo
{
    
}