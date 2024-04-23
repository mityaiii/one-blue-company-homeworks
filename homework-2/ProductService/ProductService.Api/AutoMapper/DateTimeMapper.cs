using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace ProductService.Api.AutoMapper;

public class DateTimeMapper : Profile
{
    public DateTimeMapper()
    {
        CreateMap<Timestamp, DateTime>().ConstructUsing(x => x.ToDateTime());
        CreateMap<DateTime, Timestamp>().ConstructUsing(x => Timestamp.FromDateTime(x));
    }
}