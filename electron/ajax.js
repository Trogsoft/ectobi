const ajax = function (config) {

    let a = {
    };

    let xhr = new XMLHttpRequest();
    xhr.onload = loadHandler;

    if (!config.url)
        throw new Error('Unspecified URL.');

    a.url = config.url;
    a.data = config.data;
    a.headers = config.headers || {};

    a.get = function () {
        var qs = '';
        if (a.data) {
            qs = Object.keys(a.data).map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(a.data[key])
            }).join('&');
        }
        xhr.open('get', a.url + (qs != '' ? '?' + qs : ''), true);
        if (a.headers) {
            Object.keys(a.headers).forEach(h => {
                xhr.setRequestHeader(h, a.headers[h]);
            })
        }
        xhr.send();
        return a;
    }

    a.delete = function () {
        var qs = '';
        if (a.data) {
            qs = Object.keys(a.data).map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(a.data[key])
            }).join('&');
        }
        xhr.open('delete', a.url + (qs != '' ? '?' + qs : ''), true);
        if (a.headers) {
            Object.keys(a.headers).forEach(h => {
                xhr.setRequestHeader(h, a.headers[h]);
            })
        }
        xhr.send();
        return a;
    }

    a.post = function () {
        var qs = '';
        if (a.data) {
            qs = Object.keys(a.data).map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(a.data[key])
            }).join('&');
        }
        xhr.open('post', a.url, true);
        if (a.headers) {
            Object.keys(a.headers).forEach(h => {
                xhr.setRequestHeader(h, a.headers[h]);
            })
        }
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send(qs);
        return a;
    }

    a.postJson = function () {

        xhr.open('post', a.url);
        if (a.headers) {
            Object.keys(a.headers).forEach(h => {
                xhr.setRequestHeader(h, a.headers[h]);
            })
        }
        xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8")
        xhr.send(JSON.stringify(a.data));
        return a;

    }

    a.putJson = function () {

        xhr.open('put', a.url);
        if (a.headers) {
            Object.keys(a.headers).forEach(h => {
                xhr.setRequestHeader(h, a.headers[h]);
            })
        }
        xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8")
        xhr.send(JSON.stringify(a.data));
        return a;

    }

    a.then = function (doneHandlers) {
        if (doneHandlers.success)
            a.success = doneHandlers.success;

        if (doneHandlers.error)
            a.error = doneHandlers.error;

        if (doneHandlers.fail)
            a.fail = doneHandlers.fail;
    }

    function loadHandler() {
        if (xhr.status === 200) {
            var o = JSON.parse(xhr.responseText);
            if (o.succeeded) {
                standardSuccess(o);
                if (a.success)
                    a.success(o);
            } else {
                if (a.fail) {
                    if (a.fail(o) !== false)
                        standardFail(o);
                } else
                    standardFail(o);
            }
        } else {
            try {
                var o = JSON.parse(xhr.responseText);
                if (a.error) {
                    if (a.error(o) !== false)
                        standardError(o);
                } else
                    standardError(o);
            } catch (error) {
                var o = { errorCode: xhr.status, statusMessage: xhr.responseText };
                if (a.error) {
                    if (a.error(o) !== false)
                        standardError(o);
                } else
                    standardError(o);
            }
        }
    }

    function standardSuccess(obj) {
    }

    function standardError(obj) {
        window.ipc.alert({ message: `Error ${obj.errorCode}: ${obj.statusMessage}`, type: 'error' })
    }

    function standardFail(obj) {
        window.ipc.alert({ message: `Error ${obj.errorCode}: ${obj.statusMessage}`, type: 'error' })
    }

    return a;

}
