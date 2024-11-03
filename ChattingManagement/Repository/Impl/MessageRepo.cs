using Entity;

namespace ChattingManagement.Repository.Impl;

public interface IMessageRepo : IRepositoryBase<Message>
{
    
}
public class MessageRepo(RallyWaveContext repositoryContext) : RepositoryBase<Message>(repositoryContext), IMessageRepo
{
    
}