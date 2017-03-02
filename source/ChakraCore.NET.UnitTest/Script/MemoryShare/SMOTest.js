function arrayBufferAdd(a) {
    let aa = new Int8Array(a);
    typedArrayAdd(aa);
}

function typedArrayAdd(a) {
    for (var i = 0; i < a.byteLength; i++) {
        a[i] += a[i];
    }
}

function dataViewAdd(d) {
    for (var i = 0; i < d.byteLength; i++) {
        d.setInt8(d.getInt8(i) * 2);
    }
}

