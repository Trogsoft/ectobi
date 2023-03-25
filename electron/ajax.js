const ajax = function (config) {

    let a = {
    };

    let xhr = new XMLHttpRequest();
    xhr.onload = loadHandler;

    if (!config.url)
        throw new Error('Unspecified URL.');

    a.url = config.url;
    a.data = config.data;

    a.get = function () {
        var qs = '';
        if (a.data) {
            qs = Object.keys(a.data).map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(a.data[key])
            }).join('&');
        }
        xhr.open('get', a.url + (qs != '' ? '?' + qs : ''), true);
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
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send(qs);
        return a;
    }

    a.postJson = function () {

        xhr.open('post', a.url);
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
                //ajaxConfig.events.trigger('success', o);
                standardSuccess(o);
                if (a.success)
                    a.success(o);
            } else {
                //ajaxConfig.events.trigger('fail', o);
                if (a.fail) {
                    if (a.fail(o) !== false)
                        standardFail(o);
                } else
                    standardFail(o);
            }
        } else {
            try {
                var o = JSON.parse(xhr.responseText);
                standardError(xhr.status, xhr.responseText, o);
                if (a.error)
                    var res = a.error(xhr.status, xhr.responseText, o);
            } catch (error) {
                standardError(xhr.status, xhr.responseText);
                if (a.error)
                    var res = a.error(xhr.status, xhr.responseText);
            }
        }
    }

    function standardSuccess(obj) {
    }

    function standardError(st, rawResponse, obj) {
    }

    function standardFail(obj) {
        alert.show(obj.statusMessage);
    }

    return a;

}