using AutoMapper;
using AutoMapper.Configuration.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

            cfg.CreateMap<SchemaVersion, SchemaVersionModel>()
                .ReverseMap();

            cfg.CreateMap<SchemaEditModel, Schema>()
                .ForMember(x => x.SchemaFields, y => y.MapFrom(z => z.Fields))
                .ReverseMap();

            cfg.CreateMap<SchemaFieldModel, SchemaField>()
                .Include<SchemaFieldEditModel, SchemaField>()
                .ReverseMap();

            cfg.CreateMap<SchemaFieldModel, SchemaFieldVersion>()
                .ForMember(x => x.Id, y => y.Ignore())
                .ReverseMap();

            cfg.CreateMap<Schema, SchemaVersion>()
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.Schema, y => y.Ignore())
                .ForMember(x => x.Created, y => y.MapFrom(z => DateTime.Now))
                .ForMember(x => x.Fields, y => y.Ignore())
                .ReverseMap();

            cfg.CreateMap<SchemaField, SchemaFieldVersion>()
                .ForMember(x => x.Id, y => y.MapFrom(z => 0))
                .ReverseMap();

            cfg.CreateMap<SchemaField, SchemaFieldEditModel>()
                .ReverseMap();

            cfg.CreateMap<SchemaFieldVersion, SchemaFieldEditModel>()
                .ReverseMap();

            cfg.CreateMap<SchemaFieldVersion, SchemaFieldVersion>()
                .ForMember(x => x.Id, y => y.MapFrom(z => 0));

            cfg.CreateMap<LookupSet, LookupSetModel>().ReverseMap();

            cfg.CreateMap<LookupSetValue, LookupSetValueModel>().ReverseMap();

            cfg.CreateMap<SchemaFieldEditModel, SchemaField>()
                .ReverseMap();

            cfg.CreateMap<BatchModel, Batch>()
                .ReverseMap();

            cfg.CreateMap<WebHook, WebHookModel>().ReverseMap();

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
