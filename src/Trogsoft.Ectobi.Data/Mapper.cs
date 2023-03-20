using AutoMapper;
using AutoMapper.Configuration.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.Data
{
    public class EctoMapper : Common.Interfaces.IEctoMapper
    {

        MapperConfiguration mpc = new MapperConfiguration(cfg =>
        {

            cfg.CreateMap<SchemaModel, Schema>()
                .Include<SchemaEditModel, Schema>()
                .ForMember(x => x.SchemaFields, y => y.MapFrom(z => z.Fields))
                .ReverseMap()
                .ForMember(x => x.Fields, y => y.MapFrom(z => z.SchemaFields));

            cfg.CreateMap<SchemaEditModel, Schema>()
                .ForMember(x => x.SchemaFields, y => y.MapFrom(z => z.Fields))
                .ReverseMap();

            cfg.CreateMap<SchemaFieldModel, SchemaField>()
                .Include<SchemaFieldEditModel, SchemaField>()
                .ReverseMap();

            cfg.CreateMap<SchemaFieldEditModel, SchemaField>().ReverseMap();


        });

        private readonly IMapper mapper;

        public EctoMapper()
        {
            mapper = mpc.CreateMapper();
        }

        public TDest Map<TDest>(object source) => mapper.Map<TDest>(source);

        public TDest Map<TSource, TDest>(TSource source) => mapper.Map<TSource, TDest>(source);

    }
}
