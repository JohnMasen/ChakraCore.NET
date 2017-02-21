function add(s) {
    return s + s
}
function addcallback(s, callback) {
    return s + callback(s)
}