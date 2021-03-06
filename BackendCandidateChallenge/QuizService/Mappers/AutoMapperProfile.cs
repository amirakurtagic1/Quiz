using AutoMapper;
using QuizService.Model;
using QuizService.Model.Domain;

namespace QuizService.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Quiz, QuizResponseModel>()
            .ForMember(x => x.Id, src => src.MapFrom(x => x.Id))
            .ForMember(x => x.Title, src => src.MapFrom(x => x.Title));

            CreateMap<QuizCreateModel, Quiz>()
            .ForMember(x => x.Title, src => src.MapFrom(x => x.Title));

            CreateMap<QuestionCreateModel, Question>()
            .ForMember(x => x.Text, src => src.MapFrom(x => x.Text));

            CreateMap<QuizUpdateModel, Quiz>()
            .ForMember(x => x.Title, src => src.MapFrom(x => x.Title));

            CreateMap<QuestionUpdateModel, Question>()
            .ForMember(x => x.Text, src => src.MapFrom(x => x.Text))
            .ForMember(x => x.CorrectAnswerId, src => src.MapFrom(x => x.CorrectAnswerId));

            CreateMap<Question, QuestionResponseModel>()
            .ForMember(x => x.Text, src => src.MapFrom(x => x.Text))
            .ForMember(x => x.CorrectAnswerId, src => src.MapFrom(x => x.CorrectAnswerId));

            CreateMap<AnswerUpdateModel, Answer>()
            .ForMember(x => x.Text, src => src.MapFrom(x => x.Text));
            
            CreateMap<AnswerCreateModel, Answer>()
            .ForMember(x => x.Text, src => src.MapFrom(x => x.Text));

            CreateMap<TakeQuizModel, QuizResponse>()
            .ForMember(x => x.QuizId, src => src.MapFrom(x => x.QuizId))
            .ForMember(x => x.QuestionId, src => src.MapFrom(x => x.QuestionId))
            .ForMember(x => x.AnswerId, src => src.MapFrom(x => x.AnswerId))
            .ForMember(x => x.UserId, src => src.MapFrom(x => x.UserId));

        }
    }
}