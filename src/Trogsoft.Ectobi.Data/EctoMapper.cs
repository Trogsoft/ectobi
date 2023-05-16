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

            // This is used in ISchemaData when creating a schema
            cfg.CreateMap<SchemaEditModel, Schema>()
                .ReverseMap();

            // This is used in CreateSchema to create the first SchemaVersion
            cfg.CreateMap<Schema, SchemaVersionEditModel>();

            // This is used in CreateSchemaVersion to create the entity for insertion into the database.
            cfg.CreateMap<SchemaVersionEditModel, SchemaVersion>();

            cfg.CreateMap<SchemaFieldModel, SchemaFieldEditModel>();

            cfg.CreateMap<SchemaFieldModel, SchemaField>()
                .Include<SchemaFieldEditModel, SchemaField>()
                .ReverseMap();

            #region FieldFilter Stuff

            cfg.CreateMap<SchemaFieldVersion, FieldFilterModel>()
                .ForMember(x => x.Options, y => y.MapFrom(z => z.LookupSet.Values.OrderBy(q => q.Name)))
                .ReverseMap();

            cfg.CreateMap<LookupSetValue, FieldFilterOption>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.NumericValue));

            #endregion

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

            cfg.CreateMap<SchemaFieldVersion, SchemaFieldModel>()
                .ForMember(x => x.Populator, y => y.MapFrom(z => z.Populator != null ? z.Populator.TextId : null))
                .ForMember(x => x.LookupTid, y => y.MapFrom(z => z.LookupSet != null ? z.LookupSet.TextId : null))
                .ForMember(x => x.ModelName, y => y.MapFrom(z => z.Model != null ? z.Model.TextId : null))
                .ReverseMap();

            cfg.CreateMap<SchemaField, SchemaFieldEditModel>()
                .ReverseMap();

            cfg.CreateMap<SchemaFieldVersion, SchemaFieldEditModel>()
                .ForMember(x => x.Populator, y => y.MapFrom(z => z.Populator != null ? z.Populator.TextId : null))
                .ForMember(x => x.LookupTid, y => y.MapFrom(z => z.LookupSet != null ? z.LookupSet.TextId : null))
                .ForMember(x => x.ModelName, y => y.MapFrom(z => z.Model != null ? z.Model.TextId : null))
                .ReverseMap()
                .ForMember(x => x.Populator, y => y.Ignore());

            cfg.CreateMap<SchemaFieldVersion, SchemaFieldVersion>()
                .ForMember(x => x.Id, y => y.MapFrom(z => 0));

            cfg.CreateMap<LookupSet, LookupSetModel>().ReverseMap()
                .ForMember(x => x.Values, y => y.MapFrom(z => z.Values.OrderBy(q => q.Name)))
                .ReverseMap();

            cfg.CreateMap<LookupSetValue, LookupSetValueModel>()
                .ForMember(x => x.Value, y => y.MapFrom(z => z.NumericValue))
                .ReverseMap()
                .ForMember(x => x.NumericValue, y => y.MapFrom(z => z.Value));

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
