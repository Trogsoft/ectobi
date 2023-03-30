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
        getVersions: (schema) => this.get(`api/schema/${schema}/versions`)
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

    populator = {
        list: () => this.get('api/ecto/populator')
    }

}
