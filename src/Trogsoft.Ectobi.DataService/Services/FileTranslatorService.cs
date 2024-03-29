﻿using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class FileTranslatorService : IFileTranslatorService
    {
        private readonly ModuleManager mm;

        public FileTranslatorService(ModuleManager mm)
        {
            this.mm = mm;
        }

        public async Task<Success<List<SchemaFieldEditModel>>> GetSchemaFieldEditModelCollection(BinaryFileModel file)
        {

            var extension = file.Filename?.Split('.').Last();
            if (extension == null) return Success<List<SchemaFieldEditModel>>.Error("Filename not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var handler = mm.GetFileHandlerForFileExtension(extension);
            if (handler == null) return Success<List<SchemaFieldEditModel>>.Error("File not supported.", ErrorCodes.ERR_FILE_NOT_SUPPORTED);

            var loadResult = handler.LoadFile(file);
            if (!loadResult.Succeeded) 
                return Success<List<SchemaFieldEditModel>>.Error(loadResult.StatusMessage ?? "Unable to load file.", ErrorCodes.ERR_LOAD_FILE_FAILED);

            var headers = handler.GetHeaders();
            if (!headers.Succeeded) return Success<List<SchemaFieldEditModel>>
                    .Error(headers.StatusMessage ?? "Unable to load headers.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);

            var list = headers.Result.Select(header => new SchemaFieldEditModel { Name = header }).ToList();

            list.ForEach(x =>
            {
                var contents = handler.GetContentsOfColumn(x.Name);
                if (contents.Succeeded && contents.Result != null)
                    x.RawValues.AddRange(contents.Result);
            });

            return new Success<List<SchemaFieldEditModel>>(list);

        }

        public async Task<Success<ValueMap>> GetValueMap(BinaryFileModel file)
        {

            var extension = file.Filename?.Split('.').Last();
            if (extension == null) return Success<ValueMap>.Error("Filename not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var handler = mm.GetFileHandlerForFileExtension(extension);
            if (handler == null) return Success<ValueMap>.Error("File not supported.", ErrorCodes.ERR_FILE_NOT_SUPPORTED);

            var loadResult = handler.LoadFile(file);
            if (!loadResult.Succeeded)
                return Success<ValueMap>.Error(loadResult.StatusMessage ?? "Unable to load file.", ErrorCodes.ERR_LOAD_FILE_FAILED);

            var map = handler.GetValueMap();
            return map;

        }
    }
}
