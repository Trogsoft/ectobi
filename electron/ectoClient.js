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

    schema = {
        list: () => this.get(`api/schema`),
        getVersions: (schema) => this.get(`api/schema/${schema}/versions`)
    };
}
