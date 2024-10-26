using AutoMapper;
using System.ComponentModel;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent
{
    public class UpdateComponentMapper : Profile
    {
        public UpdateComponentMapper()
        {
            CreateMap<UpdateComponentCommand, Component>();
        }
    }
}
