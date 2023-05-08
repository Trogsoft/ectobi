const fieldFlags = {
    1: 'Allow Null',
    2: 'Numeric ID',
    4: 'Display Value',
    8: 'Required At Import',
    16: 'Personally Identifiable Information'
};

const filterType = {
    set: 1
}

const fieldType = {
    Text: 1,
    Integer: 2,
    Decimal: 3,
    YesNo: 4,
    LookupSet: 5,
    Script: 6,
    Populator: 7,
    DateTime: 8,

    getName(value) {
        var name = 'Unknown';
        Object.keys(this).forEach(x => {
            if (this[x] == value)
                name = x.toString();
        });
        return name;
    }
};

const ectoModelPropertyFlags = {
    None: 0,
    PersonallyIdentifiableInformation: 1,
    GeographicIdentifier: 2,
    EmailAddress: 4,
    PhoneNumber: 8,
    SuggestedDefault: 16
};

const webHookEventType = {
    1: { textId: 'SchemaCreated', name: 'Schema Created' },
    2: { textId: 'SchemaUpdated', name: 'Schema Updated' },
    4: { textId: 'SchemaDeleted', name: 'Schema Deleted' },
    8: { textId: 'FieldCreated', name: 'Field Created' },
    16: { textId: 'FieldUpdated', name: 'Field Updated' },
    32: { textId: 'FieldDeleted', name: 'Field Deleted' },
    64: { textId: 'SchemaVersionCreated', name: 'Schema Version Created' },
    128: { textId: 'SchemaVersionUpdated', name: 'Schema Version Updated' },
    256: { textId: 'SchemaVersionDeleted', name: 'Schema Version Deleted' },
    512: { textId: 'LookupValueCreated', name: 'Lookup Value Created' },
    1024: { textId: 'LookupValueUpdated', name: 'Lookup Value Updated' },
    2048: { textId: 'LookupValueDeleted', name: 'Lookup Value Deleted' },
    4096: { textId: 'BatchCreated', name: 'Batch Created' },
    8192: { textId: 'BatchUpdated', name: 'Batch Updated' },
    16384: { textId: 'BatchDeleted', name: 'Batch Deleted' },
    32768: { textId: 'RecordCreated', name: 'Record Created' },
    65535: { textId: 'RecordUpdated', name: 'Record Updated' },
    131072: { textId: 'RecordDeleted', name: 'Record Deleted' },

    getNumericValue(textId) {
        Object.keys(this).forEach(key => {
            if (this[key] == textId) {
                return parseInt(key);
            }
        })
    }
}

export { fieldFlags, fieldType, webHookEventType, ectoModelPropertyFlags, filterType }