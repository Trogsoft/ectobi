export class ectoClient {

    host = 'localhost:7247';
    constructor() {
    }

    get(url) {
        return new Promise((resolve, reject) => {
            ajax({
                url: `https://${this.host}/${url}`
            }).get().then({
                success: h => resolve(h),
                fail: h => reject(h)
            });
        });
    }

    delete(url) {
        return new Promise((resolve, reject) => {
            ajax({
                url: `https://${this.host}/${url}`
            }).delete().then({
                success: h => resolve(h),
                fail: h => reject(h)
            });
        });
    }

    postJson(url, model) {
        return new Promise((resolve, reject)=>{
            ajax({
                url: `https://${this.host}/${url}`,
                data: model
            }).postJson().then({
                success: h => resolve(h),
                fail: h => reject(h)
            })
        });
    }

    schema = {
        list: () => this.get(`api/schema`),
        get: (schema) => this.get(`api/schema/${schema}`),
        delete: (schema) => this.delete(`api/schema/${schema}`),
        getVersions: (schema) => this.get(`api/schema/${schema}/versions`),
        create: (model) => this.postJson(`api/schema`, model)
    };

    batch = {
        list: (schemaTid) => this.get(`api/batch/${schemaTid}`),
        createEmpty: (batchModel) => this.postJson(`api/batch/empty`, batchModel)
    };

    field = {
        list: (schemaTid) => this.get(`api/field/${schemaTid}`),
        listBySchemaVersion: (schemaTid, version) => this.get(`api/field/${schemaTid}/version/${version}`),
        create: (schemaTid, model) => this.postJson(`api/field/${schemaTid}`, model),
        delete: (schemaTid, fieldTid) => this.delete(`api/field/${schemaTid}/${fieldTid}`),
        getLatest: (schemaTid, fieldTid) => this.get(`api/field/${schemaTid}/${fieldTid}`)
    }

    lookup = {
        list: () => this.get(`api/lookup`),
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

}
