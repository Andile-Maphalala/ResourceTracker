using AutoMapper;
using System.ComponentModel;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest
{
    public class UpdateQuestMapper : Profile
    {
        public UpdateQuestMapper()
        {
            CreateMap<UpdateQuestCommand, Component>();
        }
    }
}
