/**
 * @typedef {Object} Success
 * @property {bool} succeeded - True or false depending on the success of the operation
 * @property {*} result - the result of the operation, if any.
 * @property {string} statusMessage - a status message usually accompanying a failure response
 * @property {int} errorCode - a numeric error code  
 */

export class ectoClient {

    host = 'localhost:7247';
    headers = {};
    token;

    constructor(token) {
        this.updateToken(token);
    }

    /**
     * Get information about the current server
     * @returns {Success} Information about the connected server.
     * @property {bool} result.requiresLogin - does this server require login? 
     * @property {string} result.name - the nameo of the connected server. 
     */
    getServerInfo = () => this.get('api/ecto/server');

    model = {
        /**
         * Get a list of Models defined on the server.
         * @returns A Success object where the Result is a list of Models
         */
        list: () => this.get('api/model'),
        /**
         * Add model properties to the current schema
         * @param {Object} model - details of the model and which schema to apply it to
         * @param {string} model.schemaTid - the schema to add model properties to 
         * @param {string} model.modelName - the model whose properties should be applied
         * @param {string[]} model.properties = the properties to apply to the schema 
         * @returns {Success} a Success object containing a list of SchemaFieldModels as its result. 
         */
        configure: (model) => this.postJson('api/model/config', model)
    }

    data = {
        getFieldFilters: (schema) => this.get('api/data/fieldFilter/'+schema),
        query: (query) => this.postJson('api/data/query', query)
    }

    auth = {
        login: (username, password) => this.postJson('api/auth', { username: username, password: password })
    }

    schema = {
        list: () => this.get(`api/schema`),
        get: (schema) => this.get(`api/schema/${schema}`),
        delete: (schema) => this.delete(`api/schema/${schema}`),
        getVersions: (schema) => this.get(`api/schema/${schema}/versions`),
        create: (model) => this.postJson(`api/schema`, model),
        createEmpty: (model) => this.postJson(`api/schema/empty`, model)
    };

    batch = {
        list: (schemaTid) => this.get(`api/batch/${schemaTid}`),
        createEmpty: (batchModel) => this.postJson(`api/batch/empty`, batchModel),
        upload: (model) => this.postJson('api/batch', model),
        delete: (id) => this.delete(`api/batch/${id}`)
    };

    field = {
        list: (schemaTid) => this.get(`api/field/${schemaTid}`),
        listBySchemaVersion: (schemaTid, version) => this.get(`api/field/${schemaTid}/version/${version}`),
        create: (schemaTid, model) => this.postJson(`api/field/${schemaTid}`, model),
        delete: (schemaTid, fieldTid) => this.delete(`api/field/${schemaTid}/${fieldTid}`),
        update: (schemaTid, model) => this.putJson(`api/field/${schemaTid}`, model),
        getLatest: (schemaTid, fieldTid) => this.get(`api/field/${schemaTid}/${fieldTid}`)
    }

    lookup = {
        list: (schema) => this.get(`api/lookup/?schemaTid=${schema}`),
        get: (id) => this.get(`api/lookup/${id}`),
        create: (model) => this.postJson(`api/lookup`, model)
    }

    populator = {
        list: () => this.get('api/ecto/populator')
    }

    importer = {
        list: () => this.get('api/ecto/importer')
    }

    webhook = {
        list: () => this.get('api/ecto/webhook'),
        get: (id) => this.get(`api/ecto/webhook/${id}`)
    }

    updateToken = (token) => {
        this.token = token;
        this.headers = {
            'Authorization': 'Bearer ' + token
        }
    }

    get = (url) => {
        return new Promise((resolve, reject) => {
            ajax({
                url: `https://${this.host}/${url}`,
                headers: this.headers
            }).get().then({
                success: h => resolve(h),
                fail: h => reject(h)
            });
        });
    }

    delete = (url) => {
        return new Promise((resolve, reject) => {
            ajax({
                url: `https://${this.host}/${url}`,
                headers: this.headers
            }).delete().then({
                success: h => resolve(h),
                fail: h => reject(h),
                error: h => reject(h)
            });
        });
    }


    postJson = (url, model) => {
        return new Promise((resolve, reject) => {
            ajax({
                url: `https://${this.host}/${url}`,
                data: model,
                headers: this.headers
            }).postJson().then({
                success: h => resolve(h),
                fail: h => reject(h),
                error: h => reject(h)
            })
        });
    }

    putJson = (url, model) => {
        return new Promise((resolve, reject) => {
            ajax({
                url: `https://${this.host}/${url}`,
                data: model,
                headers: this.headers
            }).putJson().then({
                success: h => resolve(h),
                fail: h => reject(h),
                error: h => reject(h)
            })
        });
    }

}
